using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    
    enum AudioRecorderOutputFormat
    {
       // MP3,
        WAV
    }
    
    [RecorderSettings(typeof(AudioRecorder), "Audio")]
    class AudioRecorderSettings : RecorderSettings
    {
        public AudioRecorderOutputFormat  outputFormat = AudioRecorderOutputFormat.WAV;
        
        [SerializeField] AudioInputSettings m_AudioInputSettings = new AudioInputSettings();

        protected internal override string Extension
        {
            get { return outputFormat.ToString().ToLower(); }
        }

        public AudioInputSettings audioInputSettings
        {
            get { return m_AudioInputSettings; }
        }
        
        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return m_AudioInputSettings; }
        }


        public AudioRecorderSettings()
        {
            fileNameGenerator.FileName = "mixdown";
        }
    }
}