using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

namespace UnityEditor.Recorder
{   
    /// <summary>
    /// Class that models a Recorder List (a stack of preset Recorder Settings instances) that you can save and load for reuse into a Recorder Window.
    /// </summary>
    public class RecorderControllerSettingsPreset : ScriptableObject
    {
        [SerializeField] Preset m_Model;
        [SerializeField] List<Preset> m_RecorderPresets = new List<Preset>();
        
        internal Preset model
        {
            get { return m_Model; }
        }
        
        internal Preset[] recorderPresets
        {
            get { return m_RecorderPresets.ToArray(); }
        }

        /// <summary>
        /// Saves the specified Recorder List to a file on disk. Note that this method doesn't save Scene references (such as a GameObject reference in Animation Recorder Settings).
        /// </summary>
        /// <param name="model">The Recorder List to save.</param>
        /// <param name="path">The path on disk where to save the Recorder List. You must specify a path relative to the project.</param>
        public static void SaveAtPath(RecorderControllerSettings model, string path)
        {
            var data = CreateInstance<RecorderControllerSettingsPreset>();
            
            var copy = Instantiate(model);
            copy.name = model.name;
            
            // TODO Remove this once there's an official way to exclude some field from being save into presets
            copy.ClearRecorderSettings(); // Do not save asset references in the preset. 

            var p = new Preset(copy) { name = model.name };
            data.m_Model = p;
            
            foreach (var recorder in model.RecorderSettings)
            {
                var rp = new Preset(recorder) { name = recorder.name };
                data.m_RecorderPresets.Add(rp);
            }
            
            //var preset = new Preset(data);
            //AssetDatabase.CreateAsset(preset, "Assets/test.preset");

            var preset = data; //new Preset(data);
            AssetDatabase.CreateAsset(preset, path); //AssetDatabase.CreateAsset(preset, "Assets/test.preset");
            
            foreach (var rp in data.m_RecorderPresets)
                AddHiddenObjectToAsset(rp, preset);
            
            AddHiddenObjectToAsset(p, preset);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
                
        /// <summary>
        /// Applies the current Recorder List to the specified RecorderControllerSettings instance.
        /// </summary>
        /// <param name="prefs">The RecorderControllerSettings instance to apply the Recorder List to.</param>
        public void ApplyTo(RecorderControllerSettings prefs)
        {
            prefs.ReleaseRecorderSettings();
            
            m_Model.ApplyTo(prefs);
            
            foreach (var rp in m_RecorderPresets)
            {
                var r = (RecorderSettings) CreateFromPreset(rp);
                r.name = rp.name;
                prefs.AddRecorderSettings(r);
            }
            
            prefs.Save();
        }

        static ScriptableObject CreateFromPreset(Preset preset)
        {
            var instance = CreateInstance(preset.GetTargetFullTypeName());
            preset.ApplyTo(instance);
            
            return instance;
        }
        
        static void AddHiddenObjectToAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject)
        {
            objectToAdd.hideFlags |= HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(objectToAdd, assetObject);
        }
    }
}