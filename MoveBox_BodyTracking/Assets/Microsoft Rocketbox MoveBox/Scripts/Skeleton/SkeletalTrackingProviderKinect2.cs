using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using Windows.Kinect;
using Kinect = Windows.Kinect;
using UnityEngine;

public class SkeletalTrackingProviderKinect2 : BackgroundDataProvider
{
    bool readFirstFrame = false;
    TimeSpan initialTimestamp;

    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter { get; set; } = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    public Stream RawDataLoggingFile = null;

    private KinectSensor sensor;
    private BodyFrameReader bodyFrameReader;
    private Windows.Kinect.Body[] bodies;
    public Material BoneMaterial;
    
    protected override void RunBackgroundThreadAsync(int id)
    {
        // Attempt to open Kinect 2 sensor.
        try
        {
            BackgroundData currentFrameData = new BackgroundData();
            sensor = KinectSensor.GetDefault();

            if (sensor != null)
            {
                // Open bodyFrameReader (part of Kinect 2 Sensor)
                bodyFrameReader = sensor.BodyFrameSource.OpenReader();

                if (!sensor.IsOpen)
                {
                    // Open Kinect V2 Sensor.
                    sensor.Open();
                    UnityEngine.Debug.Log("Open Kinect2 device successful.");

                    // Get Frames

                    BodyFrame frame = null;

                    while (m_runBackgroundThread)
                    {
                        if (bodyFrameReader != null)
                        {
                            // Get latest frame from Kinect V2
                            frame = bodyFrameReader.AcquireLatestFrame();

                            if (frame != null)
                            {
                                // Ensure that the thread is set to be running
                                IsRunning = true;

                                if (bodies == null)
                                {
                                    bodies = new Windows.Kinect.Body[sensor.BodyFrameSource.BodyCount];
                                }

                                // Extract frame data
                                currentFrameData.NumOfBodies = (ulong)frame.BodyCount;
                                frame.GetAndRefreshBodyData(bodies);


                                foreach (var body in bodies)
                                {
                                    if (body == null)
                                    {
                                        continue;
                                    }

                                    if (body.IsTracked)
                                    {
                                        //Copy this body to the 'currentFrameData'
                                        currentFrameData.Bodies[0].CopyFromBodyTrackingSdk(body, sensor.CoordinateMapper);
                                        continue;
                                    }
                                }

                                // Time stamp infomation copied from K4A file
                                if (!readFirstFrame)
                                {
                                    readFirstFrame = true;
                                    initialTimestamp = frame.RelativeTime;
                                }

                                currentFrameData.TimestampInMs = (float)(frame.RelativeTime - initialTimestamp).TotalMilliseconds;
                                currentFrameData.DepthImageHeight = 100; //filler
                                currentFrameData.DepthImageWidth = 100; //filler

                                // Discard frame now that we are finished with it.
                                frame.Dispose();
                                frame = null;

                                // Update data variable that is being read in the UI thread.
                                SetCurrentFrameData(ref currentFrameData);
                            }
                        }
                    }
                    // Close Resources once loop is exited.
                    sensor.Close();
                    if (RawDataLoggingFile != null)
                    {
                        RawDataLoggingFile.Close();
                    }
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }
}
