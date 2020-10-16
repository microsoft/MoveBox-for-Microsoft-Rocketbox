using System;

namespace UnityEditor.Recorder.Input
{
    class RenderTextureInput : BaseRenderTextureInput
    {
        TextureFlipper m_VFlipper;
        
        RenderTextureInputSettings cbSettings
        {
            get { return (RenderTextureInputSettings)settings; }
        }

        protected internal override void BeginRecording(RecordingSession session)
        {
            if (cbSettings.renderTexture == null)
                throw new Exception("No Render Texture object provided as source");
            
            OutputHeight = cbSettings.OutputHeight;
            OutputWidth = cbSettings.OutputWidth;
            
            OutputRenderTexture = cbSettings.renderTexture;
            
            if (cbSettings.FlipFinalOutput)
                m_VFlipper = new TextureFlipper();
        }

        protected internal override void NewFrameReady(RecordingSession session)
        {
            if (cbSettings.FlipFinalOutput)
                m_VFlipper.Flip(OutputRenderTexture);
            
            base.NewFrameReady(session);
        }
        
    }
}