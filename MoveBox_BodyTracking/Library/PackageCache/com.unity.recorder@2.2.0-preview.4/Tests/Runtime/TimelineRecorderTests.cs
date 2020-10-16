using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder.Timeline;
using UnityEngine.Playables;
using UnityEngine.TestTools;
using UnityEngine.Timeline;

namespace UnityEngine.Recorder.Tests
{
	class TimelineRecorderTests
	{	
		[UnityTest]
		public IEnumerator TimelineRecorder_ShouldHaveProperPublicAPI()
		{
			var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
			var track = timeline.CreateTrack<RecorderTrack>(null, "AAA");

			var clip = track.CreateClip<RecorderClip>();
			
			clip.start = 1.0f;
			clip.duration = 3.0f;

			var recorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();

			var expectedOutputFile = Application.dataPath + "/../RecordingTests/movie_test_from_timeline_001.mp4";

			recorderSettings.OutputFile = Application.dataPath + "/../RecordingTests/movie_test_from_timeline_" + DefaultWildcard.Take;
			
			recorderSettings.ImageInputSettings = new GameViewInputSettings
			{
				OutputWidth = 640,
				OutputHeight = 480
			};
				
			recorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
			recorderSettings.VideoBitRateMode = VideoBitrateMode.Low;
			
			var recorderClip = (RecorderClip) clip.asset;
			recorderClip.settings = recorderSettings;

			var director = new GameObject("director").AddComponent<PlayableDirector>();
			director.playableAsset = timeline;

			timeline.durationMode = TimelineAsset.DurationMode.FixedLength;
			timeline.fixedDuration = 5.0f;
			
			if (File.Exists(expectedOutputFile))
				File.Delete(expectedOutputFile);
			
			director.Play();

			new GameObject("Camera").AddComponent<Camera>().transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
			new GameObject("Light").AddComponent<Light>().type = LightType.Directional;
			var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.localPosition = new Vector3(0.0f, -5.0f, 0.0f);

			var runTimeSeconds = clip.start + clip.duration + 0.5f;
			
			while (director.time < runTimeSeconds)
			{
				cube.transform.localPosition = cube.transform.localPosition + Vector3.up * 0.02f; 
				yield return null;
			}
			
			Assert.IsTrue(File.Exists(expectedOutputFile));
			
			File.Delete(expectedOutputFile);

			Assert.Pass();
		}
	}
}
