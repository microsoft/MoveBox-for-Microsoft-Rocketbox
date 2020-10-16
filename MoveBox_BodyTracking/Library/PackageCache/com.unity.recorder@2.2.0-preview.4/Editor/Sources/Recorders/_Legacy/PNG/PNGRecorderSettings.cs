namespace UnityEditor.Recorder.FrameCapturer
{
    [RecorderSettings(typeof(PNGRecorder), "Legacy/PNG" )]
    class PNGRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcPngConfig m_PngEncoderSettings = fcAPI.fcPngConfig.default_value;

        public PNGRecorderSettings()
        {
            fileNameGenerator.FileName = "image_" + DefaultWildcard.Frame;
        }

        protected internal override string Extension
        {
            get { return "png"; }
        }
    }
}
