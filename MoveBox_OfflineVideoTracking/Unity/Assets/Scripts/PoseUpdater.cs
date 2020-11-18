using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Helper class to draw fancy orientations (using cylinders) or simple orientations
/// and accelerations (using Gizmos).
/// </summary>
public class DrawingHelper
{

    private GameObject _cylinderX = null;
    private GameObject _cylinderY = null;
    private GameObject _cylinderZ = null;

    public DrawingHelper() { }

    /// <summary>
    /// Creates the cylinders if they don't exist already
    /// </summary>
    public void prepare()
    {
        if (_cylinderX == null)
        {
            _cylinderX = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _cylinderX.transform.position = new Vector3(0, 0, 0);
            _cylinderX.transform.localScale = new Vector3(.1f, .1f, .1f);
            _cylinderX.GetComponent<Renderer>().material.color = Color.red;
        }

        if (_cylinderY == null)
        {
            _cylinderY = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _cylinderY.transform.position = new Vector3(0, 0, 0);
            _cylinderY.transform.localScale = new Vector3(.1f, .1f, .1f);
            _cylinderY.GetComponent<Renderer>().material.color = Color.green;
        }

        if (_cylinderZ == null)
        {
            _cylinderZ = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _cylinderZ.transform.position = new Vector3(0, 0, 0);
            _cylinderZ.transform.localScale = new Vector3(.1f, .1f, .1f);
            _cylinderZ.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    /// <summary>
    /// Draw orientation using Gizmos.
    /// </summary>
    /// <param name="quat"></param>
    /// <param name="pos"></param>
    public static void DrawRotationAxes(Quaternion quat, Vector3 pos)
    {
        Matrix4x4 rot = Matrix4x4.Rotate(quat);

        // negate X because Unity is left-handed
        float lh_correction = -1.0f;
        Vector3 x = new Vector3(lh_correction * rot.GetColumn(0).x, rot.GetColumn(0).y, rot.GetColumn(0).z) * 0.2f;
        Vector3 y = new Vector3(lh_correction * rot.GetColumn(1).x, rot.GetColumn(1).y, rot.GetColumn(1).z) * 0.2f;
        Vector3 z = new Vector3(lh_correction * rot.GetColumn(2).x, rot.GetColumn(2).y, rot.GetColumn(2).z) * 0.2f;

        Debug.DrawLine(pos, pos + x, Color.red);
        Debug.DrawLine(pos, pos + y, Color.green);
        Debug.DrawLine(pos, pos + z, Color.blue);
    }

    /// <summary>
    /// Draw fancy orientations using cylinders.
    /// </summary>
    /// <param name="quat"></param>
    /// <param name="pos"></param>
    public void DrawRotationCylinder(Quaternion quat, Vector3 pos)
    {
        Matrix4x4 rot = Matrix4x4.Rotate(quat);

        // negate X because Unity is left-handed
        float lh_correction = -1.0f;
        Vector3 x = new Vector3(lh_correction * rot.GetColumn(0).x, rot.GetColumn(0).y, rot.GetColumn(0).z);
        Vector3 y = new Vector3(lh_correction * rot.GetColumn(1).x, rot.GetColumn(1).y, rot.GetColumn(1).z);
        Vector3 z = new Vector3(lh_correction * rot.GetColumn(2).x, rot.GetColumn(2).y, rot.GetColumn(2).z);

        updateCylinder(pos, x, _cylinderX);
        updateCylinder(pos, y, _cylinderY);
        updateCylinder(pos, z, _cylinderZ);
    }

    /// <summary>
    /// Set position and offset for a cylinder.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="offset"></param>
    /// <param name="cylinder"></param>
    private void updateCylinder(Vector3 pos, Vector3 offset, GameObject cylinder)
    {
        var length_scale = 5.0f;
        var scale = new Vector3(0.025f, offset.magnitude / length_scale, 0.025f);
        var position = pos + (offset / length_scale);
        position.z = position.z + 0.1f;

        cylinder.transform.position = position;
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
    }

    /// <summary>
    /// Draw accelerations using gizmos.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="acc"></param>
    /// <param name="c"></param>
    public static void DrawAcceleration(Vector3 pos, Vector3 acc, Color c)
    {
        Debug.DrawLine(pos, pos + acc, c);
    }

    /// <summary>
    /// Convert SMPL coordinate frame to Unity. This is necessary because they use
    /// different conventions. SMPL assumes y is up, x is towards the left arm of the body
    /// and z is facing forward.
    /// </summary>
    /// <param name="quat"></param>
    /// <returns></returns>
    public static Quaternion SMPLToUnity(Quaternion quat)
    {
        //testig not mirror
        //return new Quaternion(-quat.x, quat.y, quat.z, -quat.w);
        return new Quaternion(quat.x, -quat.y, -quat.z, quat.w);
    }

    /// <summary>
    /// Convert Unity coordinate frame to SMPL coordinate frame.
    /// </summary>
    /// <param name="quat"></param>
    /// <returns></returns>
    public static Quaternion UnityToSMPL(Quaternion quat)
    {
        return new Quaternion(-quat.x, quat.y, quat.z, -quat.w);
    }
}

/// <summary>
/// Update the pose of a loaded SMPL mesh.
/// </summary>
public class PoseUpdater
{

    private Dictionary<string, int> _boneNameToJointIndex;
    private Dictionary<string, Transform> _boneNameToTransform;
    private Dictionary<string, Quaternion> initBoneOrientation;
    private Dictionary<string, string> _boneneameToSMPLbone;
    private Dictionary<string, Matrix4x4> _boneNameToCoordiate;
    private Dictionary<string, Quaternion> curBoneOrientation;
    private Dictionary<string, Quaternion> nextBoneOrientation;
    public int queueSize = 15;
    private Queue<Dictionary<string, Quaternion>> jointRotationsQueue;
    private DrawingHelper _drawingHelper;

    //we don't use this to get bones now since it might fail in some Mixamo model
    //private SkinnedMeshRenderer targetMeshRenderer;

    private string _boneNamePrefix;
    private string _scope;
    public static readonly int numJoints = 24;

    public static readonly float[] tPose = new float[PoseUpdater.numJoints * 3];

    public float[] currentPose = new float[PoseUpdater.numJoints * 3];
    private bool useSmooth;

    private Transform mSMPLUpdater;
    private Matrix4x4 M_c1c2;

    private List<int> leftArmIndexList;
    private List<int> rightArmIndexList;
    private List<int> leftLegIndexList;
    private List<int> rightLegIndexList;
    private List<int> torsoIndexList;

    private Utlis.AvatarType _avatarType;

    public PoseUpdater(Transform transform, string bonePrefix, Utlis.AvatarType avatarType, bool smooth = false)
    {
        useSmooth = smooth;
        _avatarType = avatarType;
        //targetMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();

        _boneNamePrefix = bonePrefix;
        _boneNameToJointIndex = new Dictionary<string, int>();
        _boneneameToSMPLbone = new Dictionary<string, string>();

        if (_avatarType == Utlis.AvatarType.RocketBox)
        {

            _boneNameToJointIndex.Add("Pelvis", 0);
            _boneNameToJointIndex.Add("L Thigh", 1);
            _boneNameToJointIndex.Add("R Thigh", 2);
            _boneNameToJointIndex.Add("Spine", 3);
            _boneNameToJointIndex.Add("L Calf", 4);
            _boneNameToJointIndex.Add("R Calf", 5);
            _boneNameToJointIndex.Add("Spine1", 6);
            _boneNameToJointIndex.Add("L Foot", 7);
            _boneNameToJointIndex.Add("R Foot", 8);
            _boneNameToJointIndex.Add("Spine2", 9);
            _boneNameToJointIndex.Add("L Toe0", 10);
            _boneNameToJointIndex.Add("R Toe0", 11);
            _boneNameToJointIndex.Add("Neck", 12);
            _boneNameToJointIndex.Add("L Clavicle", 13);
            _boneNameToJointIndex.Add("R Clavicle", 14);
            _boneNameToJointIndex.Add("Head", 15);
            _boneNameToJointIndex.Add("L UpperArm", 16);
            _boneNameToJointIndex.Add("R UpperArm", 17);
            _boneNameToJointIndex.Add("L Forearm", 18);
            _boneNameToJointIndex.Add("R Forearm", 19);
            _boneNameToJointIndex.Add("L Hand", 20);
            _boneNameToJointIndex.Add("R Hand", 21);
            //_boneNameToJointIndex.Add("L_Hand", 22);
            //_boneNameToJointIndex.Add("R_Hand", 23);


            _boneneameToSMPLbone.Add("Pelvis", "Pelvis");
            _boneneameToSMPLbone.Add("L Thigh", "L_Hip");
            _boneneameToSMPLbone.Add("R Thigh", "R_Hip");
            _boneneameToSMPLbone.Add("Spine", "Spine1");
            _boneneameToSMPLbone.Add("L Calf", "L_Knee");
            _boneneameToSMPLbone.Add("R Calf", "R_Knee");
            _boneneameToSMPLbone.Add("Spine1", "Spine2");
            _boneneameToSMPLbone.Add("L Foot", "L_Ankle");
            _boneneameToSMPLbone.Add("R Foot", "R_Ankle");
            _boneneameToSMPLbone.Add("Spine2", "Spine3");
            _boneneameToSMPLbone.Add("L Toe0", "L_Foot");
            _boneneameToSMPLbone.Add("R Toe0", "R_Foot");
            _boneneameToSMPLbone.Add("Neck", "Neck");
            _boneneameToSMPLbone.Add("L Clavicle", "L_Collar");
            _boneneameToSMPLbone.Add("R Clavicle", "R_Collar");
            _boneneameToSMPLbone.Add("Head", "Head");
            _boneneameToSMPLbone.Add("L UpperArm", "L_Shoulder");
            _boneneameToSMPLbone.Add("R UpperArm", "R_Shoulder");
            _boneneameToSMPLbone.Add("L Forearm", "L_Elbow");
            _boneneameToSMPLbone.Add("R Forearm", "R_Elbow");
            _boneneameToSMPLbone.Add("L Hand", "L_Wrist");
            _boneneameToSMPLbone.Add("R Hand", "R_Wrist");
        }


        leftArmIndexList = new List<int>() {13, 16, 18, 20 };
        rightArmIndexList = new List<int>() { 14, 17, 19, 21 };
        leftLegIndexList = new List<int>() { 1, 4, 7, 10 };
        rightLegIndexList = new List<int>() { 2, 5, 8, 11 };
        torsoIndexList = new List<int>() { 0, 3, 6, 9, 12, 15 };


        _boneNameToTransform = new Dictionary<string, Transform>();
        initBoneOrientation = new Dictionary<string, Quaternion>();
        foreach (var item in _boneNameToJointIndex)
        {
            var _boneName = _boneNamePrefix + item.Key;
            Transform t = transform.FindChildRecursive(_boneName);
            if (t != null)
            {
                _boneNameToTransform.Add(item.Key, t);
                initBoneOrientation.Add(item.Key, t.localRotation);
            }
            else {
                Debug.Log(_boneName + "can't find gameobject");
            }

        }

        //testing interpolate
        curBoneOrientation = new Dictionary<string, Quaternion>();
        nextBoneOrientation = new Dictionary<string, Quaternion>();

        this.jointRotationsQueue = new Queue<Dictionary<string, Quaternion>>();
        

        // create cylinders to draw rotation axes if required
        _drawingHelper = new DrawingHelper();


        //debug
        DrawBoneOrientations();
    }

    public void initQueueWithStartPose(float[] startPose)
    {
        Dictionary<string, Quaternion> startJointRotations = new Dictionary<string, Quaternion>();
        computePoseForQueue(startPose, ref startJointRotations);

        for (int i = 1; i < this.queueSize; i += 1)
        {
            this.jointRotationsQueue.Enqueue(startJointRotations);
        }
    }

    private Dictionary<string, Quaternion> SmoothFilter(Queue<Dictionary<string, Quaternion>> jointsQaternionsQueue, Dictionary<string, Quaternion> lastMedians)
    {
        Dictionary<string, Quaternion> medianjointsQaternions = new Dictionary<string, Quaternion>();

        foreach (Dictionary<string, Quaternion> jointsQaternions in jointsQaternionsQueue)
        {
            foreach (KeyValuePair<string, Quaternion> bone in jointsQaternions)
            {
                Quaternion median = new Quaternion(0, 0, 0, 0);
                bool found = false;
                found = medianjointsQaternions.TryGetValue(bone.Key, out median);

                Quaternion lastMedian = lastMedians[bone.Key];
                float weight = 1 - (Quaternion.Dot(lastMedian, bone.Value) / (Mathf.PI / 2)); // 0 degrees of difference => weight 1. 180 degrees of difference => weight 0.
                Quaternion weightedQuaternion = Quaternion.Lerp(lastMedian, bone.Value, weight);

                median.x += weightedQuaternion.x;
                median.y += weightedQuaternion.y;
                median.z += weightedQuaternion.z;
                median.w += weightedQuaternion.w;
                if (!found)
                {
                    medianjointsQaternions.Add(bone.Key, median);
                }

            }
        }

        List<string> keys = new List<string>(medianjointsQaternions.Keys);
        foreach (string key in keys)
        {
            Quaternion median = medianjointsQaternions[key];
            median.x /= jointsQaternionsQueue.Count;
            median.y /= jointsQaternionsQueue.Count;
            median.z /= jointsQaternionsQueue.Count;
            median.w /= jointsQaternionsQueue.Count;
            NormalizeQuaternion(median);
            medianjointsQaternions[key] = median;
        }

        return medianjointsQaternions;
    }

    public Quaternion NormalizeQuaternion(Quaternion quaternion)
    {
        float x = quaternion.x, y = quaternion.y, z = quaternion.z, w = quaternion.w;
        float length = 1.0f / (w * w + x * x + y * y + z * z);
        return new Quaternion(x * length, y * length, z * length, w * length);
    }

    public void computeAndSetSmoothCurBoneOrientation(float[] pose)
    {
        Dictionary<string, Quaternion> unSmoothCurJointRotations = new Dictionary<string, Quaternion>();
        computePoseForQueue(pose, ref unSmoothCurJointRotations);
        jointRotationsQueue.Enqueue(unSmoothCurJointRotations);
        curBoneOrientation = SmoothFilter(this.jointRotationsQueue, getCurBoneOrientation());
        this.jointRotationsQueue.Dequeue();

        setSmoothBoneOrientation(curBoneOrientation);

    }

    public void computeNextSmoothCurBoneOrientation(float[] pose)
    {
        Dictionary<string, Quaternion> unSmoothNextJointRotations = new Dictionary<string, Quaternion>();
        computePoseForQueue(pose, ref unSmoothNextJointRotations);
        jointRotationsQueue.Enqueue(unSmoothNextJointRotations);
        nextBoneOrientation = SmoothFilter(this.jointRotationsQueue, getCurBoneOrientation());
        this.jointRotationsQueue.Dequeue();
    }

    public void saveCurBoneOrientation()
    {
        curBoneOrientation.Clear();
        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            curBoneOrientation.Add(bone.Key, bone.Value.localRotation);
        }
       
    }

    public Dictionary<string, Quaternion> getCurBoneOrientation()
    {
        Dictionary<string, Quaternion> curBoneOrientationDic = new Dictionary<string, Quaternion>();
        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            curBoneOrientationDic.Add(bone.Key, bone.Value.localRotation);
        }
        return curBoneOrientationDic;
    }

    /// <summary>
    /// Get global bone orientation of a given bone.
    /// </summary>
    /// <param name="boneName"></param>
    /// <returns></returns>
    public Quaternion GetGlobalBoneOrientation(string boneName)
    {
        return DrawingHelper.UnityToSMPL(_boneNameToTransform[boneName].rotation);
    }

    public Quaternion ConvertBoneOrientation(Quaternion qSMPL, string boneName)
    {
        int idx;
        Quaternion q_converted = Quaternion.identity;
        if (_boneNameToJointIndex.TryGetValue(boneName, out idx))
        {
            
            if (leftArmIndexList.Contains(idx))
            {
                if (_avatarType == Utlis.AvatarType.RocketBox)
                {
                    // x_c1 = x_c2, y_c1 = -z_c2, z_c1 = y_c2
                    q_converted = new Quaternion(qSMPL.x, -qSMPL.z, qSMPL.y, qSMPL.w);
                }
            }
            else if (rightArmIndexList.Contains(idx))
            {
                if (_avatarType == Utlis.AvatarType.RocketBox)
                {
                    // x_c1 = -x_c2, y_c1 = -z_c2, z_c1 = -y_c2
                    q_converted = new Quaternion(-qSMPL.x, -qSMPL.z, -qSMPL.y, qSMPL.w);
                }                         
            }
            else if (leftLegIndexList.Contains(idx))
            {
                if (_avatarType == Utlis.AvatarType.RocketBox)
                {
                    // x_c1 = y_c2, y_c1 = z_c2, z_c1 = x_c2
                    q_converted = new Quaternion(qSMPL.y, qSMPL.z, qSMPL.x, qSMPL.w);
                }             
            }
            else if (rightLegIndexList.Contains(idx))
            {
                if (_avatarType == Utlis.AvatarType.RocketBox)
                {
                    // x_c1 = y_c2, y_c1 = z_c2, z_c1 = x_c2
                    q_converted = new Quaternion(qSMPL.y, qSMPL.z, qSMPL.x, qSMPL.w);
                }
                
            }
            else if (torsoIndexList.Contains(idx))
            {
                if (_avatarType == Utlis.AvatarType.RocketBox)
                {
                    // x_c1 = -y_c2, y_c1 = z_c2, z_c1 = -x_c2
                    q_converted = new Quaternion(-qSMPL.y, qSMPL.z, -qSMPL.x, qSMPL.w);

                    //Pelvis correction
                    if (idx == 0)
                    {
                        Quaternion rotationAmount = Quaternion.Euler(0, 0, 0);
                        q_converted = q_converted * rotationAmount;                      
                    }

                }


            }
            return q_converted;
        }
        else
        {
            Debug.Log("CovertBoneOrientation fail");
            return q_converted;
        }
        
        
    }

    public void DrawBoneOrientations()
    {
        _boneNameToCoordiate = new Dictionary<string, Matrix4x4>();

        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            Vector3 x = bone.Value.right * 0.2f;
            Vector3 y = bone.Value.up * 0.2f;
            Vector3 z = bone.Value.forward * 0.2f;
            Vector3 pos = bone.Value.position;
            
            //uncomment for debugging
            //Debug.DrawLine(pos, pos + x, Color.red, 1000);
            //Debug.DrawLine(pos, pos + y, Color.green, 1000);
            //Debug.DrawLine(pos, pos + z, Color.blue, 1000);

            Vector3 x_tmp = Vector3.Normalize(x);
            Vector3 y_tmp = Vector3.Normalize(y);
            Vector3 z_tmp = Vector3.Normalize(z);
            Vector4 c1 = new Vector4(x_tmp.x, x_tmp.y, x_tmp.z, 0);
            Vector4 c2 = new Vector4(y_tmp.x, y_tmp.y, y_tmp.z, 0);
            Vector4 c3 = new Vector4(z_tmp.x, z_tmp.y, z_tmp.z, 0);
            Vector4 c4 = new Vector4(0, 0, 0, 1);

            Matrix4x4 boneCoordinate = new Matrix4x4(c1, c2, c3, c4);
            _boneNameToCoordiate.Add(bone.Key, boneCoordinate); //not used anymore

        }
            
    }

    /// <summary>
    /// Set the orientation of the specified bone to the given orientation.
    /// </summary>
    /// <param name="boneName"></param>
    /// <param name="quat"></param>
    public void setBoneOrientation(string boneName, Quaternion quat)
    {
        Transform trans;
        if (_boneNameToTransform.TryGetValue(boneName, out trans))
        {
            trans.rotation = DrawingHelper.SMPLToUnity(quat);
        }
        else
        {
            Debug.LogError("ERROR: no game object for given bone name: " + boneName);
        }
    }


    public Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }

    public void interPose(float fraction)
    {
        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            if (curBoneOrientation.ContainsKey(bone.Key))
            {
                bone.Value.localRotation = Quaternion.Slerp(curBoneOrientation[bone.Key], nextBoneOrientation[bone.Key], fraction);
            }
            else
            {
                //Debug.Log("key not found:" + bone.Key);
            }
                           
        }
    }
    public void computePoseForQueue(float[] iPose, ref Dictionary<string, Quaternion> iJointRotations)
    {
        iJointRotations.Clear();
        Quaternion quat;
        int pelvisIndex = -1;
        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            int index;

            if (_boneNameToJointIndex.TryGetValue(bone.Key, out index))
            {
                if (index == 0)
                {
                    pelvisIndex = index;
                }

                int idx = index * 3;
                Vector3 rot = new Vector3(iPose[idx], iPose[idx + 1], iPose[idx + 2]);
                float angle = rot.magnitude * Mathf.Rad2Deg;


                quat = Quaternion.AngleAxis(angle, rot.normalized);
                Quaternion boneQuat = DrawingHelper.SMPLToUnity(quat);



                if (_avatarType == Utlis.AvatarType.SMPL)
                {
                    //bone.Value.localRotation = boneQuat;
                    iJointRotations.Add(bone.Key, boneQuat);
                }
                else
                {
                    Quaternion initQuat;
                    if (initBoneOrientation.TryGetValue(bone.Key, out initQuat))
                    {
                        Quaternion convertedRot = ConvertBoneOrientation(boneQuat, bone.Key);
                        //bone.Value.localRotation = initQuat * convertedRot;
                        Quaternion nextlocalRotation = initQuat * convertedRot;
                        iJointRotations.Add(bone.Key, nextlocalRotation);

                    }
                    else
                    {
                        Debug.Log(bone.Key + " no init rot");
                    }

                }

            }
            else
            {
                //Debug.LogError("ERROR: No joint index for given bone name: " + boneName);
            }
        }

    }

    public void computeNextPose(float[] nextPose)
    {
        Quaternion quat;
        int pelvisIndex = -1;
        nextBoneOrientation.Clear();

        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            int index;

            if (_boneNameToJointIndex.TryGetValue(bone.Key, out index))
            {
                if (index == 0)
                {
                    pelvisIndex = index;
                }

                int idx = index * 3;
                Vector3 rot = new Vector3(nextPose[idx], nextPose[idx + 1], nextPose[idx + 2]);
                float angle = rot.magnitude * Mathf.Rad2Deg;


                quat = Quaternion.AngleAxis(angle, rot.normalized);
                Quaternion boneQuat = DrawingHelper.SMPLToUnity(quat);



                if (_avatarType == Utlis.AvatarType.SMPL)
                {
                    //bone.Value.localRotation = boneQuat;
                    nextBoneOrientation.Add(bone.Key, boneQuat);
                }
                else
                {
                    Quaternion initQuat;
                    if (initBoneOrientation.TryGetValue(bone.Key, out initQuat))
                    {
                        Quaternion convertedRot = ConvertBoneOrientation(boneQuat, bone.Key);
                        //bone.Value.localRotation = initQuat * convertedRot;
                        Quaternion nextlocalRotation = initQuat * convertedRot;
                        nextBoneOrientation.Add(bone.Key, nextlocalRotation);

                    }
                    else
                    {
                        Debug.Log(bone.Key + " no init rot");
                    }

                }

            }
            else
            {
                //Debug.LogError("ERROR: No joint index for given bone name: " + boneName);
            }
        }
    }

    public void setSmoothBoneOrientation(Dictionary<string, Quaternion> smoothBoneOrientations)
    {
        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            bone.Value.localRotation = smoothBoneOrientations[bone.Key];
        }
    }


    /// <summary>
    /// Set a new pose parametrized as a 72 dimensional vector.
    /// </summary>
    /// <param name="pose"></param>
    public void setNewPose(float[] pose)
    {

        currentPose = pose;
        Quaternion quat;
        int pelvisIndex = -1;

        foreach (KeyValuePair<string, Transform> bone in _boneNameToTransform)
        {
            int index;

            if (_boneNameToJointIndex.TryGetValue(bone.Key, out index))
            {
                if (index == 0)
                {
                    pelvisIndex = index;
                }

                int idx = index * 3;
                Vector3 rot = new Vector3(pose[idx], pose[idx + 1], pose[idx + 2]);
                float angle = rot.magnitude * Mathf.Rad2Deg;
              

                quat = Quaternion.AngleAxis(angle, rot.normalized);
                Quaternion boneQuat = DrawingHelper.SMPLToUnity(quat);


                if (_avatarType == Utlis.AvatarType.SMPL)
                {
                    bone.Value.localRotation = boneQuat;
                }
                else
                {
                    Quaternion initQuat;
                    if (initBoneOrientation.TryGetValue(bone.Key, out initQuat))
                    {
                        Quaternion convertedRot = ConvertBoneOrientation(boneQuat, bone.Key);
                        bone.Value.localRotation = initQuat * convertedRot;

                    }
                    else
                    {
                        Debug.Log(bone.Key + " no init rot");
                    }

                }

            }
            else
            {
                //Debug.LogError("ERROR: No joint index for given bone name: " + boneName);
            }
        }

        saveCurBoneOrientation();
    }

}