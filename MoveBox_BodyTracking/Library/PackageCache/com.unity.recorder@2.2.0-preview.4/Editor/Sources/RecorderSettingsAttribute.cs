using System;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// This attribute allows binding a Recorder Settings instance with a Recorder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RecorderSettingsAttribute : Attribute
    {
        internal readonly Type recorderType;
        internal readonly string displayName;
        internal readonly string iconName;

        /// <summary>
        /// Constructor for the attribute.
        /// </summary>
        /// <param name="recorderType">The type of Recorder that uses this Recorder Settings instance.</param>
        /// <param name="displayName">The Recorder's display name.</param>
        public RecorderSettingsAttribute(Type recorderType, string displayName)
        {
            this.recorderType = recorderType;
            this.displayName = displayName;
        }

        public RecorderSettingsAttribute(Type recorderType, string displayName, string iconName)
        {
            this.iconName = iconName;
            this.recorderType = recorderType;
            this.displayName = displayName;
        }
    }
}
