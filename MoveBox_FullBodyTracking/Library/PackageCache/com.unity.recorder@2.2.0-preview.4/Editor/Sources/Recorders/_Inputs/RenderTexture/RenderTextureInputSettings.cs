using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    [DisplayName("Render Texture Asset")]
    [Serializable]
    public class RenderTextureInputSettings : ImageInputSettings
    {
         [SerializeField] internal RenderTexture renderTexture;
         /// <summary>
         /// Indicates the render texture used for the capture.
         /// </summary>
         public RenderTexture RenderTexture
         {
             get { return renderTexture; }
             set { renderTexture = value; }
         }

         /// <summary>
        /// Use this property if you need to vertically flip the final output.
        /// </summary>
        public bool FlipFinalOutput
        {
            get { return flipFinalOutput; }
            set { flipFinalOutput = value; }
        }
        [SerializeField] private bool flipFinalOutput = false;

        /// <inheritdoc/>
        protected internal override Type InputType
        {
            get { return typeof(RenderTextureInput); }
        }

        /// <inheritdoc/>
        public override int OutputWidth
        {
            get { return renderTexture == null ? 0 : renderTexture.width; }
            set
            {
                if (renderTexture != null)
                    renderTexture.width = value;
            }
        }

        /// <inheritdoc/>
        public override int OutputHeight
        {
            get { return renderTexture == null ? 0 : renderTexture.height; }
            set
            {
                if (renderTexture != null)
                    renderTexture.height = value;
            }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (renderTexture == null)
            {
                ok = false;
                errors.Add("Missing source render texture object/asset.");
            }

            return ok;
        }
    }
}
