<a name="360View"></a>
### 360 View capture properties

These properties appear when you set **Capture** to **360 View**.

To capture 360 degree recordings, the Recorder rotates the **Source** camera 360 degrees in about its **Y** axis.

![](Images/CaptureOptions360View.png)

|Property:||Function|
|-|-|-|
| **Source** ||Specifies which camera the Recorder uses as the point of view for the 360 degree recording.<br/><br/>**Note:** some options may not appear if you're using certain render pipelines (for example [HDRP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@latest)). |
|   | Main Camera   | The Camera tagged with the MainCamera [Tag](https://docs.unity3d.com/Manual/Tags.html).  |
|   | Tagged Camera  | A camera tagged with a specific  [Tag](https://docs.unity3d.com/Manual/Tags.html). |
|**Tag**   |   | When you set **Source** to **Tagged Camera**, specifies which Tag to look for.|
| **360 View Output** ||**W** and **H** specify the width and height, in pixels, of the 360-degree video.|
| **Cube Map** ||The size of the cube map, in pixels, for the 360-degree video. |
| **Stereo** ||When you enable this option, the Recorder outputs separate left and right stereoscopic views of the 360-degree video.  |
| **Stereo Separation** ||When you enable the **Stereo** option, this property specifies the angle between the left and right views on the **Source** camera's **Y** axis.|
| **Flip Vertical** ||When you enable this option, the Recorder flips the output image vertically.<br/><br/> This is useful to correct for systems that output video upside down.|