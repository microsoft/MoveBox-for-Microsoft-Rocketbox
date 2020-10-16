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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


// Creates a custom Label on the inspector for all the scripts named ScriptName
// Make sure you have a ScriptName script in your
// project, else this will not work.




public class MoveBox : MonoBehaviour
{

    public RecorderController recorder, audiorecorder;
    public TrackerHandler KinectDevice;
    public MoveBoxPlayback PlayBack;
    public EyeBlink eyeBlinks;
    public DataProvider dp;
    public bool isKinect2, eyeBlinkOn = true, DrawSkeleton = false, restartAnimation=false, correct_roll_on=true;
    public bool startRecording = false, stopRecording = false, resizeAvatar = false, isrecording = false, isruntime, lipsyncOn, animationOn = false, isRealTime = false, configOn = false, configured = false, loopingOn = false, pingpongOn = false, lipsyncMicOn=true;
    public LipSync lipsync;
    private float rocketboxHeight;
    Dictionary<string, Quaternion> originalAvatarAbsoluteRotations;
    public GameObject ParentAvatar;
    public GameObject RootPosition;

    string filepath="";
    public Vector3 Offset;
    public string nameMic;
    public float startpoint,endpoint=1;
    public AnimationClip animationClip;
    public AudioClip audioClip;
    public Animator a;
    float scale;

    // Noise reduction

    [Space]

    [Range(0.0f, 1.0f)]
    public float _exponentialSmooth = 0.75f;

    public bool _NoWristRotation = false;

    public static GameObject GetRocketboxBone(Microsoft.Azure.Kinect.BodyTracking.JointId joinId, GameObject RootPosition)
    {
        switch (joinId)
        {
            case Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis: return RootPosition;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel: return RootPosition.transform.Find("Bip01 Spine").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.Neck: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.Head: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.HipLeft: return RootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.KneeLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.FootLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").Find("Bip01 L Foot").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.HipRight: return RootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.KneeRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.FootRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").Find("Bip01 R Foot").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").Find("Bip01 L Hand").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").Find("Bip01 R Hand").gameObject;
            default: return null;
        }
    }

    // This method takes an int in order to work with the join conversion method in TrackerHandler.cs  This adds compatibility for Kv2. 
    public static GameObject GetRocketboxBone(Windows.Kinect.JointType joint, GameObject RootPosition)
    {
        switch (joint)
        {
            case Windows.Kinect.JointType.SpineBase: return RootPosition;
            case Windows.Kinect.JointType.SpineMid: return RootPosition.transform.Find("Bip01 Spine").gameObject;
            case Windows.Kinect.JointType.SpineChest: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").gameObject;
            case Windows.Kinect.JointType.Neck: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").gameObject;
            case Windows.Kinect.JointType.Head: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").gameObject;
           // case Windows.Kinect.JointType.HipLeft: return RootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case Windows.Kinect.JointType.KneeLeft: return RootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case Windows.Kinect.JointType.AnkleLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case Windows.Kinect.JointType.FootLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").Find("Bip01 L Foot").gameObject;
          //  case Windows.Kinect.JointType.HipRight: return RootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case Windows.Kinect.JointType.KneeRight: return RootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case Windows.Kinect.JointType.AnkleRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case Windows.Kinect.JointType.FootRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").Find("Bip01 R Foot").gameObject;
            case Windows.Kinect.JointType.ShoulderLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").gameObject;
            case Windows.Kinect.JointType.ElbowLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").gameObject;
            case Windows.Kinect.JointType.WristLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").gameObject;
            case Windows.Kinect.JointType.ShoulderRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").gameObject;
            case Windows.Kinect.JointType.ElbowRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").gameObject;
            case Windows.Kinect.JointType.WristRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").gameObject;
            default: return null;
        }
    }


    public static string GetKinectObjectName(string boneName)
    {
        switch (boneName)
        {
            case "Bip01": return "pelvis";
            case "Bip01 Pelvis": return "pelvis";
            case "Bip01 Spine": return "spineNaval";
            case "Bip01 Spine1": return "spineChest";
            case "Bip01 Neck": return "neck";
            case "Bip01 Head": return "head";
            case "Bip01 L Thigh": return "leftKnee";
            case "Bip01 L Calf": return "leftAnkle";
            case "Bip01 L Foot": return "leftFoot";
            case "Bip01 R Thigh": return "rightKnee";
            case "Bip01 R Calf": return "rightAnkle";
            case "Bip01 R Foot": return "rightFoot";
            case "Bip01 L Clavicle": return "leftClavicle";
            case "Bip01 L UpperArm": return "leftShoulder";
            case "Bip01 L Forearm": return "leftElbow";
            case "Bip01 L Hand": return "leftWrist";
            case "Bip01 R Clavicle": return "rightClavicle";
            case "Bip01 R UpperArm": return "rightShoulder";
            case "Bip01 R Forearm": return "rightElbow";
            case "Bip01 R Hand": return "rightWrist";
            default: return "";
        }
    }


    public string GetKinect2ObjectName(string boneName)
    // This method attempts to replicate GetKienctObjectName for compatability for Kv2.
    {
        switch (boneName)
        {
            case "Bip01": return "pelvis";
            case "Bip01 Pelvis": return "pelvis";
            case "Bip01 Spine": return "spineMid";
            case "Bip01 Spine1": return "spineShoulder";
            case "Bip01 Neck": return "neck";
            case "Bip01 Head": return "head";
            case "Bip01 L Thigh": return "leftKnee";
            case "Bip01 L Calf": return "leftAnkle";
            case "Bip01 L Foot": return "leftFoot";
            case "Bip01 R Thigh": return "rightKnee";
            case "Bip01 R Calf": return "rightAnkle";
            case "Bip01 R Foot": return "rightFoot";
            case "Bip01 L Clavicle": return "leftShoulder";
            case "Bip01 L UpperArm": return "leftElbow";
            case "Bip01 L Forearm": return "leftWrist";
            case "Bip01 R Clavicle": return "rightShoulder";
            case "Bip01 R UpperArm": return "rightElbow";
            case "Bip01 R Forearm": return "rightWrist";
            default: return "";
        }
    }

    private void Start()
    {
        isruntime = true;
        StartAfterConfig();
    }

    public void StartMoCap()
    {
        gameObject.AddComponent<ConfigLoader>();
        KinectDevice = gameObject.AddComponent<TrackerHandler>();
        KinectDevice.isKinect2 = isKinect2;
        KinectDevice.drawSkeletons = DrawSkeleton;
        dp = gameObject.AddComponent<DataProvider>();
        if (isRealTime)
        {
            dp.StartAfterConfig();

        }
    }

    public void StartAfterConfig()
    {
        configOn = false;
        configured = true;

        if (ParentAvatar.TryGetComponent<MoveBoxPlayback>(out PlayBack))
        {
            PlayBack.standalone = false;
            PlayBack.enabled = false;
        }


        // START AVATAR
        RootPosition = ParentAvatar.transform.Find("Bip01").Find("Bip01 Pelvis").gameObject;
        
        originalAvatarAbsoluteRotations = new Dictionary<string, Quaternion>();
        Retrieve_OriginalAvatarRotations(RootPosition);

        GameObject rockethead = GetRocketboxBone(JointId.Head, RootPosition);
        GameObject rocketfoot = GetRocketboxBone(JointId.FootLeft, RootPosition);
        rocketboxHeight = rockethead.transform.position.y - rocketfoot.transform.position.y;

        Offset = RootPosition.transform.position;


       
        // START MOCAP

        StartMoCap();


        // Start Playback
        PlayBack = gameObject.AddComponent<MoveBoxPlayback>();
        PlayBack.standalone = false;
        PlayBack.ParentAvatar = ParentAvatar;
        PlayBack.RootPosition = RootPosition;
        PlayBack.animationOn = animationOn;
        PlayBack.eyeBlinkOn = eyeBlinkOn;
        PlayBack.lipsyncOn = lipsyncOn;

        //START EYEBLINK

        if (eyeBlinkOn)
        {
            PlayBack.StartEyeBlinks();
        }


        // START LIPSYNC
        
        if (lipsyncOn)
        {
            
            if (lipsyncMicOn)
            {
                PlayBack.lipsyncOn = false;
                if (!ParentAvatar.TryGetComponent<LipSync>(out lipsync))
                    lipsync = ParentAvatar.AddComponent<LipSync>();
                lipsync.nameOfMicrophone = nameMic;
                lipsync.realtimeMoCap = true;
                lipsync.StartAfterConfig(null, RootPosition);
               
            }
            else
            {
                
                PlayBack.audioClip = audioClip;
                PlayBack.StartLipSync();
            }

        }
        

        // ANIMATION PLAYBACK
        if (animationOn & !isRealTime)
        {
            PlayBack.animationClip = animationClip;
            PlayBack.loopingOn = loopingOn;
            PlayBack.StartAnimationPlayback();
         }
        else
        {
            PlayBack.animationOn =false;
        }
    }

    private void Retrieve_OriginalAvatarRotations(GameObject currentBone)
    {
        originalAvatarAbsoluteRotations[currentBone.name] = currentBone.transform.rotation;
        for (int i = 0; i < currentBone.transform.childCount; i++)
        {
            Retrieve_OriginalAvatarRotations(currentBone.transform.GetChild(i).gameObject);
        }

    }


    public void createAnimationRecorder()
    {
        // ANIMATION RECORDER


        AnimationRecorderSettings AnimRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
        RecorderControllerSettings controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();

        UnityEditor.Recorder.Input.AnimationInputSettings anim = new UnityEditor.Recorder.Input.AnimationInputSettings();
        anim.gameObject = ParentAvatar;
        Debug.Log(typeof(Transform));
        anim.AddComponentToRecord(typeof(Transform));
        anim.AddComponentToRecord(typeof(SkinnedMeshRenderer));
        anim.Recursive = true;


        AnimRecorder.name = "MoCapRocketbox";
        AnimRecorder.FrameRate = 30;
        AnimRecorder.FrameRatePlayback = FrameRatePlayback.Constant;
        AnimRecorder.CapFrameRate = true;
        filepath = Application.dataPath + "/Resources/RB_animation" + DateTime.Now.Day + "_" + DateTime.Now.TimeOfDay;
        AnimRecorder.OutputFile = filepath;
        AnimRecorder.RecordMode = RecordMode.Manual;
        AnimRecorder.AnimationInputSettings = anim;
        AnimRecorder.Enabled = true;

        // controllerSettings.FrameRate = 30;
        controllerSettings.AddRecorderSettings(AnimRecorder);
        recorder = new RecorderController(controllerSettings);
        // controllerSettings.SetRecordModeToManual();
        //  controllerSettings.Save();

    }

    private static void CopyRotation(Microsoft.Azure.Kinect.BodyTracking.JointId A_id, Microsoft.Azure.Kinect.BodyTracking.JointId B_id, GameObject RootPosition)
    {
        GameObject Abone = GetRocketboxBone(A_id, RootPosition);
        GameObject Bbone = GetRocketboxBone(B_id, RootPosition);
        if (Abone != null && Bbone != null)
        {
            Bbone.transform.rotation = Abone.transform.rotation;
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (configOn)
        {
            
            StartAfterConfig();
        
        }

     

        if (!configured || !isRealTime || !animationOn)
            return;

        if (resizeAvatar) //resize avatar
        {
            resizeAvatar = false;
            ResizeAvatar();
        }

        if (startRecording)
        {
            createAnimationRecorder();
            recorder.PrepareRecording();
            recorder.StartRecording();
            startRecording = false;
            isrecording = true;
        }

        if (stopRecording)
        {
            
            recorder.StopRecording();
            stopRecording = false;
            isrecording = false;
        }
        
    }
    public void ResizeAvatar()
    {
        GameObject rockethead = GetRocketboxBone(JointId.Head, RootPosition);
        GameObject rocketfoot = GetRocketboxBone(JointId.FootLeft, RootPosition);
        if (rockethead != null && rocketfoot != null)
        {
            // Debug.Log(currentbone.name + " " + (JointId)j);     
            GameObject MoCapHead = GameObject.Find(GetKinectObjectName(rockethead.name));
            GameObject MoCapFoot = GameObject.Find(GetKinectObjectName(rocketfoot.name));
            scale = (MoCapHead.transform.position.y - MoCapFoot.transform.position.y) / rocketboxHeight;
           // lipsync.avatar_scale = scale;
            RootPosition.transform.parent.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void LateUpdate()
    {
        if (!configured || !isRealTime || !animationOn)
            return;
   
        int countJoints = (int)Microsoft.Azure.Kinect.BodyTracking.JointId.Count;

        if (KinectDevice.isKinect2) // Run this loop if it is a Kinect2 Device.
            countJoints = (int)Windows.Kinect.JointType.Count;
         
        
        for (int j = 0; j < countJoints; j++)
        {
            Microsoft.Azure.Kinect.BodyTracking.JointId jointNum;
            GameObject currentbone;

            if (KinectDevice.isKinect2)
            {
                currentbone = GetRocketboxBone((Windows.Kinect.JointType)j, RootPosition);
                jointNum = Body.FromKinect2ToAzure((Windows.Kinect.JointType)j);
            }
            else
            {
                    jointNum = (Microsoft.Azure.Kinect.BodyTracking.JointId)j;
                    currentbone = GetRocketboxBone((Microsoft.Azure.Kinect.BodyTracking.JointId)j, RootPosition);
            }

            
            if (currentbone != null)
            {
                Transform finalJoint = currentbone.transform;

                if (originalAvatarAbsoluteRotations.ContainsKey(currentbone.name))
                {
                    // Exponential filter
                    {
                        finalJoint.rotation =
                            Quaternion.Slerp(
                                KinectDevice.absoluteJointRotations[(int)jointNum] * originalAvatarAbsoluteRotations[currentbone.name],
                                finalJoint.rotation,
                                _exponentialSmooth
                                );

                        //
                        // Correct for the roll angles for Kinect V2.
                        //
                        if (KinectDevice.isKinect2 && correct_roll_on)
                        {
                            Debug.DrawLine(currentbone.transform.position, currentbone.transform.position + currentbone.transform.right * (-0.5f), Color.red);


                            // Rotate the X axis from the direction in the original Avatar Absolute Rotation to the desired direction.

                            Vector3 Orig_Right = originalAvatarAbsoluteRotations[currentbone.name] * new Vector3(1,0,0);
                            Vector3 New_Right = finalJoint.right;

                            Vector3 Axis = Vector3.Cross(Orig_Right, New_Right);
                            float angle = Vector3.Angle(Orig_Right, New_Right);

                            finalJoint.rotation = originalAvatarAbsoluteRotations[currentbone.name];

                            const float epsilon = 0.000000001f;
                            if (angle >= epsilon)
                                finalJoint.transform.Rotate(Axis, angle, Space.World);

                        }
                    }
                }
            }
        }

        GameObject MoCapBone = GameObject.Find(GetKinect2ObjectName(RootPosition.name));
        Vector3 v = MoCapBone.transform.position;
        if(isRealTime)
            RootPosition.transform.localPosition = new Vector3(Offset.x - v.z, - v.x, + v.y);
        else
            RootPosition.transform.localPosition = new Vector3(Offset.x - v.z, Offset.y - v.x, Offset.z + v.y);
        if (_NoWristRotation)
        {
            CopyRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft, Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft, RootPosition);
            CopyRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight, Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight, RootPosition);
        }

            
             // Divide rotation with shoulder and clavicle
              //  DivideRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft, Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft);
              //  DivideRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight, Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight);

            
        
    }
}
