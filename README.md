# MoveBox for Microsoft Rocketbox

MoveBox is a toolbox to animate the Microsoft Rocketbox avatars (https://github.com/microsoft/Microsoft-Rocketbox) using motion captured in three different ways: (1) Body Tracking, (2) Inverse Kinematics or (3) Archival Footage. Each form of animation of the avatars is presented as a separate project.

1- MoveBox_BodyTracking. We create a Motion Capture (MoCap) studio. Where MoCap can be performed using a single depth sensor, such as Azure Kinect or Windows Kinect V2. Our toolbox enables real-time animation of the user's avatar by converting the transformations between systems that have different joints and hierarchies. Additional features of the toolbox include recording, playback and looping animations, as well as basic audio lip sync, blinking and resizing of avatars. Our main contribution is both in the creation of this open source tool as well as the integration of MoveBox with Kinect V2 and Azure Kinect, as well as the interfacing with the Microsoft Rocketbox avatars.

2- MoveBox_IKHandTracking \*. Using an off the shelf HMD you can recover hand and head positions and rotations of the user and transfer them to a Microsoft Rocketbox Avatar of your choice. All in realtime. The hand tracking is used for an Inverse Kinematics (IK) solver that reconstructs a possible elbow position. Our demo project also includes finger tracking for the Oculus Quest.

3- MoveBox_OfflineVideoTracking. Movebox includes an external tool for 3D multi-person human pose estimation from RGB videos. We utilized a deep-learning based approach open sourced as VIBE (https://github.com/mkocabas/VIBE), which trains a temporal model to predict the parameters of the SMPL body model for each frame while a motion discriminator tries to distinguish between real and regressed sequences. 
The output of the model is a sequence of pose and shape parameters in the SMPL body model format (https://smpl.is.tue.mpg.de/). To animate Microsoft RocketBox avatars with predicted 3D poses, the toolbox first extracted the joints data from pose parameters, computed the transformation between SMPL and RocketBox skeleton structures, and then mapped to the corresponding joint in the Microsoft RocketBox avatar skeleton.

\*Note: Tools from the capture studio (MoveBox) can be imported to project 2 if there is a need to record the motions of the participant or user for later use.

Microsoft Privacy Statement
https://privacy.microsoft.com/en-us/privacystatement

## Reference:
We released this paper at IEEE AIVR together with the toolbox and gives more details of the features included:

_Mar Gonzalez-Franco, Zelia Egan, Matthew Peachey, Angus Antley, Tanmay Randhavane, Payod Panda, Yaying Zhang,  Cheng Yao Wang, Derek F. Reilly, Tabitha C Peck, Andrea Stevenson Won, Anthony Steed and Eyal Ofek (2020) "**MoveBox: Democratizing MoCap for the Microsoft Rocketbox Avatar Library**". IEEE International Conference on Artificial Intelligence and Virtual Reality (AIVR)_

If you use this library for research or academic purposes, please also cite the aforementioned paper.

## Instructions for MoveBox_BodyTracking

### Install SDKs for Kinect V2 and/or Azure Kinect
Kinect V2
https://developer.microsoft.com/en-us/windows/kinect/

Kinect for Windows SDK 2.0
https://www.microsoft.com/en-us/download/details.aspx?id=44561

Azure Kinect
https://docs.microsoft.com/en-us/azure/kinect-dk/

Azure Kinect Sensor SDK
Azure Kinect Body Tracking SDK
https://docs.microsoft.com/en-us/azure/kinect-dk/sensor-sdk-download
https://docs.microsoft.com/en-us/azure/kinect-dk/body-sdk-download


### FOR AZURE KINECT
#### 1) First get the latest nuget packages of libraries:

Open the MoveBox_BodyTracking project in Unity.
Open the Visual Studio Solution associated with this project "MoveBox_BodyTracking.sln".
If there is no Visual Studio Solution yet you can make one by opening the Unity Editor
and selecting one of the csharp files in the project and opening it for editing.
You may also need to set the preferences->External Tools to Visual Studio

In Visual Studio:
Select Tools->NuGet Package Manager-> Package Manager Console

On the command line of the console at type the following command:

```bash
Install-Package Microsoft.Azure.Kinect.BodyTracking -Version 1.0.1
```

The body tracking libraries will be put in the Packages folder under MoveBox_BodyTracking 


#### 2) Next add these libraries to the Assets/Plugins folder:

You can do this by hand or just run the batch file MoveLibraryFile.bat in the MoveBox_BodyTracking directory


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
Open the Unity Project and under Assets\Microsoft Rocketbox MoveBox\Scenes select the MoveBox_BodyTracking
Introduce the Microsoft Rocketbox avatar into the scene
Attach the MoveBoxPlayback script if you are reproducing exising animation
Or select the avatar parent in the MoveBox script on the MoveBox gameobject for realtime capturing and/or recording.
On the inspector select whether you use a Kinect v2 or an Azure Kinect

Press play.

## Instructions for MoveBox_IKHandTracking

MoveBox_IKHandTracking enables users of VR headsets to embody their rocketbox avatars and use their controllers to control the avatar motions in realtime from their HMD. We extrapolate the position of the upperbody (head and arms) based on the head and the hand-controllers positions and rotations. The demo also has finger tracking implemented for users of Oculus Quest.

## Instructions for MoveBox_OfflineVideoTracking
Using an external tool we can retrieve the skeletons from archival footage and convert them to animations for the rocketbox avatars.

## Main Contributors
Mar Gonzalez-Franco - Microsoft Research

Eyal Ofek - Microsoft Research

### Kinect Sensing
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


