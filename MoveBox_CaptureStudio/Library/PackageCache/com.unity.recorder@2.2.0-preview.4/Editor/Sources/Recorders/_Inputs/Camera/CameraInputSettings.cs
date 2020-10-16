using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Use this class to manage all the information required to record from a Scene Camera.
    /// </summary>
    [DisplayName("Targeted Camera")]
    [Serializable]
    public class CameraInputSettings : StandardImageInputSettings
    {
        /// <summary>
        /// Indicates the Camera input type.
        /// </summary>
        public ImageSource Source
        {
            get { return source; }
            set { source = value; }
        }

        [SerializeField] private ImageSource source = ImageSource.MainCamera;

        /// <summary>
        /// Indicates the GameObject tag of the Camera used for the capture.
        /// </summary>
        public string CameraTag
        {
            get { return cameraTag; }
            set { cameraTag = value; }
        }

        [SerializeField] private string cameraTag;

        /// <summary>
        /// Use this property if you need to vertically flip the final output.
        /// </summary>
        public bool FlipFinalOutput
        {
            get { return flipFinalOutput; }
            set { flipFinalOutput = value; }
        }
        [SerializeField] private bool flipFinalOutput;

        /// <summary>
        /// Use this property to include the UI GameObjects to the recording.
        /// </summary>
        public bool CaptureUI
        {
            get { return captureUI; }
            set { captureUI = value; }
        }
        [SerializeField] private bool captureUI;
        internal static bool UsingHDRP()
        {
            var pipelineAsset = GraphicsSettings.renderPipelineAsset;
            var usingHDRP = pipelineAsset != null && pipelineAsset.GetType().FullName.Contains("High");
            return usingHDRP;
        }

        internal static bool UsingURP()
        {
            var pipelineAsset = GraphicsSettings.renderPipelineAsset;
            var usingURP = pipelineAsset != null &&
                           (pipelineAsset.GetType().FullName.Contains("Universal") ||
                            pipelineAsset.GetType().FullName.Contains("Lightweight"));
            return usingURP;
        }
        internal static bool UsingLegacyRP()
        {
            var pipelineAsset = GraphicsSettings.renderPipelineAsset;
            return pipelineAsset == null;
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CameraInputSettings()
        {
            outputImageHeight = ImageHeight.Window;
        }

        /// <inheritdoc/>
        protected internal override Type InputType
        {
            get { return typeof(CameraInput); }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = base.ValidityCheck(errors);
            if (Source == ImageSource.TaggedCamera && string.IsNullOrEmpty(CameraTag))
            {
                ok = false;
                errors.Add("Missing tag for camera selection");
            }

            return ok;
        }
    }
}
