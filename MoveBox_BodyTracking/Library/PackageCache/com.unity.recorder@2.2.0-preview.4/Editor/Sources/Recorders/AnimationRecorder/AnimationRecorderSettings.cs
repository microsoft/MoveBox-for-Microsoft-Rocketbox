using System;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Class describing the settings for Animation Recorder.
    /// </summary>
    [Serializable]
    [RecorderSettings(typeof(AnimationRecorder), "Animation Clip", "animation_recorder")]
    public class AnimationRecorderSettings : RecorderSettings
    {
        [SerializeField] AnimationInputSettings m_AnimationInputSettings = new AnimationInputSettings();

        /// <summary>
        /// Stores the reference to the current Animation Recorder input settings.
        /// </summary>
        public AnimationInputSettings AnimationInputSettings
        {
            get { return m_AnimationInputSettings; }
            set { m_AnimationInputSettings = value; }
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public AnimationRecorderSettings()
        {
            var goWildcard = DefaultWildcard.GeneratePattern("GameObject");

            fileNameGenerator.AddWildcard(goWildcard, GameObjectNameResolver);
            fileNameGenerator.AddWildcard(DefaultWildcard.GeneratePattern("GameObjectScene"), GameObjectSceneNameResolver);

            fileNameGenerator.ForceAssetsFolder = true;
            fileNameGenerator.Root = OutputPath.Root.AssetsFolder;
            fileNameGenerator.FileName = "animation_" + goWildcard + "_" + DefaultWildcard.Take;
        }

        string GameObjectNameResolver(RecordingSession session)
        {
            var go = m_AnimationInputSettings.gameObject;
            return go != null ? go.name : "None";
        }

        string GameObjectSceneNameResolver(RecordingSession session)
        {
            var go = m_AnimationInputSettings.gameObject;
            return go != null ? go.scene.name : "None";
        }

        /// <inheritdoc/>
        /// <remarks>
        ///  Animation Recorder currently supports the following platforms: LinuxEditor, OSXEditor, WindowsEditor.
        /// </remarks>
        public override bool IsPlatformSupported
        {
            get
            {
                return Application.platform == RuntimePlatform.LinuxEditor ||
                       Application.platform == RuntimePlatform.OSXEditor ||
                       Application.platform == RuntimePlatform.WindowsEditor;
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return m_AnimationInputSettings; }
        }

        /// <inheritdoc/>
        protected internal override string Extension
        {
            get { return "anim"; }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = base.ValidityCheck(errors);

            if (m_AnimationInputSettings.gameObject == null)
            {
                ok = false;
                errors.Add("No input object set");
            }

            return ok;
        }

        /// <inheritdoc/>
        public override void OnAfterDuplicate()
        {
            m_AnimationInputSettings.DuplicateExposedReference();
        }

        void OnDestroy()
        {
            m_AnimationInputSettings.ClearExposedReference();
        }
    }
}
