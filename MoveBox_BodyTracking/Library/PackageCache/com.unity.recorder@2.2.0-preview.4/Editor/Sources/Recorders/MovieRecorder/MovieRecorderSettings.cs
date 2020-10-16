using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.Recorder
{
    [RecorderSettings(typeof(MovieRecorder), "Movie", "movie_16")]
    public class MovieRecorderSettings : RecorderSettings
    {
        /// <summary>
        /// Available options for the output video format used by Movie Recorder.
        /// </summary>
        public enum VideoRecorderOutputFormat
        {
            /// <summary>
            /// Output the recording in MP4 format.
            /// </summary>
            MP4,
            /// <summary>
            /// Output the recording in WebM format.
            /// </summary>
            WebM
        }
        /// <summary>
        /// Indicates the output video format currently used for this Recorder.
        /// </summary>
        public VideoRecorderOutputFormat OutputFormat
        {
            get { return outputFormat; }
            set { outputFormat = value; }
        }

        [SerializeField] VideoRecorderOutputFormat outputFormat = VideoRecorderOutputFormat.MP4;

        /// <summary>
        /// Indicates the video bit rate preset currently used for this Recorder.
        /// </summary>
        public VideoBitrateMode VideoBitRateMode
        {
            get { return videoBitRateMode; }
            set { videoBitRateMode = value; }
        }

        [SerializeField] private VideoBitrateMode videoBitRateMode = VideoBitrateMode.High;

        /// <summary>
        /// Use this property to capture the alpha channel (True) or not (False).
        /// </summary>
        public bool CaptureAlpha
        {
            get { return captureAlpha; }
            set { captureAlpha = value; }
        }

        [SerializeField] private bool captureAlpha;

        [SerializeField] ImageInputSelector m_ImageInputSelector = new ImageInputSelector();
        [SerializeField] AudioInputSettings m_AudioInputSettings = new AudioInputSettings();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MovieRecorderSettings()
        {
            fileNameGenerator.FileName = "movie";
            FrameRate = 30;

            var iis = m_ImageInputSelector.Selected as StandardImageInputSettings;
            if (iis != null)
                iis.maxSupportedSize = ImageHeight.x2160p_4K;

            m_ImageInputSelector.ForceEvenResolution(OutputFormat == VideoRecorderOutputFormat.MP4);
        }

        /// <summary>
        /// Indicates the Image Input Settings currently used for this Recorder.
        /// </summary>
        public ImageInputSettings ImageInputSettings
        {
            get { return m_ImageInputSelector.ImageInputSettings; }
            set { m_ImageInputSelector.ImageInputSettings = value; }
        }

        /// <summary>
        /// Indicates the Audio Input Settings currently used for this Recorder.
        /// </summary>
        public AudioInputSettings AudioInputSettings
        {
            get { return m_AudioInputSettings; }
        }

        /// <inheritdoc/>
        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get
            {
                yield return m_ImageInputSelector.Selected;
                yield return m_AudioInputSettings;
            }
        }

        /// <inheritdoc/>
        protected internal override string Extension
        {
            get { return OutputFormat.ToString().ToLower(); }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = base.ValidityCheck(errors);

            if (FrameRatePlayback == FrameRatePlayback.Variable)
            {
                errors.Add("Movie recorder does not properly support Variable frame rate playback. Please consider using Constant frame rate instead");
                ok = false;
            }

            if (OutputFormat == VideoRecorderOutputFormat.MP4)
            {
                var iis = m_ImageInputSelector.Selected as ImageInputSettings;
                if (iis != null)
                {
                    if (iis.OutputHeight % 2 != 0 || iis.OutputWidth % 2 != 0)
                    {
                        errors.Add("Mp4 format does not support odd values in resolution");
                        ok = false;
                    }
                }
            }

            return ok;
        }

        internal override void SelfAdjustSettings()
        {
            var selectedInput = m_ImageInputSelector.Selected;
            if (selectedInput == null)
                return;

            var iis = selectedInput as StandardImageInputSettings;

            if (iis != null)
            {
                iis.maxSupportedSize = OutputFormat == VideoRecorderOutputFormat.MP4
                    ? ImageHeight.x2160p_4K
                    : ImageHeight.x4320p_8K;

                if (iis.outputImageHeight != ImageHeight.Window && iis.outputImageHeight != ImageHeight.Custom)
                {
                    if (iis.outputImageHeight > iis.maxSupportedSize)
                        iis.outputImageHeight = iis.maxSupportedSize;
                }
            }

            var cbis = selectedInput as ImageInputSettings;
            if (cbis != null)
            {
                cbis.AllowTransparency = OutputFormat == VideoRecorderOutputFormat.WebM && CaptureAlpha;
            }

            var gis = selectedInput as GameViewInputSettings;
            if (gis != null)
                gis.FlipFinalOutput = SystemInfo.supportsAsyncGPUReadback;

            m_ImageInputSelector.ForceEvenResolution(OutputFormat == VideoRecorderOutputFormat.MP4);
        }
    }
}
