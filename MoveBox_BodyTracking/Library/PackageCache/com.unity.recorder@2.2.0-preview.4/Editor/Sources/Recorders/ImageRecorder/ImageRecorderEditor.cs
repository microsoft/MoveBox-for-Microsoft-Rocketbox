using UnityEditor.Recorder.Input;
using UnityEngine;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(ImageRecorderSettings))]
    class ImageRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;
        SerializedProperty m_CaptureAlpha;
        SerializedProperty m_CaptureHDR;

        static class Styles
        {
            internal static readonly GUIContent FormatLabel = new GUIContent("Format");
            internal static readonly GUIContent CaptureAlphaLabel = new GUIContent("Capture Alpha");
            internal static readonly GUIContent CaptureHDRLabel = new GUIContent("Capture Frames in HDR");
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            var pf = new PropertyFinder<ImageRecorderSettings>(serializedObject);
            m_OutputFormat = pf.Find(w => w.OutputFormat);
            
            m_OutputFormat = serializedObject.FindProperty("outputFormat");
            m_CaptureAlpha = serializedObject.FindProperty("captureAlpha");
            m_CaptureHDR = serializedObject.FindProperty("captureHDR");
        }

        protected override void FileTypeAndFormatGUI()
        {           
            EditorGUILayout.PropertyField(m_OutputFormat, Styles.FormatLabel);
            var imageSettings = (ImageRecorderSettings) target;
            if (!CameraInputSettings.UsingHDRP())
            {
                using (new EditorGUI.DisabledScope(!imageSettings.CanCaptureAlpha()))
                {
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.PropertyField(m_CaptureAlpha, Styles.CaptureAlphaLabel);
                    --EditorGUI.indentLevel;
                }
            }

            using( new EditorGUI.DisabledScope(!imageSettings.CanCaptureHDRFrames()))
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_CaptureHDR, Styles.CaptureHDRLabel);
                --EditorGUI.indentLevel;
            }
        }
    }
}
