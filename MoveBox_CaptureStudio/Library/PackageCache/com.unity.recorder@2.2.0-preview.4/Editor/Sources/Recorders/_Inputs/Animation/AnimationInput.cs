#if UNITY_2018_3_OR_NEWER
using UnityEditor.Animations;
#else
using UnityEditor.Experimental.Animations;
#endif

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Use this class to record animations in the scene in the Unity native format.
    /// </summary>
    public class AnimationInput : RecorderInput
    {
        /// <summary>
        /// Indicates the internal GameObject Recorder to use for the capture.
        /// </summary>
        public GameObjectRecorder GameObjectRecorder { get; private set; }
        float m_Time;

        /// <inheritdoc/>
        protected internal override void BeginRecording(RecordingSession session)
        {
            var aniSettings = (AnimationInputSettings) settings;

            var srcGO = aniSettings.gameObject;

            if (srcGO == null)
                throw new System.NullReferenceException("srcGO");

            GameObjectRecorder = new GameObjectRecorder(srcGO);

            foreach (var binding in aniSettings.bindingType)
            {
                GameObjectRecorder.BindComponentsOfType(srcGO, binding, aniSettings.Recursive);
            }

            m_Time = session.recorderTime;
        }

        /// <inheritdoc/>
        protected internal override void NewFrameReady(RecordingSession session)
        {
            if (GameObjectRecorder != null && session.isRecording)
            {
                GameObjectRecorder.TakeSnapshot(session.recorderTime - m_Time);
                m_Time = session.recorderTime;
            }
        }
    }
}
