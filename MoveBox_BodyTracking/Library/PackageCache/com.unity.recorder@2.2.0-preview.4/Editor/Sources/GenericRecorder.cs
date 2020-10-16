using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Base class for all Recorders.
    /// </summary>
    /// <typeparam name="T">The Recorder settings type associated with the Recorder.</typeparam>
    public abstract class GenericRecorder<T> : Recorder where T : RecorderSettings
    {
        [SerializeField] T m_Settings;

        internal override RecorderSettings settings
        {
            get { return m_Settings; }
            set { m_Settings = (T)value; }
        }

        /// <summary>
        /// Indicates the Recorder settings for the current Recorder.
        /// </summary>
        public T Settings
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }
    }
}
