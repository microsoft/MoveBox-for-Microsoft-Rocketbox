using System.Timers;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;


[CustomEditor(typeof(EyeBlink))]
public class EyeBlinkEditor : Editor
{
    public override void OnInspectorGUI()
    {
      //  DrawDefaultInspector();
    }
}

public class EyeBlink : MonoBehaviour
{
    public bool blinkOn;
    public bool blinkTriggered = false;
    public bool blinkClosing=true;

    

    private static System.Timers.Timer blinkTime;

    private const int MIN_TIME_BETWEEN_BLINKS = 3000, MAX_TIME_BETWEEN_BLINKS = 8000;

    private Transform rootBone;
    GameObject L_UpperLidBone, R_UpperLidBone, L_LowerLidBone, R_LowerLidBone;
    Vector3 L_UpperLidOpen, R_UpperLidOpen, L_LowerLidOpen, R_LowerLidOpen;

    private const float UPPER_BLINK_SPEED_FACTOR = 0.16f;
    private const float LOWER_BLINK_SPEED_FACTOR = 0.1f * UPPER_BLINK_SPEED_FACTOR;
    private float cheekToEyeLen;
    private float upperBlinkSpeed;
    private float lowerBlinkSpeed;
    private const float CLOSING_INTERPOLANT = 1f;
    private const float OPENING_INTERPOLANT = 0.1f;

    public void StartAfterConfig(GameObject root)
    {
        //set timer
        SetTimer();
        rootBone = root.transform;

        L_UpperLidBone = FindFaceBone("Bip01 LEyeBlinkTop");
        R_UpperLidBone = FindFaceBone("Bip01 REyeBlinkTop");
        L_LowerLidBone = FindFaceBone("Bip01 LEyeBlinkBottom");
        R_LowerLidBone = FindFaceBone("Bip01 REyeBlinkBottom");

        L_UpperLidOpen = L_UpperLidBone.transform.localPosition;
        R_UpperLidOpen = R_UpperLidBone.transform.localPosition;
        L_LowerLidOpen = L_LowerLidBone.transform.localPosition;
        R_LowerLidOpen = R_LowerLidBone.transform.localPosition;

        Vector3 lEyePos = FindFaceBone("Bip01 LEye").transform.position;
        Vector3 lCheekPos = FindFaceBone("Bip01 LCheek").transform.position;
        cheekToEyeLen = Vector3.Distance(lCheekPos, lEyePos);
        // constants for how much to open eye are relative to distance between cheeck and eye bone
        upperBlinkSpeed = UPPER_BLINK_SPEED_FACTOR * cheekToEyeLen;
        lowerBlinkSpeed = LOWER_BLINK_SPEED_FACTOR * cheekToEyeLen;
    }

    // Update is called once per frame
    private void Update()
    {
        //closing the eye lower lid negative
        // closing the uper lid positive
        if (!blinkTriggered)
            return;
        
        if (blinkClosing)
        {
            UpdateAllLids(1, CLOSING_INTERPOLANT);
            /* replaced by method:
            L_UpperLidBone.transform.localPosition = Vector3.Lerp(L_UpperLidBone.transform.localPosition, L_UpperLidBone.transform.localPosition + new Vector3(upperBlinkSpeed, 0f, 0f), CLOSING_INTERPOLANT);
            R_UpperLidBone.transform.localPosition = Vector3.Lerp(R_UpperLidBone.transform.localPosition, R_UpperLidBone.transform.localPosition + new Vector3(upperBlinkSpeed, 0f, 0f), CLOSING_INTERPOLANT);
            L_LowerLidBone.transform.localPosition = Vector3.Lerp(L_LowerLidBone.transform.localPosition, L_LowerLidBone.transform.localPosition - new Vector3(lowerBlinkSpeed, 0f, 0f), CLOSING_INTERPOLANT);
            R_LowerLidBone.transform.localPosition = Vector3.Lerp(R_LowerLidBone.transform.localPosition, R_LowerLidBone.transform.localPosition - new Vector3(lowerBlinkSpeed, 0f, 0f), CLOSING_INTERPOLANT);
            */
        }
        else
        {
            UpdateAllLids(-1, OPENING_INTERPOLANT);
            /* replaced by method:
            L_UpperLidBone.transform.localPosition = Vector3.Lerp(L_UpperLidBone.transform.localPosition, L_UpperLidBone.transform.localPosition - new Vector3(upperBlinkSpeed, 0f, 0f), OPENING_INTERPOLANT);
            R_UpperLidBone.transform.localPosition = Vector3.Lerp(R_UpperLidBone.transform.localPosition, R_UpperLidBone.transform.localPosition - new Vector3(upperBlinkSpeed, 0f, 0f), OPENING_INTERPOLANT);
            L_LowerLidBone.transform.localPosition = Vector3.Lerp(L_LowerLidBone.transform.localPosition, L_LowerLidBone.transform.localPosition + new Vector3(lowerBlinkSpeed, 0f, 0f), OPENING_INTERPOLANT);
            R_LowerLidBone.transform.localPosition = Vector3.Lerp(R_LowerLidBone.transform.localPosition, R_LowerLidBone.transform.localPosition + new Vector3(lowerBlinkSpeed, 0f, 0f), OPENING_INTERPOLANT);
            */
        }

        if (blinkClosing && (L_UpperLidBone.transform.position.y < L_LowerLidBone.transform.position.y))
        {
            blinkClosing = false;
            return;
        }
        
        if (!blinkClosing && L_UpperLidBone.transform.localPosition.x <= L_UpperLidOpen.x)
        {
            L_UpperLidBone.transform.localPosition = L_UpperLidOpen;
            L_LowerLidBone.transform.localPosition = L_LowerLidOpen;
            R_UpperLidBone.transform.localPosition = R_UpperLidOpen;
            R_LowerLidBone.transform.localPosition = R_LowerLidOpen;
            blinkTriggered = false;
            blinkClosing = true;
            return;
        }

    }

    private void SetTimer()
    {
        System.Random r = new System.Random();
        float time = r.Next(MIN_TIME_BETWEEN_BLINKS, MAX_TIME_BETWEEN_BLINKS);
        blinkTime = new System.Timers.Timer(time);
        blinkTime.Elapsed += OnTimedEvent;
        blinkTime.AutoReset = false;
        blinkTime.Enabled = true;
    }
    private void OnTimedEvent(object sender, ElapsedEventArgs elapseEventArg)
    {
        SetTimer();
        //trigger eye closing
        blinkTriggered = true;
    }

    private GameObject FindFaceBone(string boneName)
    {
        return rootBone.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").Find(boneName).gameObject;
    }

    private void UpdateAllLids(int direction, float interpolant)
    {
        UpdateLid(L_UpperLidBone, upperBlinkSpeed * direction, interpolant);
        UpdateLid(R_UpperLidBone, upperBlinkSpeed * direction, interpolant);
        UpdateLid(L_LowerLidBone, -lowerBlinkSpeed * direction, interpolant);
        UpdateLid(R_LowerLidBone, -lowerBlinkSpeed * direction, interpolant);
    }
    private void UpdateLid(GameObject bone, float posChange, float interpolant)
    {
        bone.transform.localPosition = Vector3.Lerp(bone.transform.localPosition, bone.transform.localPosition + new Vector3(posChange, 0f, 0f), interpolant);
    }

    private void OnDestroy()
    {
        if (blinkTime != null)
        {
            blinkTime.Stop();
            blinkTime.Dispose();
        }
    }
}

