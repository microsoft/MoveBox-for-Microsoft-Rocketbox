using NUnit.Framework;
using UnityEditor.Recorder;

namespace UnityEngine.Recorder.Tests
{
	class SceneHookTests
	{	
		[Test]
		public void SceneHookGameObject_AfterStartRecording_ShouldBeVisible()
		{
			var settings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
			var recorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
			
			settings.AddRecorderSettings(recorder);
			var recorderController = new RecorderController(settings);

			RecorderOptions.VerboseMode = false; // Make sure visibility is not toggled on because of debugMode.

            recorderController.PrepareRecording();
			Assert.IsTrue(recorderController.StartRecording());
			Assert.IsTrue(recorderController.IsRecording());
		
			var gameObject =  GameObject.Find("Unity-RecorderSessions");
			
			Assert.IsNotNull(gameObject);
			Assert.IsTrue(gameObject.hideFlags == HideFlags.None);
			
			recorderController.StopRecording();
			
			Object.DestroyImmediate(recorder);
			Object.DestroyImmediate(settings);
		}
	}
}
