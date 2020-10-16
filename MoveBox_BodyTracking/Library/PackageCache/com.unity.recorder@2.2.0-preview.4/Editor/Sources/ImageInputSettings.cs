using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    /// <inheritdoc />
    /// <summary>
    /// Optional base class for image related inputs.
    /// </summary>
    public abstract class ImageInputSettings : RecorderInputSettings
    {
        /// <summary>
        /// Stores the output image width.
        /// </summary>
        public abstract int OutputWidth { get; set; }
        /// <summary>
        /// Stores the output image height.
        /// </summary>
        public abstract int OutputHeight { get; set; }

        /// <summary>
        /// Indicates if derived classes support transparency.
        /// </summary>
        public virtual bool SupportsTransparent
        {
            get { return true; }
        }


        internal bool AllowTransparency;
    }

    /// <inheritdoc />
    /// <summary>
    /// This class regroups settings required to specify the size of an image input using a size and an aspect ratio.
    /// </summary>
    [Serializable]
    public abstract class StandardImageInputSettings : ImageInputSettings
    {
        [SerializeField] OutputResolution m_OutputResolution = new OutputResolution();

        internal bool forceEvenSize;

        /// <inheritdoc />
        public override int OutputWidth
        {
            get { return ForceEvenIfNecessary(m_OutputResolution.GetWidth()); }
            set { m_OutputResolution.SetWidth(ForceEvenIfNecessary(value)); }
        }

        /// <inheritdoc />
        public override int OutputHeight
        {
            get { return ForceEvenIfNecessary(m_OutputResolution.GetHeight()); }
            set { m_OutputResolution.SetHeight(ForceEvenIfNecessary(value)); }
        }

        internal ImageHeight outputImageHeight
        {
            get { return m_OutputResolution.imageHeight; }
            set { m_OutputResolution.imageHeight = value; }
        }

        internal ImageHeight maxSupportedSize
        {
            get { return m_OutputResolution.maxSupportedHeight; }
            set { m_OutputResolution.maxSupportedHeight = value; }
        }

        int ForceEvenIfNecessary(int v)
        {
            if (forceEvenSize && outputImageHeight != ImageHeight.Custom)
                return (v + 1) & ~1;

            return v;
        }

        /// <inheritdoc />
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            var h = OutputHeight;

            if (h > (int) maxSupportedSize)
            {
                ok = false;
                errors.Add("Output size exceeds maximum supported size: " + (int) maxSupportedSize );
            }

            var w = OutputWidth;
            if (w <= 0 || h <= 0)
            {
                ok = false;
                errors.Add("Invalid output resolution: " + w + "x" + h);
            }

            return ok;
        }
    }
}
