using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System;
using UnityEngine.SocialPlatforms;
//using System.Runtime.Remoting.Messaging;
using Unity.Collections;

public class KinectMoCapRocketboxAvatar : MonoBehaviour
{
    public TrackerHandler KinectDevice;
    public Dictionary<string, Quaternion> originalAvatarAbsoluteRotations;
    public Dictionary<string, float> originalAvatarBonePositions;

    public GameObject RootPosition;
    public float OffsetX, OffsetY, OffsetZ;
    // Noise reduction

    [Space]

    [Range(0.0f, 1.0f)]
    public float _exponentialSmooth = 0.75f;
    private float rocketboxHeight;

    void FixedUpdate()
    {

    }

    public GameObject GetRocketboxBone(JointId joinId)
    {
        switch (joinId)
        {
            case JointId.Pelvis: return RootPosition;
            case JointId.SpineNavel: return RootPosition.transform.Find("Bip01 Spine").gameObject;
            case JointId.SpineChest: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").gameObject;
            case JointId.Neck: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").gameObject;
            case JointId.Head: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").gameObject;
            case JointId.HipLeft: return RootPosition.transform.Find("Bip01 L Thigh").gameObject;
            case JointId.KneeLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").gameObject;
            case JointId.AnkleLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").Find("Bip01 L Foot").gameObject;
            case JointId.FootLeft: return RootPosition.transform.Find("Bip01 L Thigh").Find("Bip01 L Calf").Find("Bip01 L Foot").Find("Bip01 L Toe0").gameObject;
            case JointId.HipRight: return RootPosition.transform.Find("Bip01 R Thigh").gameObject;
            case JointId.KneeRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").gameObject;
            case JointId.AnkleRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").Find("Bip01 R Foot").gameObject;
            case JointId.FootRight: return RootPosition.transform.Find("Bip01 R Thigh").Find("Bip01 R Calf").Find("Bip01 R Foot").Find("Bip01 R Toe0").gameObject;
            case JointId.ClavicleLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").gameObject;
            case JointId.ShoulderLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").gameObject;
            case JointId.ElbowLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").gameObject;
            case JointId.WristLeft: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 L Clavicle").Find("Bip01 L UpperArm").Find("Bip01 L Forearm").Find("Bip01 L Hand").gameObject;
            case JointId.ClavicleRight: return  RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").gameObject;
            case JointId.ShoulderRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").gameObject;
            case JointId.ElbowRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").gameObject;
            case JointId.WristRight: return RootPosition.transform.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 R Clavicle").Find("Bip01 R UpperArm").Find("Bip01 R Forearm").Find("Bip01 R Hand").gameObject;
            default: return null;
        }
    }

    public string GetKinectObjectName(string boneName)
    {
        switch (boneName)
        {
            case "Bip01": return "pelvis";
            case "Bip01 Pelvis": return "pelvis";
            case "Bip01 Spine": return "spineNaval";
            case "Bip01 Spine1": return "spineChest";
            case "Bip01 Neck": return "neck";
            case "Bip01 Head": return "head";
            case "Bip01 L Thigh": return "leftHip";
            case "Bip01 L Calf": return "leftKnee";
            case "Bip01 L Foot": return "leftAnkle";
            case "Bip01 L Toe0": return "leftFoot";
            case "Bip01 R Thigh": return "rightHip";
            case "Bip01 R Calf": return "rightKnee";
            case "Bip01 R Foot": return "rightAnkle";
            case "Bip01 R Toe0": return "rightFoot";
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
    public string GetKinectObjectName(JointId boneName)
    {
        switch (boneName)
        {
            case JointId.Pelvis: return "pelvis";
            case JointId.SpineNavel: return "spineNaval";
            case JointId.SpineChest: return "spineChest";
            case JointId.Neck: return "neck";
            case JointId.Head: return "head";
            case JointId.HipLeft: return "leftHip";
            case JointId.KneeLeft: return "leftKnee";
            case JointId.AnkleLeft: return "leftAnkle";
            case JointId.FootLeft: return "leftFoot";
            case JointId.HipRight: return "rightHip";
            case JointId.KneeRight: return "rightKnee";
            case JointId.AnkleRight: return "rightAnkle";
            case JointId.FootRight: return "rightFoot";
            case JointId.ClavicleLeft: return "leftClavicle";
            case JointId.ShoulderLeft: return "leftShoulder";
            case JointId.ElbowLeft: return "leftElbow";
            case JointId.WristLeft: return "leftWrist";
            case JointId.ClavicleRight: return "rightClavicle";
            case JointId.ShoulderRight: return "rightShoulder";
            case JointId.ElbowRight: return "rightElbow";
            case JointId.WristRight: return "rightWrist";
            default: return null;
        }
    }

    public string GetRocketboxObjectName(string kinectBoneName)
    {
        switch (kinectBoneName)
        {
            case "pelvis": return "Bip01";
            case "spineNaval": return "Bip01 Spine";
            case "spineChest": return "Bip01 Spine1";
            case "neck": return "Bip01 Neck";
            case "head": return "Bip01 Head";
            case "leftHip": return "Bip01 L Thigh";
            case "leftKnee": return "Bip01 L Calf";
            case "leftAnkle": return "Bip01 L Foot";
            case "leftFoot": return "Bip01 L Toe0";
            case "rightHip": return "Bip01 R Thigh";
            case "rightKnee": return "Bip01 R Calf";
            case "rightAnkle": return "Bip01 R Foot";
            case "rightFoot": return "Bip01 R Toe0";
            case "leftClavicle": return "Bip01 L Clavicle";
            case "leftShoulder": return "Bip01 L UpperArm";
            case "leftElbow": return "Bip01 L Forearm";
            case "leftWrist": return "Bip01 L Hand";
            case "rightClavicle": return "Bip01 R Clavicle";
            case "rightShoulder": return "Bip01 R UpperArm";
            case "rightElbow": return "Bip01 R Forearm";
            case "rightWrist": return "Bip01 R Hand";
            default: return "";
        }
    }

    private void Start()
    {
        originalAvatarAbsoluteRotations = new Dictionary<string, Quaternion>(); 
        Retrieve_OriginalAvatarRotations(RootPosition);
        GameObject rockethead = GetRocketboxBone(JointId.Head);
        GameObject rocketfoot = GetRocketboxBone(JointId.FootLeft);
        rocketboxHeight = rockethead.transform.position.y - rocketfoot.transform.position.y;
        originalAvatarBonePositions = new Dictionary<string, float>();
        Retrieve_OriginalBonePositions(RootPosition);
    }

    public void Retrieve_OriginalBonePositions(GameObject currentBone)
    {
        originalAvatarBonePositions[currentBone.name] = (currentBone.transform.position - currentBone.transform.parent.position).magnitude;
        for (int i = 0; i < currentBone.transform.childCount; i++)
        {
            Retrieve_OriginalBonePositions(currentBone.transform.GetChild(i).gameObject);
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

    private void CopyRotation(JointId A_id, JointId B_id)
    {
        GameObject Abone = GetRocketboxBone(A_id);
        GameObject Bbone = GetRocketboxBone(B_id);
        if (Abone != null && Bbone != null)
        { 
            Bbone.transform.rotation = Abone.transform.rotation;
        }
    }



    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //resize avatar
        {
            GameObject rockethead = GetRocketboxBone(JointId.Head);
            GameObject rocketfoot = GetRocketboxBone(JointId.FootLeft);
            if (rockethead != null && rocketfoot != null)
            {
                // Debug.Log(currentbone.name + " " + (JointId)j);     
                GameObject MoCapHead = GameObject.Find(GetKinectObjectName(rockethead.name));
                GameObject MoCapFoot = GameObject.Find(GetKinectObjectName(rocketfoot.name));
                float scale = (MoCapHead.transform.position.y - MoCapFoot.transform.position.y) / rocketboxHeight;
                Debug.Log(scale);
                Debug.Log(RootPosition.transform.parent.parent.localScale);
                RootPosition.transform.parent.parent.localScale = new Vector3(scale, scale, scale);
                Retrieve_OriginalBonePositions(RootPosition);
            }

        /*    for (int jointID=0; jointID < (int)JointId.Count; jointID++)
            {
                if (GetRocketboxBone((JointId)jointID) != null && (JointId)jointID != JointId.Head)
                    SetScaledJointPositions((JointId)jointID);
            }*/


 /*           Vector3 v = new Vector3(1, 1, 1);
            //calculate size of the person from kinect data and resize the avatar

            //TODO ZELIA

            //calibrate the size of the avatar.
            RootPosition.transform.parent.parent.localScale=v;*/
        }
    }

    private void SetScaledJointPositions(JointId currentJoint)
    {

        if (currentJoint != JointId.Pelvis)
        {
            GameObject rocketcurrent = GetRocketboxBone(currentJoint);
            //GameObject rocketparent = GetRocketboxBone(KinectDevice.parentJointMap[currentKinectJoint]);

            Debug.Log(rocketcurrent.name);

            GameObject MoCapCurrent = GameObject.Find(GetKinectObjectName(currentJoint));
            GameObject MoCapParent = GameObject.Find(GetKinectObjectName(KinectDevice.parentJointMap[currentJoint]));

            Debug.Log(MoCapCurrent.name);

            float scale = (MoCapCurrent.transform.position - MoCapParent.transform.position).magnitude / originalAvatarBonePositions[rocketcurrent.name];
            rocketcurrent.transform.localScale = new Vector3(scale, scale, scale);

        }

        
    }
 
    private void LateUpdate()
    {
        for (int j = 0; j < (int)JointId.Count; j++)
        {
            GameObject currentbone = GetRocketboxBone((JointId)j);
            if (currentbone != null)
            {
                // Debug.Log(currentbone.name + " " + (JointId)j);             
                Transform finalJoint = currentbone.transform;

                if (originalAvatarAbsoluteRotations.ContainsKey(currentbone.name))
                {

                    // Exponential filter
                    finalJoint.rotation =
                        Quaternion.Slerp(
                            KinectDevice.absoluteJointRotations[j] * originalAvatarAbsoluteRotations[currentbone.name],
                            finalJoint.rotation,
                            _exponentialSmooth
                            );

                    if (currentbone.name == RootPosition.name)
                    {
                        GameObject MoCapBone = GameObject.Find(GetKinectObjectName(currentbone.name));
                        Vector3 v = MoCapBone.transform.position;
                        finalJoint.localPosition = new Vector3(OffsetX - v.z, OffsetY - v.x, OffsetZ + v.y);
                    }
                }
            }
        }
        // After update - check for wierd wrist rotations.

        
        CopyRotation(JointId.ElbowLeft, JointId.WristLeft);
        CopyRotation(JointId.ElbowRight, JointId.WristRight);

    }
}
