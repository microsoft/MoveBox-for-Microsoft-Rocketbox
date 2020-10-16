using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace UnityEditor.Recorder
{
    class ImageRecorder : BaseTextureRecorder<ImageRecorderSettings>
    {
        Queue<string> m_PathQueue = new Queue<string>();

        protected override TextureFormat ReadbackTextureFormat
        {
            get
            {
                return Settings.OutputFormat != ImageRecorderSettings.ImageRecorderOutputFormat.EXR ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat;
            }
        }

        protected internal override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            Settings.fileNameGenerator.CreateDirectory(session);

            return true;
        }

        protected internal override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");
            // Store path name for this frame into a queue, as WriteFrame may be called
            // asynchronously later on, when the current frame is no longer the same (thus creating
            // a file name that isn't in sync with the session's current frame).
            m_PathQueue.Enqueue(Settings.fileNameGenerator.BuildAbsolutePath(session));
            base.RecordFrame(session);
        }

        protected override void WriteFrame(Texture2D tex)
        {
            byte[] bytes;
            Profiler.BeginSample("ImageRecorder.EncodeImage");
            try
            {
                switch (Settings.OutputFormat)
                {
                case ImageRecorderSettings.ImageRecorderOutputFormat.PNG:
                    bytes = tex.EncodeToPNG();
                    break;
                case ImageRecorderSettings.ImageRecorderOutputFormat.JPEG:
                    bytes = tex.EncodeToJPG();
                    break;
                case ImageRecorderSettings.ImageRecorderOutputFormat.EXR:
                    bytes = tex.EncodeToEXR();
                    break;
                default:
                    Profiler.EndSample();
                    throw new ArgumentOutOfRangeException();
                }
            }
            finally
            {
                Profiler.EndSample();
            }

            if(m_Inputs[0] is BaseRenderTextureInput || Settings.OutputFormat != ImageRecorderSettings.ImageRecorderOutputFormat.JPEG)
                UnityHelpers.Destroy(tex);

            WriteToFile(bytes);
        }

        private void WriteToFile(byte[] bytes)
        {
            Profiler.BeginSample("ImageRecorder.WriteToFile");
            File.WriteAllBytes(m_PathQueue.Dequeue(), bytes);
            Profiler.EndSample();
        }
    }
}
