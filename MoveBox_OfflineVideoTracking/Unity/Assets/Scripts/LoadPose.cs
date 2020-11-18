using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPose: MonoBehaviour
{
    
    public float frameRate = 30.0f;
    private List<Dictionary<string, object>> data;
    private int maxFrame = 0;
    private float[] currentPose;
    private float[] nextPose;
    private int currentFrame;
    private int numCols;
    private int nextFrame;
    public bool startRender = false;
    private PoseUpdater _poseUpdater;
    //bonePrefix for MS RocketBox Avatar is always "bip01 "
    private string bonePrefix = "Bip01 ";
    private Utlis.AvatarType avatarType = Utlis.AvatarType.RocketBox;
    public string poseFile = "smpl_pose";
    private int queueSize = 5;
    private bool useSmooth = true;
    private bool useInterp = true;
    private float t = 0;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        _poseUpdater = new PoseUpdater(this.transform, bonePrefix, avatarType, useSmooth);
        _poseUpdater.queueSize = this.queueSize;
        data = CSVReader.Read(poseFile);
        numCols = data[0].Count;
        currentPose = new float[72];
        nextPose = new float[72];
        currentFrame = 0;
        nextFrame = 1;
        maxFrame = data.Count - 1;

        float[] startPose = new float[72];
        for (int i = 0; i < 72; i++)
        {
            if (numCols == 73)
            {
                startPose[i] = Convert.ToSingle(data[0][(i + 1).ToString()]);
            }
            else
            {
                startPose[i] = Convert.ToSingle(data[0][(i).ToString()]);
            }
        }
        _poseUpdater.initQueueWithStartPose(startPose);        

    }

    // Update is called once per frame
    void Update()
    {

        t += Time.deltaTime;
        if (t >= 1.0f / frameRate)
        {
            updatePose();

            t = 0;
        }
        else
        {
            // interpolate fraction
            if (useInterp)
            {
                float fFraction = t / (1.0f / frameRate);
                if (startRender)
                {
                    _poseUpdater.interPose(fFraction);
                }
            }
            
        }

    }

    private void updatePose()
    {
        if (startRender)
        {
            //support both csv with and without frameid
            for (int i = 0; i < 72; i++)
            {
                if (numCols == 73)
                {
                    currentPose[i] = Convert.ToSingle(data[currentFrame][(i + 1).ToString()]);
                }
                else
                {
                    currentPose[i] = Convert.ToSingle(data[currentFrame][(i).ToString()]);
                }
            }
            if (useSmooth)
            {
                _poseUpdater.computeAndSetSmoothCurBoneOrientation(currentPose);
            }
            else
            {
                _poseUpdater.setNewPose(currentPose);
            }

            // Get nexframe for interpolate
            nextFrame = currentFrame + 1;
            if (nextFrame > maxFrame)
            {
                nextFrame = 0;
            }
            for (int i = 0; i < 72; i++)
            {

                if (numCols == 73)
                {
                    nextPose[i] = Convert.ToSingle(data[nextFrame][(i + 1).ToString()]);
                }
                else
                {
                    nextPose[i] = Convert.ToSingle(data[nextFrame][(i).ToString()]);
                }

            }
            if (useSmooth)
            {
                _poseUpdater.computeNextSmoothCurBoneOrientation(nextPose);
            }
            else
            {
                _poseUpdater.computeNextPose(nextPose);
            }
            
            currentFrame++;

            if (currentFrame > maxFrame)
            {
                currentFrame = 0;
            }
        }
    }
}
