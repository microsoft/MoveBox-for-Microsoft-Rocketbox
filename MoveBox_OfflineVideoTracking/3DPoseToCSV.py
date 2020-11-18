# Generate the CSV file from VIBE's output.
import os
import csv
import joblib
import argparse
import numpy as np

def main(args):
  vibe_output = os.path.abspath(args.vibe_output)
  output = joblib.load(vibe_output)

  output_path = os.path.join(args.output_folder, "csv_result")
  os.makedirs(output_path, exist_ok=True)

  vid_name = os.path.basename(vibe_output)
  vibe_result_folder = output_path
  
  # output the pose result as csv
  # format: v_personId_numFrames
  pose_filename_list = []

  for i in output.keys():
    pose_filename = vibe_result_folder + "/" + vid_name + "_"+ str(i) + "_" +  str(output[i]['pose'].shape[0]) + ".csv"
    pose_filename_list.append(pose_filename)
    field_names = []
    for idx in range(73): # 72 -> 73 (+ frame_id at 0)
      field_names.append(str(idx))

      with open(pose_filename, 'w', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(field_names)
        for frame_id in range(len(output[i]['pose'])):
          output_data = [output[i]['frame_ids'][frame_id]]
          output_data.extend(output[i]['pose'][frame_id])
          writer.writerow(output_data)

if __name__ == '__main__':
  parser = argparse.ArgumentParser()
  parser.add_argument('--vibe_output', type=str, 
                        help='VIBE pkl output file path')
  parser.add_argument('--output_folder', type=str,
                        help='output csv folder')
  args = parser.parse_args()
  main(args)