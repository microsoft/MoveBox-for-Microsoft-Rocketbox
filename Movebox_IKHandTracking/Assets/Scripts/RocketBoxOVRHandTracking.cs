using System.Collections.Generic;
using UnityEngine;
using bid = OVRSkeleton.BoneId;

/// <summary>
/// Class for mapping Oculus hand tracking to RocketBox avatar hand
/// Attach this script to RocketBox avatar gameobject.
/// </summary>
public class RocketBoxOVRHandTracking : MonoBehaviour
{
    public OVRSkeleton OVRSkeleton_L;
    public OVRSkeleton OVRSkeleton_R;

    private RocketBoxHand handL;
    private RocketBoxHand handR;

    void Start()
    {
        handL = new RocketBoxHand(RocketBoxHand.HandType.LeftHand);
        handR = new RocketBoxHand(RocketBoxHand.HandType.RightHand, new Vector3(0.0f, 0.0f, 180.0f)); 
    }

    void LateUpdate()
    {
        handL.UpdateHand(OVRSkeleton_L);
        handR.UpdateHand(OVRSkeleton_R);
    }

    /// <summary>
    /// Class for RocketBox avatar's hand, including 5 fingers and 3 bones on each finger.
    /// </summary>
    class RocketBoxHand
    {
        public enum HandType
        {
            LeftHand,
            RightHand
        }

        public HandType handType;
        public FingerBone[,] fingerBones = new FingerBone[5,3];

        /// <summary>
        /// Auto detect finger bones, set offset.
        /// </summary>
        /// <param name="type">Left or right hand</param>
        /// <param name="offset">Offset the finger rotation to make it works correctly. For right hand, it needs an offset of Vector3(0.0f, 0.0f, 180.0f).</param>
        public RocketBoxHand(HandType type, Vector3 offset = new Vector3())
        {
            handType = type;
            for (int fingerIndex = 0; fingerIndex <= fingerBones.GetUpperBound(0); fingerIndex++)
            {
                for(int fingerBoneIndex = 0; fingerBoneIndex <= fingerBones.GetUpperBound(1); fingerBoneIndex++)
                {
                    string name = "Finger" + fingerIndex + ((fingerBoneIndex == 0) ? "" : fingerBoneIndex.ToString());
                    string namePrefix = "Bip01 " + ((type == HandType.LeftHand) ? "L " : "R ");
                    Transform transform = GameObject.Find(namePrefix + name).transform;

                    fingerBones[fingerIndex, fingerBoneIndex] = new FingerBone(name, transform, offset);
                }
            }
        }

        /// <summary>
        /// Update the fingers of this hand, so they rotate as the OVR skeleton rotates. Call this in your program's LateUpdate()
        /// </summary>
        /// <param name="s"> OVRSkeleton, provided by Oculus Integration SDK. </param>
        public void UpdateHand(OVRSkeleton s)
        {
            if (s == null || s.Bones.Count == 0)
            {
                return;
            }

            foreach (FingerBone f in fingerBones)
            {
                f.UpdateFinger(s); // if we want to change the offset for fingers, we can use UpdateFinger(s, newOffset).
            }
        }

        override public string ToString()
        {
            string s = "";
            s += $"_fingerBones.length: {fingerBones.Length}";
            foreach (FingerBone f in fingerBones)
            {
                s += f.ToString();
                s += "\n";
            }
            return s;
        }

        public class FingerBone
        {
            public string name;
            public Transform transform;
            public Vector3 offset;

            public Dictionary<string, bid> boneMapping = new Dictionary<string, bid>
            {
                { "Finger0", bid.Hand_Thumb1 },
                { "Finger01", bid.Hand_Thumb2 },
                { "Finger02", bid.Hand_Thumb3 },

                { "Finger1", bid.Hand_Index1 },
                { "Finger11", bid.Hand_Index2 },
                { "Finger12", bid.Hand_Index3 },

                { "Finger2", bid.Hand_Middle1 },
                { "Finger21", bid.Hand_Middle2 },
                { "Finger22", bid.Hand_Middle3 },

                { "Finger3", bid.Hand_Ring1 },
                { "Finger31", bid.Hand_Ring2 },
                { "Finger32", bid.Hand_Ring3 },

                { "Finger4", bid.Hand_Pinky1 },
                { "Finger41", bid.Hand_Pinky2 },
                { "Finger42", bid.Hand_Pinky3 },
            };

            public FingerBone(string n, Transform t, Vector3 o)
            {
                name = n;
                transform = t;
                offset = o;
            }

            public void UpdateFinger(OVRSkeleton s)
            {
                if (s == null || s.Bones.Count == 0)
                {
                    return;
                }

                Transform tracking = s.Bones[(int)boneMapping[name]].Transform;

                SetProperties(transform, tracking);
            }

            public void UpdateFinger(OVRSkeleton s, Vector3 newOffset)
            {
                offset = newOffset;
                UpdateFinger(s);
            }

            private void SetProperties(Transform d1, Transform d2)
            {
                if (d1 == null || d2 == null)
                {
                    return;
                }

                //d1.position = d2.position; // Uncomment this if you want elastic fingers like alien ;)
                d1.transform.rotation = d2.rotation;
                d1.transform.rotation *= Quaternion.Euler(offset);
            }

            override public string ToString()
            {
                if (transform == null)
                {
                    return name + ": transform null";
                }

                return $"{name}: Position: {transform.position}; Rotation: {transform.rotation}";
            }
        }
    }
}
