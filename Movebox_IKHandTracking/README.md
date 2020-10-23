# MoveBox IKHandTracking for Microsoft Rocketbox


Using an off the shelf HMD we recover realtime motions to create avatar embodiment. The hand and head positions and rotations of the user are transferred to a Microsoft Rocketbox Avatar of your choice. All in realtime. The hand tracking is used for an Inverse Kinematics (IK) solver (included) that reconstructs a possible elbow position. Our demo project also includes finger tracking for the Oculus Quest.


1. Open the Unity MoveBox_IKHandTracking.  
2. Load the scene MoveBox_IKHandTrackingOVRDemo from Assets/Scenes
3. Add your avatar and add:
 - RocketBoxOVRHandTracking script to enable finger tracking with Oculus quest
 - RocketboxArmIK to enable Inverse Kinematics from the controllers. Set the controllers to the Effector L and R and the ehad to the center of the HMD.


