using System;
using UnityEngine;
using NUnit.Framework;
using UnityEditor.Recorder.Input;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder.Tests
{
	class RecorderSettingsTests
	{
		[Test]
		public void ImageRecorderSettings_ShouldHaveProperPublicAPI()
		{
			var recorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();

			AssertBaseProperties(recorder);

			recorder.CaptureAlpha = true;
			recorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;

			Assert.IsTrue(recorder.imageInputSettings is GameViewInputSettings);
			
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new CameraInputSettings());
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new GameViewInputSettings());
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new Camera360InputSettings());
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new RenderTextureInputSettings());
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new RenderTextureSamplerSettings());
			
			Assert.Throws<ArgumentNullException>(() => recorder.imageInputSettings = null);
			
			UnityObject.DestroyImmediate(recorder);
		}
		
		[Test]
		public void MovieRecorderSettings_ShouldHaveProperPublicAPI()
		{
			var recorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();

			AssertBaseProperties(recorder);

			recorder.CaptureAlpha = true;
			recorder.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
			recorder.VideoBitRateMode = VideoBitrateMode.High;

			Assert.IsTrue(recorder.ImageInputSettings is GameViewInputSettings);
			Assert.IsNotNull(recorder.AudioInputSettings);
			
			Assert.DoesNotThrow(() => recorder.ImageInputSettings = new CameraInputSettings());
			Assert.DoesNotThrow(() => recorder.ImageInputSettings = new GameViewInputSettings());
			Assert.DoesNotThrow(() => recorder.ImageInputSettings = new Camera360InputSettings());
			Assert.DoesNotThrow(() => recorder.ImageInputSettings = new RenderTextureInputSettings());
			Assert.DoesNotThrow(() => recorder.ImageInputSettings = new RenderTextureSamplerSettings());
			
			Assert.Throws<ArgumentNullException>(() => recorder.ImageInputSettings = null);
			
			UnityObject.DestroyImmediate(recorder);
		}
		
		[Test]
		public void AnimationRecorderSettings_ShouldHaveProperPublicAPI()
		{
			var recorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();

			AssertBaseProperties(recorder);

			Assert.IsNotNull(recorder.AnimationInputSettings);
			
			UnityObject.DestroyImmediate(recorder);
		}

		[Test]
		public void GIFRecorderSettings_ShouldHaveProperPublicAPI()
		{
			var recorder = ScriptableObject.CreateInstance<GIFRecorderSettings>();

			AssertBaseProperties(recorder);

			recorder.NumColors = 123;
			recorder.KeyframeInterval = 15;
			recorder.MaxTasks = 10;

			Assert.IsTrue(recorder.imageInputSettings is CameraInputSettings);
			
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new CameraInputSettings());
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new RenderTextureInputSettings());
			Assert.DoesNotThrow(() => recorder.imageInputSettings = new RenderTextureSamplerSettings());
			
			Assert.Throws<ArgumentException>(() => recorder.imageInputSettings = new GameViewInputSettings());
			Assert.Throws<ArgumentException>(() => recorder.imageInputSettings = new Camera360InputSettings());
			
			Assert.Throws<ArgumentNullException>(() => recorder.imageInputSettings = null);
			
			UnityObject.DestroyImmediate(recorder);
		}
		
		[TestCase("C:/AAA/BBB.MP4", "C:/AAA/BBB.MP4")]
		[TestCase("C:\\\\AAA///\\BBB.MP4", "C:/AAA/BBB.MP4")]
		[TestCase("AAA.MP4", "AAA.MP4")]
		[TestCase("AAA", "AAA")]
		[TestCase("Assets/AAA/BBB.MP4", "Assets/AAA/BBB.MP4")]
		[TestCase("C:/Assets/AAA/BBB.MP4", "C:/Assets/AAA/BBB.MP4")]
		[TestCase("../AAA/BBB.MP4", "../AAA/BBB.MP4")]
		[TestCase("/AAA", "/AAA")]
		[TestCase("/AAA.MP4", "/AAA.MP4")]
		[Ignore("Waiting for CI to be fixed")]
		public void OutputFile_ShouldReturnAssignedValue(string value, string expected)
		{
			var recorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();

			recorder.OutputFile = value;
			Assert.AreEqual(expected, recorder.OutputFile);
		}
		
		[TestCase(null)]
		[TestCase("")]
		public void OutputFile_InvalidPathShouldThrow(string value)
		{
			var recorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();

			Assert.Throws<ArgumentException>(() => recorder.OutputFile = value);
		}

		static void AssertBaseProperties(RecorderSettings recorder)
		{
			Assert.IsTrue(recorder.Enabled);
			Assert.IsNotNull(recorder.OutputFile);
			Assert.IsTrue(recorder.Take == 1);

			// Test public access
			Assert.DoesNotThrow(() =>
			{
				var b = recorder.IsPlatformSupported;
			});
		}
	}
}
