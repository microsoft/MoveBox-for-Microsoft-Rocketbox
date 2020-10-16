using System;
using UnityEngine;

namespace UnityEditor.Recorder.FrameCapturer
{
    class MP4Recorder : GenericRecorder<MP4RecorderSettings>
    {
        fcAPI.fcMP4Context m_ctx;
        
        protected internal override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            Settings.fileNameGenerator.CreateDirectory(session);

            var input = (BaseRenderTextureInput)m_Inputs[0];
            if (input.OutputWidth > 4096 || input.OutputHeight > 2160 )
            {
                Debug.LogError("Mp4 format does not support resolutions larger than 4096x2160.");
            }

            return true;
        }

        protected internal override void EndRecording(RecordingSession session)
        {
            m_ctx.Release();
            base.EndRecording(session);
        }

        protected internal override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (BaseRenderTextureInput)m_Inputs[0];
            var frame = input.OutputRenderTexture;

            if(!m_ctx)
            {
                var s = Settings.m_MP4EncoderSettings;
                s.video = true;
                s.audio = false;
                s.videoWidth = frame.width;
                s.videoHeight = frame.height;
                s.videoTargetFramerate = (int)Math.Ceiling(Settings.FrameRate);
                if (Settings.m_AutoSelectBR)
                {
                    s.videoTargetBitrate = (int)(( (frame.width * frame.height/1000.0) / 245 + 1.16) * (s.videoTargetFramerate / 48.0 + 0.5) * 1000000);
                }
                var path = Settings.fileNameGenerator.BuildAbsolutePath(session);
                m_ctx = fcAPI.fcMP4OSCreateContext(ref s, path);
            }

            fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                fcAPI.fcMP4AddVideoFramePixels(m_ctx, data, fmt, session.recorderTime);
            });
        }
    }
}
