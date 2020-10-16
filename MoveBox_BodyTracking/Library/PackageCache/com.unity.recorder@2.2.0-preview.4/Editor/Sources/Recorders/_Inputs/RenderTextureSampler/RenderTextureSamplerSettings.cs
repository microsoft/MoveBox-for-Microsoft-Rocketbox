using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Sets which level of multisample anti-aliasing (MSAA) to use for the capture.
    /// </summary>
    public enum SuperSamplingCount
    {
        /// <summary>
        /// MSAA level 1
        /// </summary>
        X1 = 1,
        /// <summary>
        /// MSAA level 2
        /// </summary>
        X2 = 2,
        /// <summary>
        /// MSAA level 4
        /// </summary>
        X4 = 4,
        /// <summary>
        /// MSAA level 8
        /// </summary>
        X8 = 8,
        /// <summary>
        /// MSAA level 16
        /// </summary>
        X16 = 16,
    }

    [DisplayName("Texture Sampling")]
    [Serializable]
    public class RenderTextureSamplerSettings : ImageInputSettings
    {
        [SerializeField] internal ImageSource source = ImageSource.ActiveCamera;

        [SerializeField] int m_RenderWidth = 1280;
        [SerializeField] int m_RenderHeight = (int)ImageHeight.x720p_HD;

        [SerializeField] int m_OutputWidth = 1280;
        [SerializeField] int m_OutputHeight = (int)ImageHeight.x720p_HD;

        [SerializeField] internal AspectRatio outputAspectRatio = new AspectRatio();

        /// <summary>
        /// Indicates the multisample anti-aliasing (MSAA) level used for this Recorder input.
        /// </summary>
        public SuperSamplingCount SuperSampling
        {
            get { return superSampling; }
            set { superSampling = value; }
        }
        [SerializeField] SuperSamplingCount superSampling = SuperSamplingCount.X1;
        [SerializeField]internal float superKernelPower = 16f;
        [SerializeField]internal float superKernelScale = 1f;

        /// <summary>
        /// Stores the GameObject tag that defines the Camera used for the recording.
        /// </summary>
        public string CameraTag
        {
            get => cameraTag;
            set => cameraTag = value;
        }

        [SerializeField] string cameraTag;

        /// <summary>
        /// Indicates the color space used for this Recorder input.
        /// </summary>
        public ColorSpace ColorSpace
        {
            get { return colorSpace; }
            set { colorSpace = value; }
        }
        [SerializeField] ColorSpace colorSpace = ColorSpace.Gamma;

        /// <summary>
        /// Use this property if you need to vertically flip the final output.
        /// </summary>
        public bool FlipFinalOutput
        {
            get { return flipFinalOutput; }
            set { flipFinalOutput = value; }
        }
        [SerializeField] bool flipFinalOutput;

        internal readonly int kMaxSupportedSize = (int)ImageHeight.x2160p_4K;

        /// <inheritdoc/>
        protected internal override Type InputType
        {
            get { return typeof(RenderTextureSampler); }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            var h = OutputHeight;
            if (h > kMaxSupportedSize)
            {
                ok = false;
                errors.Add("Output size exceeds maximum supported size: " + kMaxSupportedSize);
            }

            return ok;
        }

        /// <inheritdoc/>
        public override int OutputWidth
        {
            get { return m_OutputWidth; }
            set { m_OutputWidth = Mathf.Min(16 * 1024, value); }
        }

        /// <inheritdoc/>
        public override int OutputHeight
        {
            get { return m_OutputHeight; }
            set { m_OutputHeight = value; }
        }

        /// <summary>
        /// Use this property to specify a different render width from the output (overscan).
        /// </summary>
        public int RenderWidth
        {
            get { return m_RenderWidth; }
            set { m_RenderWidth = value; }
        }

        /// <summary>
        /// Use this property to specify a different render height from the output (overscan).
        /// </summary>
        public int RenderHeight
        {
            get { return m_RenderHeight; }
            set { m_RenderHeight = value; }
        }
    }
}
