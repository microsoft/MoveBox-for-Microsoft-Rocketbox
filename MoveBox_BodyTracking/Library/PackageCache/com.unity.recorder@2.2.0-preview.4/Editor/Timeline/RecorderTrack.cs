using System;
using UnityEngine.Timeline;

namespace UnityEditor.Recorder.Timeline
{
    /// <summary>
    /// Indicates the Track type for the Recorder.
    /// </summary>
    [Serializable]
    [TrackClipType(typeof(RecorderClip))]
    [TrackColor(0f, 0.53f, 0.08f)]
    public class RecorderTrack : TrackAsset
    {
    }
}
