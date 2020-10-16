namespace UnityEditor.Recorder.FrameCapturer
{
    [RecorderSettings(typeof(EXRRecorder), "Legacy/OpenEXR")]
    class EXRRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcExrConfig m_ExrEncoderSettings = fcAPI.fcExrConfig.default_value;

        public EXRRecorderSettings()
        {
            fileNameGenerator.FileName = "image_" + DefaultWildcard.Frame;
        }

        protected internal override string Extension
        {
            get { return "exr"; }
        }
    }
}
