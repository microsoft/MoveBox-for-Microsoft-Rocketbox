using System;

namespace UnityEditor.Recorder.FrameCapturer
{
    class PNGRecorder : GenericRecorder<PNGRecorderSettings>
    {
        fcAPI.fcPngContext m_ctx;
        
        protected internal override bool BeginRecording(RecordingSession session)
        {
            if (!base.BeginRecording(session)) { return false; }

            Settings.fileNameGenerator.CreateDirectory(session);

            m_ctx = fcAPI.fcPngCreateContext(ref Settings.m_PngEncoderSettings);
            return m_ctx;
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
            var path = Settings.fileNameGenerator.BuildAbsolutePath(session);

            fcAPI.fcLock(frame, (data, fmt) =>
            {
                fcAPI.fcPngExportPixels(m_ctx, path, data, frame.width, frame.height, fmt, 0);
            });
        }

    }
}
