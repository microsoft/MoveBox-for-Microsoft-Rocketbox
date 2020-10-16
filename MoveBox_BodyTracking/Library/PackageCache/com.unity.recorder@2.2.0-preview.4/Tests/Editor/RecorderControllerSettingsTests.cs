using UnityEngine;
using NUnit.Framework;
using System.Linq;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder.Tests
{
	class RecorderControllerSettingsTests
	{

		[Test]
		public void AddAndRemoveRecorderSettings_ShouldBeHandledProperly()
		{
			var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
			
			Assert.IsEmpty(settings.RecorderSettings);

			var recorder0 = ScriptableObject.CreateInstance<ImageRecorderSettings>();
			
			settings.AddRecorderSettings(recorder0);
			
			Assert.IsTrue(settings.RecorderSettings.Count() == 1);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(0) == recorder0);
			
			var recorder1 = ScriptableObject.CreateInstance<ImageRecorderSettings>();
			
			settings.AddRecorderSettings(recorder1);
			
			Assert.IsTrue(settings.RecorderSettings.Count() == 2);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(0) == recorder0);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(1) == recorder1);
			
			settings.AddRecorderSettings(recorder1); // Add twice the same
			
			Assert.IsTrue(settings.RecorderSettings.Count() == 2);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(0) == recorder0);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(1) == recorder1);
			
			var recorder2 = ScriptableObject.CreateInstance<ImageRecorderSettings>();
			
			settings.RemoveRecorder(recorder2); // Remove a recorder that was not previously added
			
			Assert.IsTrue(settings.RecorderSettings.Count() == 2);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(0) == recorder0);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(1) == recorder1);
			
			settings.RemoveRecorder(recorder0);
			
			Assert.IsTrue(settings.RecorderSettings.Count() == 1);
			Assert.IsTrue(settings.RecorderSettings.ElementAt(0) == recorder1);
			
			settings.RemoveRecorder(recorder1);
			
			Assert.IsEmpty(settings.RecorderSettings);
			
			UnityObject.DestroyImmediate(recorder0);
			UnityObject.DestroyImmediate(recorder1);
			UnityObject.DestroyImmediate(recorder2);
			UnityObject.DestroyImmediate(settings);
		}
	}
}
