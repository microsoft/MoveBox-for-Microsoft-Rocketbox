:: Avoid printing all the comments in the Windows cmd
@echo off
echo -----install VIBE ----- -----
call pip install torch===1.4.0 torchvision===0.5.0 -f https://download.pytorch.org/whl/torch_stable.html
call pip install pytube3
call conda install -c conda-forge ffmpeg
call pip install -r requirements.txt

echo ----- downloading VIBE and YOLOv3 models -----
call mkdir data
call cd data

call gdown "https://drive.google.com/uc?id=1untXhYOLQtpNEy4GTY_0fL_H-k6cTf_r"
call tar -xf vibe_data.zip
call del vibe_data.zip
call gdown "https://drive.google.com/uc?id=1ryhzPas5rNTkx9lvF_GV_w7IoGhgw192"
call gdown "https://drive.google.com/uc?id=1FeO2HqHtVk62COnfOVZn3OXQvBXUboZK"

call cd..
if not exist "%HOMEDRIVE%\%HOMEPATH%\.torch\models\" mkdir "%HOMEDRIVE%\%HOMEPATH%\.torch\models\"
if not exist "%HOMEDRIVE%\%HOMEPATH%\.torch\config\" mkdir "%HOMEDRIVE%\%HOMEPATH%\.torch\config\"
call move  data\vibe_data\sample_video.mp4 .
call move  data\yolov3.weights "%HOMEDRIVE%\%HOMEPATH%\.torch\models\"
call move  data\yolov3.cfg "%HOMEDRIVE%\%HOMEPATH%\.torch\config\"




