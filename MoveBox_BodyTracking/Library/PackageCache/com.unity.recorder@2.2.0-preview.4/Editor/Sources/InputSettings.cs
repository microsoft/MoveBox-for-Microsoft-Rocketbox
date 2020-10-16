using System;
using System.Collections.Generic;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Base class that represents a RecorderSetting Input that can be recorded from. (like a Camera, a RenderTexture...) 
    /// </summary>
    [Serializable]
    public abstract class RecorderInputSettings
    {
        protected internal abstract Type InputType { get; }
        protected internal abstract bool ValidityCheck(List<string> errors);
    }
}
