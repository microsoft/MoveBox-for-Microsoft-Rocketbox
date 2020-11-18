# Movebox Offline Video Tracking Application

### Directions for getting started:

This project uses the VIBE: Video Inference for Human Body Pose and Shape Estimation [CVPR-2020]

> [**VIBE: Video Inference for Human Body Pose and Shape Estimation**](https://arxiv.org/abs/1912.05656),            
> [Muhammed Kocabas](https://ps.is.tuebingen.mpg.de/person/mkocabas), [Nikos Athanasiou](https://ps.is.tuebingen.mpg.de/person/nathanasiou), 
[Michael J. Black](https://ps.is.tuebingen.mpg.de/person/black),        
> *IEEE Computer Vision and Pattern Recognition, 2020* 
Watch [this video](https://www.youtube.com/watch?v=fW0sIZfQcIs) for more qualitative results.


# LICENSE NOTE:
VIBE needs to be downloaded from 
```bash
git clone https://github.com/mkocabas/VIBE.git
```
VIBE has a License agreement only for research uses. Please look at the original license before using it.

# SMLP for Microsoft Rocketbox


#### 1) Use the VIBE system:

1. Run your video throught VIBE

2. Use the 3DPoseToCSV.py file to convert the output pkl to csv

```bash
python 3DPoseToCSV.py --vibe_output ./vibe_output.pkl --output_folder ./
```


#### 2) Import the offline 3D pose data to the Unity project at MoveBox_OfflineVideo

1. Copy the 3D pose csv data to the Assets/Resource folder
2. Attach the LoadPose script to the avatar (you can use whatever avatar of your choice) game object
3. Copy the pose csv file name to the Pose File property in the LoadPose script 
4. Play and press P to start animating the avatar

