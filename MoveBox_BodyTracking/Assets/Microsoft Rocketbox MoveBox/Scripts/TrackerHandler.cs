using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Windows.Kinect;
using UnityEditor;

[CustomEditor(typeof(TrackerHandler))]
public class TrackerHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
    }
}
public class TrackerHandler : MonoBehaviour
{
    public Dictionary<Microsoft.Azure.Kinect.BodyTracking.JointId, Microsoft.Azure.Kinect.BodyTracking.JointId> parentJointMap;
    Dictionary<Microsoft.Azure.Kinect.BodyTracking.JointId, Quaternion> basisJointMap;

    // Second version of parent-joint and basis maps for Kinect v2
    public Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> parentJointMapK2;
    Dictionary<Windows.Kinect.JointType, Quaternion> basisJointMapK2;


    public Quaternion[] absoluteJointRotations = new Quaternion[(int)Microsoft.Azure.Kinect.BodyTracking.JointId.Count];
    public bool drawSkeletons = true;

    public bool isKinect2 = true;
    
    Quaternion Y_180_FLIP = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Awake()
    {
  
            parentJointMapK2 = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>();

            // SpineBase does not have a parent, set it to itself.
            parentJointMapK2[Windows.Kinect.JointType.SpineBase] = Windows.Kinect.JointType.SpineBase;

            // Spine and Head Joints
            parentJointMapK2[Windows.Kinect.JointType.SpineMid] = Windows.Kinect.JointType.SpineBase;
            parentJointMapK2[Windows.Kinect.JointType.SpineChest] = Windows.Kinect.JointType.SpineMid;
            parentJointMapK2[Windows.Kinect.JointType.Neck] = Windows.Kinect.JointType.SpineChest;
            parentJointMapK2[Windows.Kinect.JointType.Head] = Windows.Kinect.JointType.Neck;

            // Left Arm Joints
            parentJointMapK2[Windows.Kinect.JointType.ShoulderLeft] = Windows.Kinect.JointType.SpineChest;
            parentJointMapK2[Windows.Kinect.JointType.ElbowLeft] = Windows.Kinect.JointType.ShoulderLeft;
            parentJointMapK2[Windows.Kinect.JointType.WristLeft] = Windows.Kinect.JointType.ElbowLeft;
            parentJointMapK2[Windows.Kinect.JointType.HandLeft] = Windows.Kinect.JointType.WristLeft;
            parentJointMapK2[Windows.Kinect.JointType.HandTipLeft] = Windows.Kinect.JointType.HandLeft;

            // Right Arm Joints
            parentJointMapK2[Windows.Kinect.JointType.ShoulderRight] = Windows.Kinect.JointType.SpineChest;
            parentJointMapK2[Windows.Kinect.JointType.ElbowRight] = Windows.Kinect.JointType.ShoulderRight;
            parentJointMapK2[Windows.Kinect.JointType.WristRight] = Windows.Kinect.JointType.ElbowRight;
            parentJointMapK2[Windows.Kinect.JointType.HandRight] = Windows.Kinect.JointType.WristRight;
            parentJointMapK2[Windows.Kinect.JointType.HandTipRight] = Windows.Kinect.JointType.HandRight;

            // Left Leg Joints
            parentJointMapK2[Windows.Kinect.JointType.HipLeft] = Windows.Kinect.JointType.SpineBase;
            parentJointMapK2[Windows.Kinect.JointType.KneeLeft] = Windows.Kinect.JointType.HipLeft;
            parentJointMapK2[Windows.Kinect.JointType.AnkleLeft] = Windows.Kinect.JointType.KneeLeft;
            parentJointMapK2[Windows.Kinect.JointType.FootLeft] = Windows.Kinect.JointType.AnkleLeft;

            // Right Leg Joints
            parentJointMapK2[Windows.Kinect.JointType.HipRight] = Windows.Kinect.JointType.SpineBase;
            parentJointMapK2[Windows.Kinect.JointType.KneeRight] = Windows.Kinect.JointType.HipRight;
            parentJointMapK2[Windows.Kinect.JointType.AnkleRight] = Windows.Kinect.JointType.KneeRight;
            parentJointMapK2[Windows.Kinect.JointType.FootRight] = Windows.Kinect.JointType.AnkleRight;


            Vector3 zpositive = Vector3.forward;
            Vector3 xpositive = Vector3.right;
            Vector3 ypositive = Vector3.up;
            // spine and left hip are the same
            Quaternion leftHipBasis = Quaternion.LookRotation(-xpositive, -ypositive) ; //Quaternion.LookRotation(xpositive, -zpositive);
            Quaternion spineHipBasis = Quaternion.identity; //Quaternion.LookRotation(zpositive, ypositive);// Quaternion.identity; //Quaternion.LookRotation(xpositive, zpositive);
            Quaternion rightHipBasis = Quaternion.LookRotation(xpositive, -ypositive);  //Quaternion.LookRotation( -xpositive,-zpositive); //Quaternion.LookRotation(xpositive, zpositive);
                                                                                        // arms and thumbs share the same basis
            Quaternion leftArmBasis = Quaternion.LookRotation(ypositive, -xpositive);// * Quaternion.LookRotation(-zpositive);
            Quaternion rightArmBasis = Quaternion.LookRotation(ypositive,xpositive );
            Quaternion leftHandBasis = Quaternion.LookRotation(zpositive, -xpositive);
            Quaternion rightHandBasis = Quaternion.LookRotation(zpositive, xpositive);

            Quaternion leftFootBasis = Quaternion.LookRotation(-xpositive, -ypositive); //Quaternion.LookRotation(xpositive, zpositive);
            Quaternion rightFootBasis = Quaternion.LookRotation(xpositive, -ypositive); //Quaternion.LookRotation(xpositive, -zpositive);



 

            basisJointMapK2 = new Dictionary<Windows.Kinect.JointType, Quaternion>();

            // Update basisJointMap for Kinect V2
            basisJointMapK2[Windows.Kinect.JointType.SpineBase] = spineHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.SpineMid] = spineHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.SpineChest] = spineHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.Neck] = spineHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.Head] = spineHipBasis;

            // Left Arm Basis
            basisJointMapK2[Windows.Kinect.JointType.ShoulderLeft] = leftArmBasis;
            basisJointMapK2[Windows.Kinect.JointType.ElbowLeft] = leftArmBasis;
            basisJointMapK2[Windows.Kinect.JointType.WristLeft] = leftArmBasis;
            basisJointMapK2[Windows.Kinect.JointType.HandLeft] = leftHandBasis;
            basisJointMapK2[Windows.Kinect.JointType.HandTipLeft] = leftHandBasis;

            // Right Arm Basis
            basisJointMapK2[Windows.Kinect.JointType.ShoulderRight] = rightArmBasis;
            basisJointMapK2[Windows.Kinect.JointType.ElbowRight] = rightArmBasis;
            basisJointMapK2[Windows.Kinect.JointType.WristRight] = rightArmBasis;
            basisJointMapK2[Windows.Kinect.JointType.HandRight] = rightHandBasis;
            basisJointMapK2[Windows.Kinect.JointType.HandTipRight] = rightHandBasis;

            // Left Leg Basis
            basisJointMapK2[Windows.Kinect.JointType.HipLeft] = leftHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.KneeLeft] = leftHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.AnkleLeft] = leftHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.FootLeft] = leftFootBasis;

            // Right Leg Basis
            basisJointMapK2[Windows.Kinect.JointType.HipRight] = rightHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.KneeRight] = rightHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.AnkleRight] = rightHipBasis;
            basisJointMapK2[Windows.Kinect.JointType.FootRight] = rightFootBasis;


            parentJointMap = new Dictionary<Microsoft.Azure.Kinect.BodyTracking.JointId, Microsoft.Azure.Kinect.BodyTracking.JointId>();

            // pelvis has no parent so set to count
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis] = Microsoft.Azure.Kinect.BodyTracking.JointId.Count;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel] = Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest] = Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Neck] = Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandTipLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.HandLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ThumbLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.HandLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandTipRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.HandRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ThumbRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.HandRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HipLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.KneeLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.HipLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.KneeLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.FootLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleLeft;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HipRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.KneeRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.HipRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.KneeRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.FootRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleRight;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Head] = Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Nose] = Microsoft.Azure.Kinect.BodyTracking.JointId.Head;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EyeLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.Head;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EarLeft] = Microsoft.Azure.Kinect.BodyTracking.JointId.Head;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EyeRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.Head;
            parentJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EarRight] = Microsoft.Azure.Kinect.BodyTracking.JointId.Head;

            zpositive = Vector3.forward;
            xpositive = Vector3.right;
            ypositive = Vector3.up;
            // spine and left hip are the same
            leftHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
            spineHipBasis = Quaternion.LookRotation(xpositive, -zpositive);
            rightHipBasis = Quaternion.LookRotation(xpositive, zpositive);
            // arms and thumbs share the same basis
            leftArmBasis = Quaternion.LookRotation(ypositive, -zpositive);
            rightArmBasis = Quaternion.LookRotation(-ypositive, zpositive);
            leftHandBasis = Quaternion.LookRotation(-zpositive, -ypositive);
            rightHandBasis = Quaternion.identity;
            leftFootBasis = Quaternion.LookRotation(xpositive, ypositive);
            rightFootBasis = Quaternion.LookRotation(xpositive, -ypositive);

            basisJointMap = new Dictionary<Microsoft.Azure.Kinect.BodyTracking.JointId, Quaternion>();

            // pelvis has no parent so set to count
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.SpineNavel] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.SpineChest] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Neck] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleLeft] = leftArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderLeft] = leftArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowLeft] = leftArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.WristLeft] = leftHandBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandLeft] = leftHandBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandTipLeft] = leftHandBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ThumbLeft] = leftArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ClavicleRight] = rightArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ShoulderRight] = rightArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ElbowRight] = rightArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.WristRight] = rightHandBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandRight] = rightHandBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HandTipRight] = rightHandBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.ThumbRight] = rightArmBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HipLeft] = leftHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.KneeLeft] = leftHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleLeft] = leftHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.FootLeft] = leftFootBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.HipRight] = rightHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.KneeRight] = rightHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.AnkleRight] = rightHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.FootRight] = rightFootBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Head] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.Nose] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EyeLeft] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EarLeft] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EyeRight] = spineHipBasis;
            basisJointMap[Microsoft.Azure.Kinect.BodyTracking.JointId.EarRight] = spineHipBasis;
       
    }

    public void updateTracker(BackgroundData trackerFrameData)
    {
        //this is an array in case you want to get the n closest bodies
        int closestBody = findClosestTrackedBody(trackerFrameData);

        // render the closest body
        Body skeleton = trackerFrameData.Bodies[0]; //closestBody
        renderSkeleton(skeleton, 0);
    }

    int findIndexFromId(BackgroundData frameData, int id)
    {
        int retIndex = -1;
        for (int i = 0; i < (int)frameData.NumOfBodies; i++)
        {
            if ((int)frameData.Bodies[i].Id == id)
            {
                retIndex = i;
                break;
            }
        }
        return retIndex;
    }

    private int findClosestTrackedBody(BackgroundData trackerFrameData)
    {
        int closestBody = -1;
        const float MAX_DISTANCE = 5000.0f;
        float minDistanceFromKinect = MAX_DISTANCE;
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)Microsoft.Azure.Kinect.BodyTracking.JointId.Pelvis];
            Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
            if (pelvisPos.magnitude < minDistanceFromKinect)
            {
                closestBody = i;
                minDistanceFromKinect = pelvisPos.magnitude;
            }
        }
        return closestBody;
    }

    public void turnOnOffSkeletons()
    {
        drawSkeletons = !drawSkeletons;
        const int bodyRenderedNum = 0;
        for (int jointNum = 0; jointNum < (int)Microsoft.Azure.Kinect.BodyTracking.JointId.Count; jointNum++)
        {
            transform.GetChild(bodyRenderedNum).GetChild(jointNum).gameObject.GetComponent<MeshRenderer>().enabled = drawSkeletons;
            transform.GetChild(bodyRenderedNum).GetChild(jointNum).GetChild(0).GetComponent<MeshRenderer>().enabled = drawSkeletons;
        }
    }

    public void renderSkeleton(Body skeleton, int skeletonNumber)
    {
        int total_joins = (int)Microsoft.Azure.Kinect.BodyTracking.JointId.Count;
        if (isKinect2)
        {
            total_joins = (int)Windows.Kinect.JointType.Count;
        }
        

        for (int joint = 0; joint < total_joins; joint++)
        {
            if ( isKinect2 && (joint == 22 || joint == 24) ) {
            // The thumb joints were breaking this function for the kinect 2.  Ignore them for now.
               continue;
            }
            Microsoft.Azure.Kinect.BodyTracking.JointId jointNum = (Microsoft.Azure.Kinect.BodyTracking.JointId)joint;
            if (isKinect2)
                jointNum = Body.FromKinect2ToAzure((Windows.Kinect.JointType)joint);


            
            Vector3 jointPos;
            if (isKinect2)// Note:  Kinect2 X & Y axis are actually opposite from K4A.  Make -x and +y for compatibility
            {
                jointPos = new Vector3(-skeleton.JointPositions3D[joint].X, skeleton.JointPositions3D[joint].Y, skeleton.JointPositions3D[joint].Z);
            } 
            else
            {
                jointPos = new Vector3(skeleton.JointPositions3D[(int)jointNum].X, -skeleton.JointPositions3D[(int)jointNum].Y, skeleton.JointPositions3D[(int)jointNum].Z);
            }
            Vector3 offsetPosition = transform.rotation * jointPos;

            Vector3 positionInTrackerRootSpace = transform.position + offsetPosition;

            Quaternion jointRot = new Quaternion();
            if (isKinect2)
            {
                jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[joint].X, skeleton.JointRotations[joint].Y,
                skeleton.JointRotations[joint].Z, skeleton.JointRotations[joint].W) * Quaternion.Inverse(basisJointMapK2[(Windows.Kinect.JointType)joint]);
            }
            else
            {
                jointRot = Y_180_FLIP * new Quaternion(skeleton.JointRotations[(int)jointNum].X, skeleton.JointRotations[(int)jointNum].Y,
                skeleton.JointRotations[(int)jointNum].Z, skeleton.JointRotations[(int)jointNum].W) * Quaternion.Inverse(basisJointMap[jointNum]);

            }

            if (jointNum != Microsoft.Azure.Kinect.BodyTracking.JointId.Count)
            {

                absoluteJointRotations[(int)jointNum] = jointRot;

                // these are absolute body space because each joint has the body root for a parent in the scene graph
                transform.GetChild(skeletonNumber).GetChild((int)jointNum).localPosition = jointPos;
                transform.GetChild(skeletonNumber).GetChild((int)jointNum).localRotation = jointRot;

                const int boneChildNum = 0;
                if (parentJointMap[(Microsoft.Azure.Kinect.BodyTracking.JointId)jointNum] != Microsoft.Azure.Kinect.BodyTracking.JointId.Head && parentJointMap[(Microsoft.Azure.Kinect.BodyTracking.JointId)jointNum] != Microsoft.Azure.Kinect.BodyTracking.JointId.Count)
                {
                    Vector3 parentTrackerSpacePosition;
                    if (isKinect2)
                    {
                        parentTrackerSpacePosition = new Vector3(-skeleton.JointPositions3D[(int)parentJointMapK2[(Windows.Kinect.JointType)joint]].X,
                        skeleton.JointPositions3D[(int)parentJointMapK2[(Windows.Kinect.JointType)joint]].Y, skeleton.JointPositions3D[(int)parentJointMapK2[(Windows.Kinect.JointType)joint]].Z);
                    }
                    else
                    {
                         parentTrackerSpacePosition = new Vector3(skeleton.JointPositions3D[(int)parentJointMap[jointNum]].X,
                        -skeleton.JointPositions3D[(int)parentJointMap[jointNum]].Y, skeleton.JointPositions3D[(int)parentJointMap[jointNum]].Z);

                    }
                    Vector3 boneDirectionTrackerSpace = jointPos - parentTrackerSpacePosition;
                    Vector3 boneDirectionWorldSpace = transform.rotation * boneDirectionTrackerSpace;
                    Vector3 boneDirectionLocalSpace = Quaternion.Inverse(transform.GetChild(skeletonNumber).GetChild((int)jointNum).rotation) * Vector3.Normalize(boneDirectionWorldSpace);
                    transform.GetChild(skeletonNumber).GetChild((int)jointNum).GetChild(boneChildNum).localScale = new Vector3(1, 20.0f * 0.5f * boneDirectionWorldSpace.magnitude, 1);
                    transform.GetChild(skeletonNumber).GetChild((int)jointNum).GetChild(boneChildNum).localRotation = Quaternion.FromToRotation(Vector3.up, boneDirectionLocalSpace);
                    transform.GetChild(skeletonNumber).GetChild((int)jointNum).GetChild(boneChildNum).position = transform.GetChild(skeletonNumber).GetChild((int)jointNum).position - 0.5f * boneDirectionWorldSpace;
                }
                else
                {
                    transform.GetChild(skeletonNumber).GetChild((int)jointNum).GetChild(boneChildNum).gameObject.SetActive(false);
                }
            }
        }
    }

    public Quaternion GetRelativeJointRotation(Microsoft.Azure.Kinect.BodyTracking.JointId joint)
    {

        Microsoft.Azure.Kinect.BodyTracking.JointId parent = parentJointMap[joint];
        Quaternion parentJointRotationBodySpace = Quaternion.identity;
        if (parent == Microsoft.Azure.Kinect.BodyTracking.JointId.Count)
        {
            parentJointRotationBodySpace = Y_180_FLIP;
        }
        else
        {
            parentJointRotationBodySpace = absoluteJointRotations[(int)parent];
        }
        Quaternion jointRotationBodySpace = absoluteJointRotations[(int)joint];
        Quaternion relativeRotation = Quaternion.Inverse(parentJointRotationBodySpace) * jointRotationBodySpace;

        return relativeRotation;

    }

    // This method is an override that uses the Kinect 2 Windows.Kinect.JointType
    public Quaternion GetRelativeJointRotation(Windows.Kinect.JointType joint)
    {

        Windows.Kinect.JointType parent = parentJointMapK2[joint];
        Quaternion parentJointRotationBodySpace = Quaternion.identity;

        parentJointRotationBodySpace = absoluteJointRotations[(int)parent];

        Quaternion jointRotationBodySpace = absoluteJointRotations[(int)joint];
        Quaternion relativeRotation = Quaternion.Inverse(parentJointRotationBodySpace) * jointRotationBodySpace;

        return relativeRotation;

    }

}
