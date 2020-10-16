<a name="TargetedCamera"></a>
### Targeted Camera capture properties

These options appear when you set **Capture** to **Targeted Camera**.

![](Images/CaptureOptionsTargetedCamera.png)

|Property:||Function:|
|-|-|-|
| **Source** ||Specifies which camera the Recorder uses to capture the recording.<br/><br/>**Note:** some options may not appear if you're using certain render pipelines (for example [HDRP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@latest)).|
|   | ActiveCamera  | The Camera that is active when you launch the recording.  |
|   | Main Camera   | The Camera tagged with the MainCamera [Tag](https://docs.unity3d.com/Manual/Tags.html).  |
|   | Tagged Camera  | A camera tagged with a specific [Tag](https://docs.unity3d.com/Manual/Tags.html). |
|**Tag**   |   | When you set **Source** to **Tagged Camera**, specifies which Tag to look for.|
| **Output Resolution** ||The dimensions of the  recording.|
|   | Match Window Size  | Matches the current monitor resolution.  |
|   |  _[PRESET RESOLUTIONS]_ | Choose from several standard video resolutions such as 1080p and 4K.  |
|   |  Custom | Uses custom width and height values that you supply.  |
| **Aspect Ratio** ||These options only appear when you set **Output Resolution** to **Custom**. |
|   | W/H   | Control the recording's width and height.|
| **Include UI** ||When you enable this option, the recording includes UI GameObjects.<br/><br/>This option only appears when you set **Source** to **ActiveCamera**.|
| **Flip Vertical** ||When you enable this option, the Recorder flips the output image vertically.<br/><br/> This is useful to correct for systems that output video upside down.|