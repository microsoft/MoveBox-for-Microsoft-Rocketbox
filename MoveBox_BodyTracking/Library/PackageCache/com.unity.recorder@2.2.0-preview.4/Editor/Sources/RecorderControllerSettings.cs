using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Use this class to manage the recording settings (frame rate, frame range, list of Recorder Settings) for the <see cref="RecorderController"/>.
    /// </summary>
    public class RecorderControllerSettings : ScriptableObject
    {
        [SerializeField] RecordMode m_RecordMode = RecordMode.Manual;
        [SerializeField] FrameRatePlayback m_FrameRatePlayback = FrameRatePlayback.Constant;
        [SerializeField] FrameRateType m_FrameRateType = FrameRateType.FR_30;
        [SerializeField] [Range(1.0f, 120.0f)] float m_CustomFrameRateValue = 30.0f;

        [SerializeField] int m_StartFrame;
        [SerializeField] int m_EndFrame;

        [SerializeField] float m_StartTime;
        [SerializeField] float m_EndTime;

        [SerializeField] bool m_CapFrameRate = true;

        static readonly Dictionary<FrameRateType, float> s_FPSToValue = new Dictionary<FrameRateType, float>()
        {
            { FrameRateType.FR_23, 24 * 1000 / 1001f },
            { FrameRateType.FR_24, 24 },
            { FrameRateType.FR_25, 25 },
            { FrameRateType.FR_29, 30 * 1000 / 1001f },
            { FrameRateType.FR_30, 30 },
            { FrameRateType.FR_50, 50 },
            { FrameRateType.FR_59, 60 * 1000 / 1001f },
            { FrameRateType.FR_60, 60 }
        };

        /// <summary>
        /// Indicates the type of frame rate (constant or variable) for the current list of Recorders.
        /// </summary>
        public FrameRatePlayback FrameRatePlayback
        {
            get { return m_FrameRatePlayback; }
            set { m_FrameRatePlayback = value; }
        }

        /// <summary>
        /// Allows setting and retrieving the frame rate for the current list of Recorders.
        /// </summary>
        public float FrameRate
        {
            get
            {
                return m_FrameRateType == FrameRateType.FR_CUSTOM ? m_CustomFrameRateValue : s_FPSToValue[m_FrameRateType];
            }

            set
            {
                m_FrameRateType = FrameRateType.FR_CUSTOM;
                m_CustomFrameRateValue = value;
            }
        }

        /// <summary>
        /// Sets the Recorders to Manual mode.
        /// </summary>
        public void SetRecordModeToManual()
        {
            m_RecordMode = RecordMode.Manual;
        }

        /// <summary>
        /// Sets the Recorders to Single Frame recording mode.
        /// </summary>
        /// <param name="frameNumber">The frame to be recorded.</param>
        public void SetRecordModeToSingleFrame(int frameNumber)
        {
            m_RecordMode = RecordMode.SingleFrame;
            m_StartFrame = m_EndFrame = frameNumber;
        }

        /// <summary>
        /// Sets the Recorders to Frame Interval mode and defines the Start and End frame of the interval to record.
        /// </summary>
        /// <param name="startFrame">Start frame.</param>
        /// <param name="endFrame">End frame.</param>
        public void SetRecordModeToFrameInterval(int startFrame, int endFrame)
        {
            m_RecordMode = RecordMode.FrameInterval;
            m_StartFrame = startFrame;
            m_EndFrame = endFrame;
        }

        /// <summary>
        /// Sets the Recorders to Time Interval mode and defines the Start and End times of the interval to record.
        /// </summary>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        public void SetRecordModeToTimeInterval(float startTime, float endTime)
        {
            m_RecordMode = RecordMode.TimeInterval;
            m_StartTime = startTime;
            m_EndTime = endTime;
        }

        /// <summary>
        /// Indicates if the Recorders frame rate should cap the Unity rendering frame rate. When enabled, Unity is prevented from rendering faster than the set FrameRate.
        /// </summary>
        public bool CapFrameRate
        {
            get { return m_CapFrameRate; }
            set { m_CapFrameRate = value; }
        }

        [SerializeField] List<RecorderSettings> m_RecorderSettings = new List<RecorderSettings>();

        string m_Path;

        /// <summary>
        /// Loads or creates Recorder Settings to the specified file path.
        /// </summary>
        /// <param name="path">The path to load or create Recorder Settings.</param>
        /// <returns>The loaded or created Recorder Settings.</returns>
        public static RecorderControllerSettings LoadOrCreate(string path)
        {
            RecorderControllerSettings prefs;
            try
            {
                var objs = InternalEditorUtility.LoadSerializedFileAndForget(path);
                prefs = objs.FirstOrDefault(p => p is RecorderControllerSettings) as RecorderControllerSettings;
            }
            catch (Exception e)
            {
                Debug.LogError("Unhandled exception while loading Recorder preferences: " + e);
                prefs = null;
            }

            if (prefs == null)
            {
                prefs = CreateInstance<RecorderControllerSettings>();
                prefs.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                prefs.name = "Global Settings";
                prefs.Save();
            }

            prefs.m_Path = path;

            return prefs;
        }

        internal void ReleaseRecorderSettings()
        {
            foreach (var recorder in m_RecorderSettings)
            {
                DestroyImmediate(recorder);
            }

            ClearRecorderSettings();
        }

        internal void ClearRecorderSettings()
        {
            m_RecorderSettings.Clear();
        }

        /// <summary>
        /// Stores the collection of Recorder Settings instances.
        /// </summary>
        public IEnumerable<RecorderSettings> RecorderSettings
        {
            get { return m_RecorderSettings; }
        }

        /// <summary>
        /// Adds a new instance of Recorder Settings to the current collection.
        /// </summary>
        /// <param name="recorder">The Recorder Settings instance to add.</param>
        public void AddRecorderSettings(RecorderSettings recorder)
        {
            if (!m_RecorderSettings.Contains(recorder))
            {
                AddRecorderInternal(recorder);
                Save();
            }
        }

        /// <summary>
        /// Removes an instance of Recorder Settings from the current collection.
        /// </summary>
        /// <param name="recorder">The Recorder settings instance to be removed.</param>
        public void RemoveRecorder(RecorderSettings recorder)
        {
            if (m_RecorderSettings.Contains(recorder))
            {
                m_RecorderSettings.Remove(recorder);
                Save();
            }
        }

        /// <summary>
        /// Saves the current list of Recorder Settings instances to disk.
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(m_Path))
                return;

            try
            {
                var directory = Path.GetDirectoryName(m_Path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var recordersCopy = RecorderSettings.ToArray();

                var objs = new UnityObject[recordersCopy.Length + 1];
                objs[0] = this;

                for (int i = 0; i < recordersCopy.Length; ++i)
                    objs[i + 1] = recordersCopy[i];

                InternalEditorUtility.SaveToSerializedFileAndForget(objs, m_Path, true);
            }
            catch (Exception e)
            {
                Debug.LogError("Unhandled exception while saving Recorder settings: " + e);
            }
        }

        internal void ApplyGlobalSetting(RecorderSettings recorder)
        {
            recorder.RecordMode = m_RecordMode;
            recorder.FrameRatePlayback = m_FrameRatePlayback;
            recorder.FrameRate = FrameRate;
            recorder.StartFrame = m_StartFrame;
            recorder.EndFrame = m_EndFrame;
            recorder.StartTime = m_StartTime;
            recorder.EndTime = m_EndTime;
            recorder.CapFrameRate = m_CapFrameRate;
            recorder.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;

            recorder.SelfAdjustSettings();
        }

        internal void ApplyGlobalSettingToAllRecorders()
        {
            foreach (var recorder in RecorderSettings)
                ApplyGlobalSetting(recorder);
        }

        void AddRecorderInternal(RecorderSettings recorder)
        {
            ApplyGlobalSetting(recorder);
            m_RecorderSettings.Add(recorder);
        }
    }
}
