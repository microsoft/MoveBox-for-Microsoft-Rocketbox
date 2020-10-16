using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditor.Recorder;

// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.

[RequireComponent(typeof(TrackerHandler))]
[RequireComponent(typeof(ConfigLoader))]

[CustomEditor(typeof(DataProvider))]
public class DataProviderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
    }
}
public class DataProvider : MonoBehaviour
{
    // Handler for SkeletalTracking thread.
    private TrackerHandler m_tracker;
    private BackgroundDataProvider m_backgroundDataProvider;
    public BackgroundData m_lastFrameData = new BackgroundData();
    public bool started = false;

    void Start()
    {
    }

    public void StartAfterConfig() { 
        m_tracker = GetComponent<TrackerHandler>();
        const int TRACKER_ID = 0;
        if (m_tracker.isKinect2)
        {
            SkeletalTrackingProviderKinect2 m_skeletalTrackingProvider2 = new SkeletalTrackingProviderKinect2();
            m_skeletalTrackingProvider2.StartClientThread(TRACKER_ID);
            m_backgroundDataProvider = m_skeletalTrackingProvider2;
        }
        else
        {
            SkeletalTrackingProvider m_skeletalTrackingProvider = new SkeletalTrackingProvider();
            m_skeletalTrackingProvider.StartClientThread(TRACKER_ID);
            m_backgroundDataProvider = m_skeletalTrackingProvider;
        }
        //tracker ids needed for when there are two trackers

        started = true;
    }

    void Update()
    {
        if (started)
        {
            if (m_backgroundDataProvider.IsRunning)
            {
                if (m_backgroundDataProvider.GetCurrentFrameData(ref m_lastFrameData))
                {
                    if (m_lastFrameData.NumOfBodies != 0)
                    {
                        m_tracker.updateTracker(m_lastFrameData);
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        // Stop background threads.
        if (m_backgroundDataProvider != null)
        {
            m_backgroundDataProvider.StopClientThread();
        }
    }
}
