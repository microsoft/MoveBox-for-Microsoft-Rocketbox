using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System;
using UnityEngine.SocialPlatforms;
using Windows.Kinect;
using UnityEditor;
using UnityEditor.Recorder;
using JetBrains.Annotations;


// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.
[CustomEditor(typeof(MoveBox))]
public class MoveBoxEditor : Editor
{
 
    public int selectedAnimationMode;
    public int selectedLipsyncMode;
    public int selectedCamera;
    
    public string[] animation = new string[2];

    public string[] camera = new string[2];
    public string[] lipSync = new string[2];

    void OnEnable()
    {
        // Setup serialized property
        MoveBox mocap = (MoveBox)target;
        if (mocap.isKinect2)
            selectedCamera = 0;
        else
            selectedCamera = 1;

        if (mocap.isRealTime)
            selectedAnimationMode = 0;
        else
            selectedAnimationMode = 1;

        if (mocap.lipSyncMicOn)
            selectedLipsyncMode = 0;
        else
            selectedLipsyncMode = 1;

     

    }
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        MoveBox mocap = (MoveBox)target;

        
        animation[0] = "Real Time";
        animation[1] = "Playback";

        camera[0] = "Kinect V2";
        camera[1] = "Azure Kinect";

        lipSync[0] = "Microphone";
        lipSync[1] = "Load File";


       /* if (!mocap.startAnimating && mocap.isRuntime)
        {
            if (GUILayout.Button("Start Animating Rocketbox"))
            {
                mocap.startAnimating = true;
            }
        }*/
        
            GUILayout.Space(10f);
            GUILayout.Label("Avatar", EditorStyles.boldLabel);
            mocap.parentAvatar = (GameObject)EditorGUILayout.ObjectField("Game Object", mocap.parentAvatar, typeof(GameObject), true);


        if (mocap.isRealTime)
        {
            GUILayout.Label("Calibration (only on runtime)");
            if (mocap.isRuntime & mocap.configured)
            {
                if (GUILayout.Button("Resize"))
                {
                    mocap.resizeAvatar = true;
                }
            }
        }
        


            GUILayout.Space(20f); //2
            GUILayout.Label("Animation", EditorStyles.boldLabel); //3

            mocap.animationOn = EditorGUILayout.Toggle("ON/OFF", mocap.animationOn);



            if (mocap.animationOn)
            {
                selectedAnimationMode = EditorGUILayout.Popup(selectedAnimationMode, animation);

                if (selectedAnimationMode == 0)
                {
                    mocap.isRealTime = true;

                     //3
                    selectedCamera = EditorGUILayout.Popup("MoCap", selectedCamera, camera);

                    if (selectedCamera == 0)
                    {
                        mocap.isKinect2 = true;
                    }
                    else
                    {
                        mocap.isKinect2 = false;
                    }

                    mocap.drawSkeleton = EditorGUILayout.Toggle("Draw Skeleton", mocap.drawSkeleton);

                    GUILayout.Label("Recording  (only on runtime)"); //3

                    if (mocap.isRuntime & mocap.configured)
                    {
                        if (!mocap.isrecording)
                        {
                            if (GUILayout.Button("Start"))
                            {
                                mocap.startRecording = true;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Stop"))
                            {
                                mocap.stopRecording = true;
                            }
                        }
                    }

                }

                else
                {
                    mocap.isRealTime = false;
                    mocap.drawSkeleton = false;
                }
                if (selectedAnimationMode == 1)
                {
                    mocap.loopingOn = EditorGUILayout.Toggle("Loop animation", mocap.loopingOn);
                    mocap.animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation File", mocap.animationClip, typeof(AnimationClip), false);

                /*if (mocap.isRuntime)
                {
                    GUILayout.Label("Crop Animation", EditorStyles.boldLabel);
                    mocap.startpoint = EditorGUILayout.Slider("Start Time Normalized", mocap.startpoint, 0, 1);
                    mocap.endpoint = EditorGUILayout.Slider("End Time Normalized", mocap.endpoint, 0, 1);
                    if (GUILayout.Button("Reset"))
                    {
                        mocap.restartAnimation = true;
                    }
                }*/
            }

            }

            GUILayout.Space(20f); //2
            GUILayout.Label("LipSync", EditorStyles.boldLabel); //3
            mocap.lipSyncOn = EditorGUILayout.Toggle("ON/OFF", mocap.lipSyncOn);

            
            if (mocap.lipSyncOn )
            {
                selectedLipsyncMode = EditorGUILayout.Popup(selectedLipsyncMode, lipSync);
                if (selectedLipsyncMode==0)
                {
                    mocap.lipSyncMicOn=true;
                    if (mocap.lipSync == null)
                    {
                        mocap.nameMic = EditorGUILayout.TextField("Microphone Name", mocap.nameMic);
                    }
                    else
                    {
                        mocap.nameMic = EditorGUILayout.TextField("Microphone Name", mocap.lipSync.nameOfMicrophone);
                    }

                    GUILayout.Label("Calibration (only on runtime)");
                    if (mocap.lipSync != null & mocap.configured)
                    {
                        mocap.lipSync.VOLUME_THRESHOLD = EditorGUILayout.Slider("Threshold", mocap.lipSync.VOLUME_THRESHOLD, 0, 1);
                        mocap.lipSync.OPEN_FACTOR = EditorGUILayout.Slider("Mouth Openning", mocap.lipSync.OPEN_FACTOR, 1, 10);
                        GUILayout.Label("Current Volume: " + mocap.lipSync.currentAverageVolume);
                        GUILayout.Label("Silence Volume: " + mocap.lipSync.silenceVolume);

                        if (GUILayout.Button("Calibrate Silence"))
                        {
                            mocap.lipSync.captureSilence = true;
                        }
                    }
                }
                else
                {
                    mocap.lipSyncMicOn = false;
                    mocap.audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio File", mocap.audioClip, typeof(AudioClip), false);
                    /*if (mocap.lipSync != null)
                    {
                        if (GUILayout.Button("Re-start play"))
                        {
                            mocap.lipSync.restart = true;
                        }
                    }*/
                }


               
            }


        GUILayout.Space(20f); //2
        GUILayout.Label("Eye Blinks", EditorStyles.boldLabel); //3
        mocap.eyeBlinkOn = EditorGUILayout.Toggle("ON/OFF", mocap.eyeBlinkOn);



    }
}

