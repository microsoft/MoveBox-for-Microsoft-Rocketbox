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
And has a License agreement only for research uses. Please look at the original license before using it.

# SMLP for Microsoft Rocketbox

## Getting Started in Windows

Clone the repo:
```bash
git clone https://github.com/mkocabas/VIBE.git
```

Install the requirements using `pip` or `conda`:
```bash
# pip
bash install_pip.sh

# conda
bash install_conda.sh
```

## Running the Demo

We have prepared a nice demo code to run VIBE on arbitrary videos. 
First, you need download the required data (i.e our trained model and SMPL model parameters). To do this you can just run:

```bash
bash prepare_data.sh
```

Then, running the demo is as simple as this:

```bash
# Run on a local video
python demo.py --vid_file sample_video.mp4 --output_folder output/ --display

# Run on a YouTube video
python demo.py --vid_file https://www.youtube.com/watch?v=wPZP8Bwxplo --output_folder output/ --display
```


