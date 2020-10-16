using System;
using System.Collections.Generic;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace UnityEditor.Recorder
{
    [RecorderSettings(typeof(ImageRecorder), "Image Sequence", "imagesequence_16")]
    public class ImageRecorderSettings : RecorderSettings
    {
        /// <summary>
        /// Available options for the output image format used by Image Sequence Recorder.
        /// </summary>
        public enum ImageRecorderOutputFormat
        {
            /// <summary>
            /// Output the recording in PNG format.
            /// </summary>
            PNG,
            /// <summary>
            /// Output the recording in JPEG format.
            /// </summary>
            JPEG,
            /// <summary>
            /// Output the recording in EXR format.
            /// </summary>
            EXR
        }

        /// <summary>
        /// Stores the output image format currently used for this Recorder.
        /// </summary>
        public ImageRecorderOutputFormat OutputFormat
        {
            get { return outputFormat; }
            set { outputFormat = value; }
        }

        [SerializeField] ImageRecorderOutputFormat outputFormat = ImageRecorderOutputFormat.JPEG;

        /// <summary>
        /// Use this property to capture the alpha channel (True) or not (False).
        /// </summary>
        /// <remarks>
        /// Alpha channel is captured only if the output image format supports it.
        /// </remarks>
        public bool CaptureAlpha
        {
            get { return captureAlpha; }
            set { captureAlpha = value; }
        }

        [SerializeField] private bool captureAlpha;


        /// <summary>
        /// Use this property to capture the frames in HDR (if the setup supports it).
        /// </summary>
        public bool CaptureHDR
        {
            get { return captureHDR; }
            set { captureHDR = value; }
        }

        [SerializeField] private bool captureHDR;

        [SerializeField] ImageInputSelector m_ImageInputSelector = new ImageInputSelector();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ImageRecorderSettings()
        {
            fileNameGenerator.FileName = "image_" + DefaultWildcard.Frame;
        }

        /// <inheritdoc/>
        protected internal override string Extension
        {
            get
            {
                switch (OutputFormat)
                {
                    case ImageRecorderOutputFormat.PNG:
                        return "png";
                    case ImageRecorderOutputFormat.JPEG:
                        return "jpg";
                    case ImageRecorderOutputFormat.EXR:
                        return "exr";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ImageInputSettings imageInputSettings
        {
            get { return m_ImageInputSelector.ImageInputSettings; }
            set { m_ImageInputSelector.ImageInputSettings = value; }
        }

        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = base.ValidityCheck(errors);

            if(string.IsNullOrEmpty(fileNameGenerator.FileName))
            {
                ok = false;
                errors.Add("missing file name");
            }

            return ok;
        }

        public override IEnumerable<RecorderInputSettings> InputsSettings
        {
            get { yield return m_ImageInputSelector.Selected; }
        }

        internal bool CanCaptureHDRFrames()
        {
            bool isGameViewInput = imageInputSettings.InputType == typeof(GameViewInput);
            bool isFormatExr = OutputFormat == ImageRecorderOutputFormat.EXR;
            return !isGameViewInput && isFormatExr && CameraInputSettings.UsingHDRP();
        }

        internal bool CanCaptureAlpha()
        {
            bool formatSupportAlpha = OutputFormat == ImageRecorderOutputFormat.PNG ||
                                      OutputFormat == ImageRecorderOutputFormat.EXR;
            bool inputSupportAlpha = imageInputSettings.SupportsTransparent;
            return (formatSupportAlpha && inputSupportAlpha && !CameraInputSettings.UsingHDRP());
        }

        internal override void SelfAdjustSettings()
        {
            var input = m_ImageInputSelector.Selected;

            if (input == null)
                return;
            var renderTextureSamplerSettings = input as RenderTextureSamplerSettings;
            if (renderTextureSamplerSettings != null)
            {
                var colorSpace = OutputFormat == ImageRecorderOutputFormat.EXR ? ColorSpace.Linear : ColorSpace.Gamma;
                renderTextureSamplerSettings.ColorSpace = colorSpace;
            }

            var cbis = input as CameraInputSettings;
            if (cbis != null)
            {
                cbis.AllowTransparency = CanCaptureAlpha() && CaptureAlpha;
            }

            var gis = input as GameViewInputSettings;
            if (gis != null)
                gis.FlipFinalOutput = SystemInfo.supportsAsyncGPUReadback;
        }
    }
}
