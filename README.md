# MoveBox for Microsoft Rocketbox

MoveBox is a toolbox to animate the Microsoft Rocketbox avatars (https://github.com/microsoft/Microsoft-Rocketbox) using motion captured in three different ways. Each form of avatar animation is presented as a separate project:

### 1) MoveBox Capture Studio
We created a Motion Capture (MoCap) studio using a single depth sensor, such as Azure Kinect or Windows Kinect V2. Our toolbox enables real-time animation of the user's avatar by converting the transformations between systems that have different joints and hierarchies. Additional features of the Captire Studio include recording, playback and looping animations, as well as basic audio lip sync, blinking and resizing of avatars. Our main contribution is both in the creation of this open source tool as well as the integration of MoveBox with Kinect V2 and Azure Kinect, as well as the interfacing with the Microsoft Rocketbox avatars.

### 2) MoveBox IK Hand Tracking
Using an off the shelf HMD we recover realtime motions to create avatar embodiment. The hand and head positions and rotations of the user are transferred to a Microsoft Rocketbox Avatar of your choice. All in realtime. The hand tracking is used for an Inverse Kinematics (IK) solver (included) that reconstructs a possible elbow position. Our demo project also includes finger tracking for the Oculus Quest.

### 3) MoveBox Offline Video Tracking
Movebox includes an external tool for 3D multi-person human pose estimation from RGB videos. We utilized a deep-learning based approach open sourced as VIBE (https://github.com/mkocabas/VIBE), which trains a temporal model to predict the parameters of the SMPL body model for each frame while a motion discriminator tries to distinguish between real and regressed sequences. 
The output of the model is a sequence of pose and shape parameters in the SMPL body model format (https://smpl.is.tue.mpg.de/). To animate Microsoft RocketBox avatars with predicted 3D poses, the toolbox first extracted the joints data from pose parameters, computed the transformation between SMPL and RocketBox skeleton structures, and then mapped to the corresponding joint in the Microsoft RocketBox avatar skeleton.

_Note: Tools from the capture studio (MoveBox) can be imported to project 2 if there is a need to record the motions of the participant or user for later use._

Microsoft Privacy Statement
https://privacy.microsoft.com/en-us/privacystatement

## Reference:
The following paper was published at IEEE AIVR 2020 to coincide with the release of this toolbox,and gives more details of the features included:

_Mar Gonzalez-Franco, Zelia Egan, Matthew Peachey, Angus Antley, Tanmay Randhavane, Payod Panda, Yaying Zhang,  Cheng Yao Wang, Derek F. Reilly, Tabitha C Peck, Andrea Stevenson Won, Anthony Steed and Eyal Ofek (2020) "**MoveBox: Democratizing MoCap for the Microsoft Rocketbox Avatar Library**". IEEE International Conference on Artificial Intelligence and Virtual Reality (AIVR)_

_If you use this library for research or academic purposes, please also cite the aforementioned paper._

## Instructions for MoveBox_CaptureStudio

### Install SDKs for Kinect V2 and/or Azure Kinect
**Kinect V2** (https://developer.microsoft.com/en-us/windows/kinect/)

* Kinect for Windows SDK 2.0
https://www.microsoft.com/en-us/download/details.aspx?id=44561


**Azure Kinect** (https://docs.microsoft.com/en-us/azure/kinect-dk/)

* Azure Kinect Sensor SDK
https://docs.microsoft.com/en-us/azure/kinect-dk/sensor-sdk-download

* Azure Kinect Body Tracking SDK
https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-download


### FOR AZURE KINECT
#### 1) First get the latest nuget packages of libraries:

Open the MoveBox_CaptureStudio project in Unity.
Open the Visual Studio Solution associated with this project "MoveBox_CaptureStudio.sln".
If there is no Visual Studio Solution yet you can make one by opening the Unity Editor
and selecting one of the csharp files in the project and opening it for editing.
You may also need to set the preferences->External Tools to Visual Studio

In Visual Studio:
Select Tools->NuGet Package Manager-> Package Manager Console

On the command line of the console at type the following command:

```bash
Install-Package Microsoft.Azure.Kinect.BodyTracking -Version 1.0.1
```

The body tracking libraries will be put in the Packages folder under MoveBox_CaptureStudio 


#### 2) Next add these libraries to the Assets/Plugins folder:

You can do this by hand or just run the batch file MoveLibraryFile.bat in the MoveBox_CaptureStudio directory


From Packages/Microsoft.Azure.Kinect.BodyTracking.1.0.1/lib/netstandard2.0

- Microsoft.Azure.Kinect.BodyTracking.deps.json
- Microsoft.Azure.Kinect.BodyTracking.xml
- Microsoft.Azure.Kinect.BodyTracking.dll
- Microsoft.Azure.Kinect.BodyTracking.pdb

From Packages/Microsoft.Azure.Kinect.Sensor.1.3.0/lib/netstandard2.0

- Microsoft.Azure.Kinect.Sensor.deps.json
- Microsoft.Azure.Kinect.Sensor.xml
- Microsoft.Azure.Kinect.Sensor.dll
- Microsoft.Azure.Kinect.Sensor.pdb

From Packages/Microsoft.Azure.Kinect.BodyTracking.Dependencies.0.9.1/lib/native/amd64/release
- cublas64_100.dll
- cudart64_100.dll
- vcomp140.dll

From Packages/System.Buffers.4.4.0/lib/netstandard2.0

- System.Buffers.dll

From Packages/System.Memory.4.5.3/lib/netstandard2.0

- System.Memory.dll

From Packages/System.Reflection.Emit.Lightweight.4.6.0/lib/netstandard2.0

- System.Reflection.Emit.Lightweight.dll

From Packages/System.Runtime.CompilerServices.Unsafe.4.5.2/lib/netstandard2.0

- System.Runtime.CompilerServices.Unsafe.dll

From Packages/Microsoft.Azure.Kinect.Sensor.1.3.0/lib/native/amd64/release

- depthengine_2_0.dll
- k4a.dll
- k4arecord.dll

From Packages/Microsoft.Azure.Kinect.BodyTracking.1.0.1/lib/native/amd64/release

- k4abt.dll
- onnxruntime.dll



#### 2) Then add these libraries to the MoveBox project root directory that contains the Assets folder

From Packages/Microsoft.Azure.Kinect.BodyTracking.Dependencies.cuDNN.0.9.1/lib/native/amd64/release

- cudnn64_7.dll

From Packages/Microsoft.Azure.Kinect.BodyTracking.Dependencies.0.9.1/lib/native/amd64/release

- cublas64_100.dll
- cudart64_100.dll

From Packages/Microsoft.Azure.Kinect.BodyTracking.1.0.1/lib/native/amd64/release

- onnxruntime.dll

From Packages/Microsoft.Azure.Kinect.BodyTracking.1.0.1/content

- dnn_model_2_0.onnx


### Run the Project
Open the Unity Project and under Assets\Microsoft Rocketbox MoveBox\Scenes select the MoveBox_CaptureStudio
Introduce the Microsoft Rocketbox avatar into the scene
Attach the MoveBoxPlayback script if you are reproducing exising animation
Or select the avatar parent in the MoveBox script on the MoveBox gameobject for realtime capturing and/or recording.
On the inspector select whether you use a Kinect v2 or an Azure Kinect

Press play.

## Instructions for MoveBox_IKHandTracking

MoveBox_IKHandTracking enables users of VR headsets to embody their rocketbox avatars and use their controllers to control the avatar motions in realtime from their HMD. We extrapolate the position of the upperbody (head and arms) based on the head and the hand-controllers positions and rotations. The demo also has finger tracking implemented for users of Oculus Quest.

## Instructions for MoveBox_OfflineVideoTracking
Using an external tool we can retrieve the skeletons from archival footage and convert them to animations for the rocketbox avatars.

#### 1) Install the VIBE system:

1. Install Anaconda
2. Create a virtual environment with python 3.7 version (e.g.  conda create -n MoveBox_Test1 python=3.7)
conda activate MoveBox_Test1

3. Open the VIBE folder downloaded from this github project
4. Run the batch file for installation ( .\install_windows.bat) 

After the above steps, the VIBE and all required models and libraries should be installed correctly on your own PC. If we want to estimate 3D pose from RGB video, then we just need to run the demo_windows,py file
5. python demo_windows.py --vid_file sample_video.mp4 --output_folder output/

There are only 2 parameters that need to be specified:
  --vid_file :  input video path
  --output_folder : output folder to write results  

Every video will generate its own result folder (e.g. output/sample_video1/, output/sample_video2/) and the joint CSV files will be stored in it.



#### 2) Import the offline 3D pose data to the Unity project at MoveBox_OfflineVideo

1. Copy the 3D pose csv data to the Assets/Resource folder
2. Attach the LoadPose script to the avatar (you can use whatever avatar of your choice) game object
3. Copy the pose csv file name to the Pose File property in the LoadPose script 
4. Play and press P to start animating the avatar


## Main Contributors
Mar Gonzalez-Franco - Microsoft Research

Eyal Ofek - Microsoft Research

### Kinect Sensing and Capture Studio tools
Angus Antley - Microsoft

Tanmay Randhavane - Microsoft

Zelia Egan - Microsoft

Mattew Peachey - Dalhousie University

### Offline Video Tracking
Andrea Stevenson Won - Cornell University

Cheng Yao Wang - Cornell University

### IK and Hand Tracking
Yaying Zhang - Microsoft


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


