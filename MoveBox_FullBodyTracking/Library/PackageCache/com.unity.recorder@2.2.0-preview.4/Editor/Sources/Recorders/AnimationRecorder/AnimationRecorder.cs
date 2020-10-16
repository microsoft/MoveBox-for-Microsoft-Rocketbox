using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class AnimationRecorder : GenericRecorder<AnimationRecorderSettings>
    {
        protected internal override void RecordFrame(RecordingSession session)
        {
        }

        protected internal override void EndRecording(RecordingSession session)
        {
            var ars = (AnimationRecorderSettings)session.settings;

            foreach (var input in m_Inputs)
            {
                var aInput = (AnimationInput)input;

                if (aInput.GameObjectRecorder == null)
                    continue;
                
                var clip = new AnimationClip();
                
                ars.fileNameGenerator.CreateDirectory(session);

                var absolutePath = FileNameGenerator.SanitizePath(ars.fileNameGenerator.BuildAbsolutePath(session));
                var clipName = absolutePath.Replace(FileNameGenerator.SanitizePath(Application.dataPath), "Assets");
                
                AssetDatabase.CreateAsset(clip, clipName);
    #if UNITY_2018_3_OR_NEWER
                aInput.GameObjectRecorder.SaveToClip(clip, ars.FrameRate);
    #else
                aInput.gameObjectRecorder.SaveToClip(clip);
    #endif
                aInput.GameObjectRecorder.ResetRecording();
            }

            base.EndRecording(session);
        }
    }
}