using System;
using UnityEngine;

namespace UnityEditor.Recorder.FrameCapturer
{   
    class WEBMRecorder : GenericRecorder<WEBMRecorderSettings>
    {
        fcAPI.fcWebMContext m_ctx;
        fcAPI.fcStream m_stream;
        
        protected internal override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            Settings.fileNameGenerator.CreateDirectory(session);

            return true;
        } 

        protected internal override void EndRecording(RecordingSession session)
        {
            m_ctx.Release();
            m_stream.Release();
            base.EndRecording(session);
        }

        protected internal override void RecordFrame(RecordingSession session)
        {
            if (m_Inputs.Count != 1)
                throw new Exception("Unsupported number of sources");

            var input = (BaseRenderTextureInput)m_Inputs[0];
            var frame = input.OutputRenderTexture;

            if (!m_ctx)
            {
                var webmSettings = Settings.m_WebmEncoderSettings;
                webmSettings.video = true;
                webmSettings.audio = false;
                webmSettings.videoWidth = frame.width;
                webmSettings.videoHeight = frame.height;
                if (Settings.m_AutoSelectBR)
                {
                    webmSettings.videoTargetBitrate = (int)(( (frame.width * frame.height/1000.0) / 245 + 1.16) * (webmSettings.videoTargetFramerate / 48.0 + 0.5) * 1000000);
                }

                webmSettings.videoTargetFramerate = (int)Math.Ceiling(Settings.FrameRate);
                m_ctx = fcAPI.fcWebMCreateContext(ref webmSettings);
                var path = Settings.fileNameGenerator.BuildAbsolutePath(session);
                m_stream = fcAPI.fcCreateFileStream(path);
                fcAPI.fcWebMAddOutputStream(m_ctx, m_stream);
            }

            fcAPI.fcLock(frame, TextureFormat.RGB24, (data, fmt) =>
            {
                fcAPI.fcWebMAddVideoFramePixels(m_ctx, data, fmt, session.recorderTime);
            });
        }

    }
}
