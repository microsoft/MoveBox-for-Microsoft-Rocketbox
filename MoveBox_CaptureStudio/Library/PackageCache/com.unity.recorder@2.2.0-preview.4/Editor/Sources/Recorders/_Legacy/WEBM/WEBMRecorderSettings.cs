namespace UnityEditor.Recorder.FrameCapturer
{
    [RecorderSettings(typeof(WEBMRecorder), "Legacy/WebM" )]
    class WEBMRecorderSettings : BaseFCRecorderSettings
    {
        public fcAPI.fcWebMConfig m_WebmEncoderSettings = fcAPI.fcWebMConfig.default_value;
        public bool m_AutoSelectBR;

        public WEBMRecorderSettings()
        {
            fileNameGenerator.FileName = "movie";
            m_AutoSelectBR = true;
        }

        protected internal override string Extension
        {
            get { return "webm"; }
        }
    }
}
