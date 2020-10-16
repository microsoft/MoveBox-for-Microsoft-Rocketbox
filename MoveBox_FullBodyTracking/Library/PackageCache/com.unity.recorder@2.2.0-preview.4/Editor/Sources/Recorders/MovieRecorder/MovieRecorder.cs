using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.Media;

namespace UnityEditor.Recorder
{
    class MovieRecorder : BaseTextureRecorder<MovieRecorderSettings>
    {
        MediaEncoder m_Encoder;

        protected override TextureFormat ReadbackTextureFormat
        {
            get { return TextureFormat.RGBA32; }
        }

        protected internal override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session))
                return false;

            try
            {
                Settings.fileNameGenerator.CreateDirectory(session);
            }
            catch (Exception)
            {
                Debug.LogError(string.Format( "Movie recorder output directory \"{0}\" could not be created.", Settings.fileNameGenerator.BuildAbsolutePath(session)));
                return false;
            }

            var input = m_Inputs[0] as BaseRenderTextureInput;
            if (input == null)
            {
                Debug.LogError("MediaRecorder could not find input.");
                return false;
            }
            int width = input.OutputWidth;
            int height = input.OutputHeight;

            if (width <= 0 || height <= 0)
            {
                Debug.LogError(string.Format("MovieRecorder got invalid input resolution {0} x {1}.", width, height));
                return false;
            }

            if (Settings.OutputFormat == MovieRecorderSettings.VideoRecorderOutputFormat.MP4)
            {
                if (width > 4096 || height > 4096)
                {
                    Debug.LogWarning(string.Format("Mp4 format might not support resolutions bigger than 4096. Current resolution: {0} x {1}.", width, height));
                }

                if (width % 2 != 0 || height % 2 != 0)
                {
                    Debug.LogError(string.Format("Mp4 format does not support odd values in resolution. Current resolution: {0} x {1}.", width, height));
                    return false;
                }
            }

            var imageInputSettings = m_Inputs[0].settings as ImageInputSettings;

            var includeAlphaFromTexture = imageInputSettings != null && imageInputSettings.SupportsTransparent && imageInputSettings.AllowTransparency;

            if (includeAlphaFromTexture && Settings.OutputFormat == MovieRecorderSettings.VideoRecorderOutputFormat.MP4)
            {
                Debug.LogWarning("Mp4 format does not support alpha.");
                includeAlphaFromTexture = false;
            }

            var videoAttrs = new VideoTrackAttributes
            {
                frameRate = RationalFromDouble(session.settings.FrameRate),
                width = (uint)width,
                height = (uint)height,
                includeAlpha = includeAlphaFromTexture,
                bitRateMode = Settings.VideoBitRateMode
            };

            if (RecorderOptions.VerboseMode)
                Debug.Log(
                    string.Format(
                        "MovieRecorder starting to write video {0}x{1}@[{2}/{3}] fps into {4}",
                        width, height, videoAttrs.frameRate.numerator,
                        videoAttrs.frameRate.denominator, Settings.fileNameGenerator.BuildAbsolutePath(session)));

            var audioInput = (AudioInput)m_Inputs[1];
            var audioAttrsList = new List<AudioTrackAttributes>();

            if (audioInput.audioSettings.PreserveAudio)
            {
#if UNITY_EDITOR_OSX
                // Special case with WebM and audio on older Apple computers: deactivate async GPU readback because there
                // is a risk of not respecting the WebM standard and receiving audio frames out of sync (see "monotonically
                // increasing timestamps"). This happens only with Target Cameras.
                if (m_Inputs[0].settings is CameraInputSettings && Settings.OutputFormat == MovieRecorderSettings.VideoRecorderOutputFormat.WebM)
                {
                    UseAsyncGPUReadback = false;
                }
#endif
                var audioAttrs = new AudioTrackAttributes
                {
                    sampleRate = new MediaRational
                    {
                        numerator = audioInput.sampleRate,
                        denominator = 1
                    },
                    channelCount = audioInput.channelCount,
                    language = ""
                };

                audioAttrsList.Add(audioAttrs);

                if (RecorderOptions.VerboseMode)
                    Debug.Log(string.Format("MovieRecorder starting to write audio {0}ch @ {1}Hz", audioAttrs.channelCount, audioAttrs.sampleRate.numerator));
            }
            else
            {
                if (RecorderOptions.VerboseMode)
                    Debug.Log("MovieRecorder starting with no audio.");
            }

            try
            {
                var path =  Settings.fileNameGenerator.BuildAbsolutePath(session);

                m_Encoder = new MediaEncoder( path, videoAttrs, audioAttrsList.ToArray() );
                return true;
            }
            catch
            {
                if (RecorderOptions.VerboseMode)
                    Debug.LogError("MovieRecorder unable to create MovieEncoder.");
            }

            return false;
        }

        protected internal override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 2)
                throw new Exception("Unsupported number of sources");

            base.RecordFrame(session);
            var audioInput = (AudioInput)m_Inputs[1];
            if (audioInput.audioSettings.PreserveAudio)
                m_Encoder.AddSamples(audioInput.mainBuffer);
        }

        protected override void WriteFrame(Texture2D t)
        {
            m_Encoder.AddFrame(t);
        }

#if UNITY_2019_1_OR_NEWER
        protected override void WriteFrame(AsyncGPUReadbackRequest r)
        {
            m_Encoder.AddFrame(r.width, r.height, 0, TextureFormat.RGBA32, r.GetData<byte>());
        }
#endif

        protected override void DisposeEncoder()
        {
            base.DisposeEncoder();

            if (m_Encoder == null)
                return;

            m_Encoder.Dispose();
            m_Encoder = null;

            // When adding a file to Unity's assets directory, trigger a refresh so it is detected.
            if (Settings.fileNameGenerator.Root == OutputPath.Root.AssetsFolder || Settings.fileNameGenerator.Root == OutputPath.Root.StreamingAssets)
                AssetDatabase.Refresh();
        }

        // https://stackoverflow.com/questions/26643695/converting-decimal-to-fraction-c
        static long GreatestCommonDivisor(long a, long b)
        {
            if (a == 0)
                return b;

            if (b == 0)
                return a;

            return (a < b) ? GreatestCommonDivisor(a, b % a) : GreatestCommonDivisor(b, a % b);
        }

        static MediaRational RationalFromDouble(double value)
        {
            var integral = Math.Floor(value);
            var frac = value - integral;

            const long precision = 10000000;

            var gcd = GreatestCommonDivisor((long)Math.Round(frac * precision), precision);
            var denom = precision / gcd;

            return new MediaRational()
            {
                numerator = (int)((long)integral * denom + ((long)Math.Round(frac * precision)) / gcd),
                denominator = (int)denom
            };
        }
    }
}
