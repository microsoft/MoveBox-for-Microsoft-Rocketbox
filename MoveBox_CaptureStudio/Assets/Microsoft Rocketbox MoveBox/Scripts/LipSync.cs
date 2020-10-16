using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine.UIElements;



[CustomEditor(typeof(LipSync))]
public class LipSyncEditor : Editor
{
    public override void OnInspectorGUI()
    {
       // DrawDefaultInspector();
    }
}

public class LipSync : MonoBehaviour
{

    public string nameOfMicrophone; // if null is passed for the name of the microphone, the default mic will be used
    private const int LENGTH = 10;
    float[] samples;
    private bool micPresent=false, filePresent=false;
   // public bool isRecording;
    private Transform rootBone;
    public float currentAverageVolume;
    public float silenceVolume;
    public bool isTalking = false, captureSilence = false, restart=false,realtimeMoCap=false;
    private bool startplaying= false;
    DateTime lastChange;
    //temporary audio vector we write to every second while recording is enabled..
    List<float> tempRecording = new List<float>();

    //full recorded clip...
    float[] fullClip;

    [Range(0, 1.0f)]
    public float VOLUME_THRESHOLD = 30f;

    public AudioSource audioSource;
    GameObject JawBone, LipBone, NoseBone;
    Vector3 closeMouthJaw, mouthJaw;
    Vector3 mouthLip, closeMouthLip;
    float openMouthLip,openMouthJaw;

    private float VOLUME_MULTIPLIER = 1;
    public float OPEN_FACTOR = 1;
    private int DELAY = 500;


    private float CURRENT_LIP = 0f;
    private float CURRENT_JAW = 0f;

    private float JAW_OPEN_FACTOR = 0.5f;
    private float LIP_FACTOR = 0.01f;
    private float JAW_CLOSE_SPEED = 0.3f;


    // Start is called before the first frame update
    void Start()
    {

    }
     public void  StartAfterConfig( AudioClip audio, GameObject root){

        rootBone = root.transform;
        if (!rootBone.parent.parent.gameObject.TryGetComponent<AudioSource>(out audioSource))
            audioSource = rootBone.parent.parent.gameObject.AddComponent<AudioSource>();
        

        lastChange = DateTime.Now;

        if (audio == null)
        {
            OPEN_FACTOR = 10;
            DELAY = 500;
            audioSource.outputAudioMixerGroup = Resources.Load<AudioMixerGroup>("LipSync");
            OpenMic();
        }
        else
        {
            OPEN_FACTOR = 1.2f;
            DELAY = 1000;
            audioSource.clip = audio;
            filePresent = true;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        JawBone = rootBone.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").Find("Bip01 MJaw").gameObject;
        LipBone = rootBone.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").Find("Bip01 MUpperLip").gameObject;
        NoseBone = rootBone.Find("Bip01 Spine").Find("Bip01 Spine1").Find("Bip01 Spine2").Find("Bip01 Neck").Find("Bip01 Head").Find("Bip01 MNose").gameObject;
        closeMouthJaw = JawBone.transform.localPosition; //JawBone.transform.localRotation;
        closeMouthLip = LipBone.transform.localPosition;
        openMouthLip = -(NoseBone.transform.position.y - LipBone.transform.position.y) / 7;
        openMouthJaw = (NoseBone.transform.position.y - LipBone.transform.position.y)/3;
    }

    void OpenMic()
    {
        if (Microphone.devices.GetLength(0) < 1)
        {
            micPresent = false;
            Debug.Log("Error: there is no available microphone device: LipSync cannot run");
        }
        else
        {

            string[] mics = Microphone.devices;
            int minFreq = 0;
            int maxFreq = 0;
            foreach (string mic in mics)
            {
                // Display Details for Microphones attached to system //
                Microphone.GetDeviceCaps(mic, out minFreq, out maxFreq);
                Debug.Log("Microphone available: \n deviceName: " + mic + "\n frequency: " + minFreq + "\n frequency: " + maxFreq);
                if (!mic.Contains("Azure") && !mic.Contains("Xbox"))
                {
                    if (nameOfMicrophone == "")
                    {
                        nameOfMicrophone = mic;
                    }
                }
                if (nameOfMicrophone == mic)
                {
                    Debug.Log("Microphone being used: \n deviceName: " + mic + "\n frequency: " + minFreq);

                }
            }
            micPresent = true;
            audioSource.clip = Microphone.Start(nameOfMicrophone, true, 1, AudioSettings.outputSampleRate); // if null is passed for the name of the microphone, the default mic will be used
            audioSource.loop = true; 
            
        }
    }

    void stopRecording()
    {
        int length = Microphone.GetPosition(nameOfMicrophone);
        float[] clipData = new float[length];
        audioSource.clip.GetData(clipData, 0);
        float[] fullClip = new float[clipData.Length + tempRecording.Count];
        for (int i = 0; i < fullClip.Length; i++)
        {
            //write data all recorded data to fullCLip vector
            if (i < tempRecording.Count)
                fullClip[i] = tempRecording[i];
            else
                fullClip[i] = clipData[i - tempRecording.Count];
        }
    }

    private void Update()
    {
        //to overide the animator we need to keep the original rotations before it came into effect.
      /*  mouthJaw = JawBone.transform.localPosition;
        // mouthliplocal = LipBone.transform.localPosition;
        mouthLip = LipBone.transform.localPosition;*/
        
    }
    // Update is called once per frame
    void LateUpdate()
    {

        if (micPresent)
        {

            if (Microphone.IsRecording(nameOfMicrophone))
            {
                int length = Microphone.GetPosition(nameOfMicrophone);
                if (length == 0 && !startplaying)
                {

                    return;
                }
                else
                {
                    if (!startplaying)
                    {
                        startplaying = true;

                        audioSource.Play();
                    }
                }
            }
        }
        if(filePresent && restart)
        {
            audioSource.Stop();
            audioSource.Play();
            restart = false;
        }
        if (micPresent || filePresent)
        {

            samples = new float[1024];

            audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);

            currentAverageVolume = samples.Average() * VOLUME_MULTIPLIER;

            if (captureSilence)
            {
                captureSilence = false;
                silenceVolume = currentAverageVolume;
            }



            if (currentAverageVolume > (silenceVolume + silenceVolume * VOLUME_THRESHOLD))
            {
                if (!isTalking) lastChange = DateTime.Now;

                isTalking = true;
            }
            else
            {
                if (isTalking) lastChange = DateTime.Now;
                isTalking = false;
            }
            if ((DateTime.Now - lastChange).TotalMilliseconds >= DELAY & isTalking)
            {
                lastChange = DateTime.Now;
                isTalking = false;
            }
            float next;
            float val;

            
            if (isTalking)
            {
              /*  val = currentAverageVolume * JAW_OPEN_FACTOR * OPEN_FACTOR;
                if (mouthJaw.eulerAngles.z + CURRENT_JAW - val > closeMouthJaw.eulerAngles.z - JAW_OPEN)
                    next= CURRENT_JAW - val;
                else
                    next = - JAW_OPEN;

                JawBone.transform.localRotation = Quaternion.Lerp(closeMouthJaw * Quaternion.Euler(new Vector3(0, 0, CURRENT_JAW)),
                        closeMouthJaw * Quaternion.Euler(new Vector3(0, 0, next)), 10f);

                CURRENT_JAW = next;
              */

                val = currentAverageVolume * JAW_OPEN_FACTOR * OPEN_FACTOR;
                if (closeMouthJaw.x + CURRENT_JAW + val < closeMouthJaw.x + openMouthJaw)
                    next = CURRENT_JAW + val;
                else
                    next = openMouthJaw;

                JawBone.transform.localPosition = Vector3.Lerp(closeMouthJaw + new Vector3(CURRENT_JAW, 0,0),
                    closeMouthJaw + new Vector3(next,0,0), 10f);

                CURRENT_JAW = next;

                val = currentAverageVolume * LIP_FACTOR;
                if (closeMouthLip.x + CURRENT_LIP - val > closeMouthLip.x + openMouthLip)
                    next = CURRENT_LIP - val;
                else
                    next = openMouthLip;

                LipBone.transform.localPosition = Vector3.Lerp(closeMouthLip + new Vector3(CURRENT_LIP, 0, 0), 
                    closeMouthLip+ new Vector3(next, 0,0), 10f);

                CURRENT_LIP = next;
            }
            else
            {
              /*  val = JAW_CLOSE_SPEED;
                if ( CURRENT_JAW + val <= 0)
                {
                    Debug.Log("here");
                    next = CURRENT_JAW + val;
                }
                else
                    next = 0;

                JawBone.transform.localRotation = Quaternion.Lerp(closeMouthJaw * Quaternion.Euler(new Vector3(0, 0, CURRENT_JAW)),
                        closeMouthJaw * Quaternion.Euler(new Vector3(0, 0, next)), 10f);

                CURRENT_JAW = next;*/
                val = JAW_CLOSE_SPEED;
                if (CURRENT_JAW - val >= 0)
                {
                    next = CURRENT_JAW - val;
                }
                else
                    next = 0;

                JawBone.transform.localPosition = Vector3.Lerp(closeMouthJaw + new Vector3(CURRENT_JAW,0, 0),
                    closeMouthJaw + new Vector3(next,0,0), 10f);
                CURRENT_JAW = next;

                val = LIP_FACTOR;
                if (CURRENT_LIP + val <= 0)
                {
                    next = CURRENT_LIP + val;
                }
                else
                    next = 0;

              
                LipBone.transform.localPosition = Vector3.Lerp(closeMouthLip + new Vector3(CURRENT_LIP, 0, 0),
                    closeMouthLip + new Vector3(next, 0, 0), 10f);  
                CURRENT_LIP = next;

            }

            /*        samples = new float[length];
                    audioSource.clip.GetData(samples, 0);

                    if (isRecording)
                    {
                        float[] clipData = new float[length];
                        audioSource.clip.GetData(clipData, 0);
                        tempRecording.AddRange(clipData);
                    }

                    //if you want to do something with the voice, use this
                    for (var i = 0; i < length; i++)
                    {
                        samples[i] = 0; //for example this line will make the volume higher or lower (adjust the power value to change it)
                    }

                    audioSource.clip.SetData(samples, 0);*/
        }
        
    }


    private void OnDestroy()
    {
        if (micPresent)
        {
            Microphone.End(nameOfMicrophone); // if null is passed for the name of the microphone, the default mic will be used
        }
    }
}
