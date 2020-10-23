# Movebox Offline Video Tracking Application

### Directions for getting started:


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
