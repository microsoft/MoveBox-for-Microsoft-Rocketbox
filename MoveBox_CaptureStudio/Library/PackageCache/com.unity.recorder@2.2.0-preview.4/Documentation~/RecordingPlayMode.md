# Recording in Play mode

This page explains how to set up and launch a recording from the Recorder window.

If you want to record from a Timeline Track, see [Recording From a Timeline Track](RecordingTimelineTrack.md).

## Before you start a recording

Before you record Play-mode data, make sure you do the following:

- Set up your Scene for recording.
- Created the Recorders you want to use to capture the Scene. See [Creating and managing Recorders](RecorderManage.md) for details.
- Set the Recorder properties. See [Setting  Recorder properties](RecorderProperties.md) for details.

## Setting up a recording

Use the recording controls in the Recorder window to set up a recording.

Use the **Frame Rate** properties to specify how to constrain the frame rate during recording. The frame rate affects the size and number of the files the Recorder outputs.

![](Images/RecordingControls.png)

### Recording controls

|Property:||Function:|
|:---|:---|:---|
|![](Images/BtnRecord.png)<br/>**[Record Button]**   | |Starts and stops recording. Clicking this button also activates Play mode if it is not already active.  |
| **Start Recording**/<br/>**Stop Recording** ||Starts and stops recording. Clicking **Start Recording** also activates Play mode if it is not already active.|
|**Exit PlayMode**   | |When this option is enabled, the Unity Recorder automatically exits Play mode when it finishes recording.  |
| **Record Mode** ||Specifies the frames or time interval to record.|
||_Manual_ |Start and stop recording when you manually click **Start Recording** and **Stop Recording**, respectively.|
|| _Single Frame_ |Record a single frame. Use the **Frame Number** property to specify this frame.|
|| _Frame Interval_ |Record a set of consecutive frames during Play mode. Use the **Start** and **End** properties to specify when to start and stop.|
|| _Time Interval_ |Record a specific duration, in seconds, during Play mode. Use the **Start** and **End** properties to specify when to start and stop.|
| **Frame Number** ||Specifies the number of the frame to capture when in **Single Frame** mode.|
| **Start**/ <br/> **End** ||• When in **Frame Interval** mode, specifies the range of frames to capture.<br/>• When in **Time Interval** mode, specifies the time, in seconds, to start and stop recording.|

### Frame Rate properties

|Property:||Function:|
|:---|:---|:---|
| **Playback** ||Specifies how to control the frame rate during recording.|
|| _Constant_ |Limit the Recorder to a specific frame rate. Use the **Target** property to specify this rate.|
|| _Variable_ |Use the application's frame rate. Specify the upper limit of the application's rate during recording with the **Max Frame Rate** property.<br/><br/> **Note:** The **Movie Recorder** does not support a variable frame rate.|
| **Target** ||Sets the frame rate to capture the recording at. This property appears when you set **Playback** to **Constant**. <br/><br/> The Unity Recorder captures at this rate regardless of whether you run your application at a higher or lower frame rate. For example, if you set **Target** to a custom value of 30 fps but you run your application at 60 fps, the Recorder captures at 30 fps.|
| **Max Frame Rate** ||Limit the rate of updates in Play mode . This property is available when you set **Playback** to **Variable**. To prevent your application from exceeding this frame rate, the Unity Recorder inserts delays during playback. Use this property to reduce the size of the output.|
| **Cap Frame Rate** ||Enable this property when the frame rate of your application is faster than the **Target** frame rate. This property is available when **Playback** is **Constant**.|


## Starting and stopping a recording

When you start a recording, the Unity Recorder activates Play mode (if it is not already active) and starts to capture Play-mode data using all active Recorders.

**To start recording:**

- Do one of the following:
  - In the Recorder window, click the Record button or the **START RECORDING** button.
  - Press **F10**/**fn+F10**.
  - From the main menu, select **Window > General > Recorder > Quick Recording**.

> [!NOTE]
> During recording, you cannot modify the properties in the Recorder window.

Most **Recording Mode** settings stop the recording automatically. If you set **Recording Mode** to **Manual**, you must stop the recording yourself.

When a recording stops the Editor remains in Play mode unless:

- You enable the **Exit PlayMode** property in the [recording controls](#setting-up-a-recording).
- You stop the recording by exiting Play mode).

**To stop recording:**

- Do one of the following:
  - In the Recorder window, click the Record button or the **START RECORDING** button.
  - Use the **F10**/**fn+F10** shortcut.
  - Close the Recorder window.
  - Exit Play mode.
