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

    public RecorderController recorder;
    public TrackerHandler kinectDevice;
    public MoveBoxPlayback playBack;
    public EyeBlink eyeBlinks;
    public DataProvider dp;

    public bool isKinect2, eyeBlinkOn = true, drawSkeleton = false, restartAnimation=false, correctRollOn=true;
    public bool startRecording = false, stopRecording = false, resizeAvatar = false, isrecording = false, isRuntime, lipSyncOn, animationOn = false, isRealTime = false, configOn = false, configured = false, loopingOn = false, pingpongOn = false, lipSyncMicOn=true;
    public LipSync lipSync;
    private float rocketboxHeight;
    Dictionary<string, Quaternion> originalAvatarAbsoluteRotations;
    public GameObject parentAvatar;
    public GameObject rootPosition;

    string filepath="";
    public Vector3 offset;
    public string nameMic;
    public float startPoint,endPoint=1;
    public AnimationClip animationClip;
    public AudioClip audioClip;
    public Animator animator;
    float scale;

    // Noise reduction

    [Space]

    [Range(0.0f, 1.0f)]
    public float _exponentialSmooth = 0.75f;

    public bool _NoWristRotation = false;

    public static GameObject GetRocketboxBone(Microsoft.Azure.Kinect.BodyTracking.JointId joinId, GameObject rootPosition)
    {
        switch (joinId)
        {
            case Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis: return rootPosition;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel: return rootPosition.transform.Find("Bip01 Spine").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.Neck: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.Head: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.HipLeft: return rootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.KneeLeft: return rootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleLeft: return rootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.FootLeft: return rootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").Find("Bip01 L Foot").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.HipRight: return rootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.KneeRight: return rootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleRight: return rootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.FootRight: return rootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").Find("Bip01 R Foot").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").Find("Bip01 L Hand").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").gameObject;
            case Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").Find("Bip01 R Hand").gameObject;
            default: return null;
        }
    }

    // This method takes an int in order to work with the join conversion method in TrackerHandler.cs  This adds compatibility for Kv2. 
    public static GameObject GetRocketboxBone(Windows.Kinect.JointType joint, GameObject rootPosition)
    {
        switch (joint)
        {
            case Windows.Kinect.JointType.SpineBase: return rootPosition;
            case Windows.Kinect.JointType.SpineMid: return rootPosition.transform.Find("Bip01 Spine").gameObject;
            case Windows.Kinect.JointType.SpineChest: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").gameObject;
            case Windows.Kinect.JointType.Neck: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").gameObject;
            case Windows.Kinect.JointType.Head: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").gameObject;
           // case Windows.Kinect.JointType.HipLeft: return rootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case Windows.Kinect.JointType.KneeLeft: return rootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case Windows.Kinect.JointType.AnkleLeft: return rootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case Windows.Kinect.JointType.FootLeft: return rootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").Find("Bip01 L Foot").gameObject;
          //  case Windows.Kinect.JointType.HipRight: return rootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case Windows.Kinect.JointType.KneeRight: return rootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case Windows.Kinect.JointType.AnkleRight: return rootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case Windows.Kinect.JointType.FootRight: return rootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").Find("Bip01 R Foot").gameObject;
            case Windows.Kinect.JointType.ShoulderLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").gameObject;
            case Windows.Kinect.JointType.ElbowLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").gameObject;
            case Windows.Kinect.JointType.WristLeft: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").gameObject;
            case Windows.Kinect.JointType.ShoulderRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").gameObject;
            case Windows.Kinect.JointType.ElbowRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").gameObject;
            case Windows.Kinect.JointType.WristRight: return rootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").gameObject;
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
        isRuntime = true;
        StartAfterConfig();
    }

    public void StartMoCap()
    {
        gameObject.AddComponent<ConfigLoader>();
        kinectDevice = gameObject.AddComponent<TrackerHandler>();
        kinectDevice.isKinect2 = isKinect2;
        kinectDevice.drawSkeletons = drawSkeleton;
        if (!drawSkeleton)
            kinectDevice.turnOnOffSkeletons();
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

        if (parentAvatar.TryGetComponent<MoveBoxPlayback>(out playBack))
        {
            playBack.standalone = false;
            playBack.enabled = false;
        }


        // START AVATAR
        rootPosition = parentAvatar.transform.Find("Bip01").Find("Bip01 Pelvis").gameObject;
        
        originalAvatarAbsoluteRotations = new Dictionary<string, Quaternion>();
        Retrieve_OriginalAvatarRotations(rootPosition);

        GameObject rockethead = GetRocketboxBone(JointId.Head, rootPosition);
        GameObject rocketfoot = GetRocketboxBone(JointId.FootLeft, rootPosition);
        rocketboxHeight = rockethead.transform.position.y - rocketfoot.transform.position.y;

        offset = rootPosition.transform.position;


       
        // START MOCAP

        StartMoCap();


        // Start playBack
        playBack = gameObject.AddComponent<MoveBoxPlayback>();
        playBack.standalone = false;
        playBack.parentAvatar = parentAvatar;
        playBack.rootPosition = rootPosition;
        playBack.animationOn = animationOn;
        playBack.eyeBlinkOn = eyeBlinkOn;
        playBack.lipSyncOn = lipSyncOn;

        //START EYEBLINK

        if (eyeBlinkOn)
        {
            playBack.StartEyeBlinks();
        }


        // START lipSync
        
        if (lipSyncOn)
        {
            
            if (lipSyncMicOn)
            {
                playBack.lipSyncOn = false;
                if (!parentAvatar.TryGetComponent<LipSync>(out lipSync))
                    lipSync = parentAvatar.AddComponent<LipSync>();
                lipSync.nameOfMicrophone = nameMic;
                lipSync.realtimeMoCap = true;
                lipSync.StartAfterConfig(null, rootPosition);
               
            }
            else
            {
                
                playBack.audioClip = audioClip;
                playBack.StartLipSync();
            }

        }
        

        // ANIMATION playBack
        if (animationOn & !isRealTime)
        {
            playBack.animationClip = animationClip;
            playBack.loopingOn = loopingOn;
            playBack.StartAnimationPlayback();
         }
        else
        {
            playBack.animationOn =false;
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
        anim.gameObject = parentAvatar;
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

    private static void CopyRotation(Microsoft.Azure.Kinect.BodyTracking.JointId A_id, Microsoft.Azure.Kinect.BodyTracking.JointId B_id, GameObject rootPosition)
    {
        GameObject Abone = GetRocketboxBone(A_id, rootPosition);
        GameObject Bbone = GetRocketboxBone(B_id, rootPosition);
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
        GameObject rockethead = GetRocketboxBone(JointId.Head, rootPosition);
        GameObject rocketfoot = GetRocketboxBone(JointId.FootLeft, rootPosition);
        if (rockethead != null && rocketfoot != null)
        {
            // Debug.Log(currentbone.name + " " + (JointId)j);     
            GameObject MoCapHead = GameObject.Find(GetKinectObjectName(rockethead.name));
            GameObject MoCapFoot = GameObject.Find(GetKinectObjectName(rocketfoot.name));
            scale = (MoCapHead.transform.position.y - MoCapFoot.transform.position.y) / rocketboxHeight;
           // lipSync.avatar_scale = scale;
            rootPosition.transform.parent.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void LateUpdate()
    {
        if (!configured || !isRealTime || !animationOn)
            return;
   
        int countJoints = (int)Microsoft.Azure.Kinect.BodyTracking.JointId.Count;

        if (kinectDevice.isKinect2) // Run this loop if it is a Kinect2 Device.
            countJoints = (int)Windows.Kinect.JointType.Count;
         
        
        for (int j = 0; j < countJoints; j++)
        {
            Microsoft.Azure.Kinect.BodyTracking.JointId jointNum;
            GameObject currentbone;

            if (kinectDevice.isKinect2)
            {
                currentbone = GetRocketboxBone((Windows.Kinect.JointType)j, rootPosition);
                jointNum = Body.FromKinect2ToAzure((Windows.Kinect.JointType)j);
            }
            else
            {
                    jointNum = (Microsoft.Azure.Kinect.BodyTracking.JointId)j;
                    currentbone = GetRocketboxBone((Microsoft.Azure.Kinect.BodyTracking.JointId)j, rootPosition);
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
                                kinectDevice.absoluteJointRotations[(int)jointNum] * originalAvatarAbsoluteRotations[currentbone.name],
                                finalJoint.rotation,
                                _exponentialSmooth
                                );

                        //
                        // Correct for the roll angles for Kinect V2.
                        //
                        if (kinectDevice.isKinect2 && correctRollOn)
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

        GameObject MoCapBone = GameObject.Find(GetKinect2ObjectName(rootPosition.name));
        Vector3 v = MoCapBone.transform.position;
        if(isRealTime)
            rootPosition.transform.localPosition = new Vector3(offset.x - v.z, - v.x, + v.y);
        else
            rootPosition.transform.localPosition = new Vector3(offset.x - v.z, offset.y - v.x, offset.z + v.y);
        if (_NoWristRotation)
        {
            CopyRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft, Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft, rootPosition);
            CopyRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight, Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight, rootPosition);
        }

            
             // Divide rotation with shoulder and clavicle
              //  DivideRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft, Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft);
              //  DivideRotation(Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight, Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight);

            
        
    }
}
