# Configuring Animation Clip Recorders

The **Animation Clip Recorder** generates an animation clip in the .anim file format.

This page covers the properties specific to Animation Clip Recorders. To fully configure an Animation Clip Recorder, you must also set:

- The Recorder's [Output properties](RecorderProperties.md).
- The [Recording Properties](Recording.md) for the capture.

## Animation Clip Recorder properties

![](Images/RecorderAnimation.png)

|Property:|Function:|
|:---|:---|
| **Game Object** |The [GameObject](https://docs.unity3d.com/Manual/class-GameObject.html) to record.|
| **Recorded Target(s)** |The components of the GameObject to record. Choose more than one item to record more than one component.|
| **Record Hierarchy** |Enable this property to record the GameObject's children.|

>[!NOTE]
>The Animation Clip Recorder can only record a GameObject in the current Scene. It cannot record GameObjects in other Scenes.
