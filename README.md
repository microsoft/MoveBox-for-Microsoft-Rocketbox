# MoveBox for Microsoft Rocketbox

MoveBox is a toolbox to animate the Microsoft Rocketbox avatars using motion captured (MoCap). Motion capture is performed using a single depth sensor, such as Azure Kinect or Windows Kinect V2. Our toolbox enables real-time animation of the user's avatar by converting the transformations between systems that have different joints and hierarchies. Additional features of the toolbox include recording, playback and looping animations, as well as basic audio lip sync, blinking and resizing of avatars. Our main contribution is both in the creation of this open source tool as well as the integration of MoveBox with Kinect V2 and Azure Kinect, as well as the interfacing with the Microsoft Rocketbox avatars.

Microsoft Privacy Statement
https://privacy.microsoft.com/en-us/privacystatement

## Directions for getting started:


### Download the desired Microsoft Rocketbox Avatars
https://github.com/microsoft/Microsoft-Rocketbox


## Installation

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

Open the MoveBox project in Unity.
Open the Visual Studio Solution associated with this project "MoveBox4Rocketbox.sln".
If there is no Visual Studio Solution yet you can make one by opening the Unity Editor
and selecting one of the csharp files in the project and opening it for editing.
You may also need to set the preferences->External Tools to Visual Studio

In Visual Studio:
Select Tools->NuGet Package Manager-> Package Manager Console

On the command line of the console at type the following command:

```bash
Install-Package Microsoft.Azure.Kinect.BodyTracking -Version 1.0.1
```

The body tracking libraries will be put in the Packages folder under MoveBox 


#### 2) Next add these libraries to the Assets/Plugins folder:

You can do this by hand or just run the batch file MoveLibraryFile.bat in the MoveBox  directory


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



## RUN THE TOOLBOX
Open the Unity Project and under Scenes/  select the MoveBox4MicrosoftRocketbox


Press play.



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


