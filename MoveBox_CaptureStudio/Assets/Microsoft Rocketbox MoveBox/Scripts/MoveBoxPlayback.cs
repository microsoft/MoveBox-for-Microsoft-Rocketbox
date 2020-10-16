using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEditor;




[CustomEditor(typeof(MoveBoxPlayback))]
public class MoveBoxPlaybackEditor : Editor
{

    
    void OnEnable()
    {


    }
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        MoveBoxPlayback mocap = (MoveBoxPlayback)target;


        if (!mocap.enabled) return;

        GUILayout.Space(20f); //2
        GUILayout.Label("Animation", EditorStyles.boldLabel); //3

        if (mocap.standalone)
            mocap.animationOn = EditorGUILayout.Toggle("ON/OFF", mocap.animationOn);

        if (mocap.animationOn)
        {
            mocap.loopingOn = EditorGUILayout.Toggle("Loop animation", mocap.loopingOn);
            mocap.animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation File", mocap.animationClip, typeof(AnimationClip), false);

            if (mocap.isRunTime)
            {
                GUILayout.Label("Current Time: " + mocap.currentTime, EditorStyles.boldLabel);
                GUILayout.Label("Crop Animation", EditorStyles.boldLabel);
                mocap.startpoint = EditorGUILayout.Slider("Start Time Normalized", mocap.startpoint, 0, 1);
                mocap.endpoint = EditorGUILayout.Slider("End Time Normalized", mocap.endpoint, 0, 1);
                if (GUILayout.Button("Reset"))
                {
                    mocap.restartAnimation = true;
                }

                //additions for smoothloop:
                GUILayout.Label("Auto-Crop Animation", EditorStyles.boldLabel);
                mocap.searchWholeClip = EditorGUILayout.Toggle("Search Whole Clip", mocap.searchWholeClip);
                GUILayout.Label("When Search Whole Clip is off, Auto-Crop will trim between Start Time and End Time", EditorStyles.miniLabel);
                mocap.trimMarginSeconds = EditorGUILayout.Slider("Trimming Window", mocap.trimMarginSeconds, 0, mocap.animationClip.length / 2);
                GUILayout.Label("Trimming Window is how many seconds can be lopped off each end", EditorStyles.miniLabel);
                if (GUILayout.Button("Trim for Smoothest Loop"))
                {
                    mocap.findBestLoopTrim = true;
                }
            }
        }

        GUILayout.Space(20f); //2
            GUILayout.Label("LipSync", EditorStyles.boldLabel); //3
        if(mocap.standalone)
            mocap.lipsyncOn = EditorGUILayout.Toggle("ON/OFF", mocap.lipsyncOn);


        if (mocap.lipsyncOn)
        {
  
            mocap.audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio File", mocap.audioClip, typeof(AudioClip), false);
 
            GUILayout.Label("Calibration (only on runtime)");
            if (mocap.lipsync != null & mocap.isRunTime)
            {

                if (GUILayout.Button("Re-start play"))
                {
                    mocap.lipsync.restart = true;
                }

                mocap.lipsync.VOLUME_THRESHOLD = EditorGUILayout.Slider("Threshold", mocap.lipsync.VOLUME_THRESHOLD, 0, 1);
                mocap.lipsync.OPEN_FACTOR = EditorGUILayout.Slider("Mouth Openning", mocap.lipsync.OPEN_FACTOR, 1, 10);
                GUILayout.Label("Current Volume: " + mocap.lipsync.currentAverageVolume);
                GUILayout.Label("Silence Volume: " + mocap.lipsync.silenceVolume);

                if (GUILayout.Button("Calibrate Silence"))
                {
                    mocap.lipsync.captureSilence = true;
                }
            }
        }
        

        GUILayout.Space(20f); //2
        GUILayout.Label("Eye Blinks", EditorStyles.boldLabel); //3
        if (mocap.standalone)
            mocap.eyeBlinkOn = EditorGUILayout.Toggle("ON/OFF", mocap.eyeBlinkOn);




    }
}


public class MoveBoxPlayback : MonoBehaviour
{

    public EyeBlink eyeBlinks;
    public bool eyeBlinkOn = true, restartAnimation = false, standalone=true;
    public bool  resizeAvatar = false, lipsyncOn=true, animationOn = true, loopingOn = false, isRunTime=false;
    public float currentTime=0;
    public LipSync lipsync;
    private float rocketboxHeight;
    public GameObject ParentAvatar, RootPosition;

    public Vector3 Offset;
    public float startpoint, endpoint = 1;
    public AnimationClip animationClip;
    public AudioClip audioClip;
    private Animator a;
    float scale;

    //additions for smoothloop:
    public bool findBestLoopTrim = false, searchWholeClip = true; // a trigger button that sets startpoint and endpoint, not an on off toggle
    public float trimMarginSeconds; //for the loop-smoothing triggered by the above function
    private OrderedDictionary[] animClipJointRotations;

    // Noise reduction

    [Space]

    [Range(0.0f, 1.0f)]
    public float _exponentialSmooth = 0.75f;

    public bool _NoWristRotation = false;
    // Start is called before the first frame update
    void Start()
    {
        isRunTime = true;
        if(standalone)
            StartAfterConfig();
    }

    public void StartEyeBlinks()
    {
        if (eyeBlinkOn)
        {
            
            eyeBlinks = ParentAvatar.AddComponent<EyeBlink>();
            
            eyeBlinks.StartAfterConfig(RootPosition);
        }
    }

    public void StartLipSync()
    {
        if (lipsyncOn)
        {
            
            lipsync = ParentAvatar.AddComponent<LipSync>();

            lipsync.realtimeMoCap = false;
            lipsync.StartAfterConfig(audioClip, RootPosition);

            // lipsync.avatar_scale = ParentAvatar.transform.localScale.x;
        }

    }

    public void StartAnimationPlayback()
    {
        if (animationClip != null)
        {
            Offset = RootPosition.transform.position;
            if (!ParentAvatar.TryGetComponent<Animator>(out a))
                a = ParentAvatar.AddComponent<Animator>();

            endpoint = animationClip.length;
            a.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator Controller");
            Debug.Log(animationClip.name);
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(a.runtimeAnimatorController);

            animatorOverrideController.name = "Runtime Animation " + animationClip.name;
            a.runtimeAnimatorController = animatorOverrideController;

            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            animatorOverrideController.GetOverrides(overrides);
            Debug.Log(overrides[0]);

            AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);


            if (loopingOn)
            {
                animationClipSettings.loopTime = true;

            }
            else
            {
                animationClip.wrapMode = WrapMode.Once;
            }
            AnimationUtility.SetAnimationClipSettings(animationClip, animationClipSettings);
            overrides[0] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[0].Key, animationClip);
            Debug.Log(overrides[0]);
            animatorOverrideController.ApplyOverrides(overrides);

            //additions for smoothloop:
            SetAnimClipRotations();
            trimMarginSeconds = (int)animationClip.length / 2;
        }

    }
    public void StartAfterConfig()
    {

        ParentAvatar = this.gameObject;

        // START AVATAR
        RootPosition = ParentAvatar.transform.Find("Bip01").Find("Bip01 Pelvis").gameObject;
       /* GameObject rockethead = MoveBox.GetRocketboxBone(JointId.Head, RootPosition);
        GameObject rocketfoot = MoveBox.GetRocketboxBone(JointId.FootLeft, RootPosition);
        rocketboxHeight = rockethead.transform.position.y - rocketfoot.transform.position.y;
       */
        

        //START EYEBLINK
        StartEyeBlinks();

        // START LIPSYNC
        StartLipSync();

        // ANIMATION PLAYBACK
        StartAnimationPlayback();
    }
  

    // Update is called once per frame
    void Update()
    {
        
        if (animationOn)
        {
            currentTime = a.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (a.GetCurrentAnimatorStateInfo(0).normalizedTime >= endpoint)
                restartAnimation = true;

            if (restartAnimation)
            {
                restartAnimation = false;
                a.StopPlayback();
                a.Play("RB_animation", 0, startpoint);
                //            a.Rebind();

            }

            //additions for smoothloop:
            if (findBestLoopTrim)
            {
                findBestLoopTrim = false;
                SetBestLoopTrim();
                restartAnimation = true;
            }
        }
    }
    void LateUpdate()
    {
        RootPosition.transform.position += Offset;

    }
    //additions for smoothloop: PopulateDictionary is called in SetAnimClipRotations();
    private void PopulateDictionary(GameObject currentBone, int frame)
    {
        animClipJointRotations[frame].Add(currentBone.name, currentBone.transform.rotation);

        for (int i = 0; i < currentBone.transform.childCount; i++)
        {
            PopulateDictionary(currentBone.transform.GetChild(i).gameObject, frame);
        }
    }


    //additions for smoothloop: SetRecordedRotations() called at the Start
    public void SetAnimClipRotations()
    {
        int numFrames = (int)(animationClip.length * animationClip.frameRate);
        animClipJointRotations = new OrderedDictionary[numFrames];
        for (int f = 0; f < numFrames; f++) //animationClip.length is Length of the clip in seconds
        {
            animationClip.SampleAnimation(ParentAvatar, f / animationClip.frameRate);
            animClipJointRotations[f] = new OrderedDictionary();
            PopulateDictionary(ParentAvatar, f);
        }
    }
    //additions for smoothloop: SetBestLoopTrim() called in Update if findBestLoopTrim
    public void SetBestLoopTrim()
    {
        Debug.Log("SetBestLoopTrim()");

        int allFrames = animClipJointRotations.Length;
        int totalFramesSearched;
        int firstFrame;
        int lastFrame;
        if (searchWholeClip)
        {
            totalFramesSearched = allFrames;
            firstFrame = 0;
            lastFrame = allFrames - 1;
        }
        else
        {
            totalFramesSearched = (int)((endpoint - startpoint) * allFrames);
            firstFrame = (int)(startpoint * allFrames);
            lastFrame = firstFrame + totalFramesSearched - 1;
        }
        int trimFrames = (int)(trimMarginSeconds * animationClip.frameRate);
        if (trimFrames * 2 >= totalFramesSearched)
        {
            trimFrames = totalFramesSearched / 2;
        }

        Quaternion[][] frontRotationsSeries = new Quaternion[trimFrames][], backRotationsSeries = new Quaternion[trimFrames][];
        for (int f = 0; f < trimFrames; f++)
        {
            frontRotationsSeries[f] = new Quaternion[animClipJointRotations[0].Count];
            backRotationsSeries[trimFrames - f - 1] = new Quaternion[animClipJointRotations[0].Count];
            animClipJointRotations[firstFrame + f].Values.CopyTo(frontRotationsSeries[f], 0);
            //Debug.Log(animClipJointRotations[firstFrame + f][0].x + " " )
            animClipJointRotations[lastFrame - f].Values.CopyTo(backRotationsSeries[trimFrames - f - 1], 0);
        }
        int fBestFrameID = 0, bBestFrameID = 0;
        float bestSimilarity = float.PositiveInfinity;
        for (int f = 0; f < frontRotationsSeries.Length; f++)
        {
            for (int b = 0; b < backRotationsSeries.Length; b++)
            {
                float similarity_index = 0;
                for (int i = 0; i < (int)JointId.Count; i++)
                {
                    //  Debug.Log(f +" "+ frontRotationsSeries.Length + " " + +" " + +" " + +" " +)
                    Quaternion q_similarity = frontRotationsSeries[f][i] * Quaternion.Inverse(backRotationsSeries[backRotationsSeries.Length - b - 1][i]);

                    similarity_index += (float)Math.Sqrt(q_similarity.x * q_similarity.x +
                        q_similarity.y * q_similarity.y + q_similarity.z * q_similarity.z);
                }
                if (similarity_index < bestSimilarity)
                {
                    bestSimilarity = similarity_index;
                    fBestFrameID = firstFrame + f;
                    bBestFrameID = lastFrame - b;
                }
            }
        }
        //int totalFrames = animClipJointRotations.Length;
        //startpoint and endpoint are in terms of normalized time (they are the percent of the way through the clip from [0,1]
        startpoint = (float)fBestFrameID / (float)allFrames;
        endpoint = (float)bBestFrameID / (float)allFrames;


        Debug.Log("start: " + startpoint + " end: " + endpoint);

    }

}
