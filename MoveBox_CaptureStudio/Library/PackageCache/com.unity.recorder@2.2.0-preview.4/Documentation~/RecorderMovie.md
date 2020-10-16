# Configuring Movie Recorders

The **Movie Recorder** generates a video in the .mp4 or .webm file format. It does not support variable frame rates.

This page covers properties specific to Movie Recorders. To fully configure a Movie Recorder, you must also set:

- The Recorder's [Output properties](RecorderProperties.md).
- The [Recording Properties](Recording.md) for the capture.

## Movie Recorder properties

![](Images/RecorderMovie.png)

|Property:||Function:|
|:---|:---|:---|
| **Format** ||The encoding format of the Recorder's output. Choose **.mp4** or **WEBM**.|
| **Capture Alpha** ||Enable this property to include the alpha channel in the recording. Disable it to only record the RGB channels. This property is only available when **Format** is **WEBM**. This property is not available when **Capture** is **Game View**.|
| **Capture** ||Specifies the input for the recording.|
|| Game View |Records frames rendered in the Game View.<br/><br/>Selecting this option displays the [Game View capture properties](#GameView). |
|| Targeted Camera |Records frames captured by a specific Camera, even if the Game View does not use that Camera.<br/><br/>Selecting this option displays the [Targeted Camera capture properties](#TargetedCamera).|
|| 360 View |Records a 360-degree video.<br/><br/>Selecting this option displays the [360 View capture properties](#360View).|
|| Render Texture Asset |Records frames rendered in a Render Texture.<br/><br/>Selecting this option displays the [Render Texture Asset capture properties](#RenderTextureAsset).|
|| Texture Sampling |Supersamples the **Source** camera during the capture to generate anti-aliased images in the recording. Use this capture method when the **Rendering Resolution** has the same or higher resolution than the **Output Resolution**. <br/><br/>Selecting this option displays the [Texture Sampling capture properties](#TextureSampling).|
| **Capture Audio** ||Enable this property to include audio in the recording.|
| **Quality** || Sets the quality of the output movie: **Low**, **Medium**, or **High**. The lower the quality, the smaller the file size.|

[!include[](InclCaptureOptionsGameview.md)]

[!include[](InclCaptureOptionsTargetedCamera.md)]

[!include[](InclCaptureOptions360View.md)]

[!include[](InclCaptureOptionsRenderTextureAsset.md)]

[!include[](InclCaptureOptionsTextureSampling.md)]



