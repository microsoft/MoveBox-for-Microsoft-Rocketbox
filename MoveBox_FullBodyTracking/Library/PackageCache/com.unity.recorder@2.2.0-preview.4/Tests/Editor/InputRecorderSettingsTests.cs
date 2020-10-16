using UnityEngine;
using NUnit.Framework;
using UnityEditor.Recorder.Input;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder.Tests
{
	class InputRecorderSettingsTests
	{
		[Test]
		public void CameraInputSettings_ShouldHaveProperPublicAPI()
		{
			var input = new CameraInputSettings
			{
				Source = ImageSource.MainCamera,
				OutputWidth = 640,
				OutputHeight = 480,
				CameraTag = "AAA",
				CaptureUI = true,
				FlipFinalOutput = false
			};

			Assert.AreEqual(640, input.OutputWidth);
			Assert.AreEqual(480, input.OutputHeight);
			
			Assert.NotNull(input);
		}

		[Test]
		public void GameViewInputSettings_ShouldHaveProperPublicAPI()
		{
			var input = new GameViewInputSettings
			{
				OutputWidth = 1920,
				OutputHeight = 1080,
			};
			
			Assert.AreEqual(1920, input.OutputWidth);
			Assert.AreEqual(1080, input.OutputHeight);

			input.OutputWidth = 123;
			input.OutputHeight = 456;
			
			Assert.AreEqual(123, input.OutputWidth);
			Assert.AreEqual(456, input.OutputHeight);
			
			Assert.NotNull(input);
		}
		
		[Test]
		public void Camera360InputSettings_ShouldHaveProperPublicAPI()
		{
			var input = new Camera360InputSettings
			{
				Source = ImageSource.MainCamera,
				CameraTag = "AAA",
				RenderStereo = true,
				StereoSeparation = 0.065f,
				MapSize = 1024,
				OutputWidth = 1024,
				OutputHeight = 2048,
				FlipFinalOutput = false
			};
			
			Assert.AreEqual(1024, input.OutputWidth);
			Assert.AreEqual(2048, input.OutputHeight);
			
			Assert.NotNull(input);
		}
		
		[Test]
		public void RenderTextureInputSettings_ShouldHaveProperPublicAPI()
		{
			var rt = new RenderTexture(1234, 123, 24);
			
			var input = new RenderTextureInputSettings
			{
				FlipFinalOutput = false,
				RenderTexture = rt
			};
			
			Assert.AreEqual(1234, input.OutputWidth);
			Assert.AreEqual(123, input.OutputHeight);

			input.OutputWidth = 256;
			input.OutputHeight = 128;
			
			Assert.AreEqual(256, rt.width);
			Assert.AreEqual(128, rt.height);
			
			UnityObject.DestroyImmediate(rt);
			
			Assert.AreEqual(0, input.OutputWidth);
			Assert.AreEqual(0, input.OutputHeight);
		}
		
		[Test]
		public void RenderTextureSamplerSettings_ShouldHaveProperPublicAPI()
		{
			var input = new RenderTextureSamplerSettings
			{
				OutputWidth = 640,
				OutputHeight = 480,
				RenderWidth = 1920,
				RenderHeight = 1080,
				CameraTag = "AAA",
				ColorSpace = ColorSpace.Gamma,
				FlipFinalOutput = false,
				SuperSampling = SuperSamplingCount.X4
			};
			
			Assert.AreEqual(640, input.OutputWidth);
			Assert.AreEqual(480, input.OutputHeight);
			
			Assert.AreEqual(1920, input.RenderWidth);
			Assert.AreEqual(1080, input.RenderHeight);
			
			Assert.NotNull(input);
		}
		
		[Test]
		public void AnimationInputSettings_ShouldHaveProperPublicAPI()
		{
			var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

			var input = new AnimationInputSettings { gameObject = cube };
			
			input.AddComponentToRecord(typeof(Transform));
			input.AddComponentToRecord(typeof(Renderer));
			
			Assert.IsTrue(input.gameObject == cube);

			input.gameObject = null;
			
			Assert.IsTrue(input.gameObject == null);
		}
		
		[Test]
		public void AudioInputSettings_ShouldHaveProperPublicAPI()
		{
			var input = new AudioInputSettings
			{
				PreserveAudio = true
			};
			
			Assert.NotNull(input);
		}
	}
}
