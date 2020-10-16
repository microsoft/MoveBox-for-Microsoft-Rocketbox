using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Base class for all Recorder inputs that capture images.
    /// </summary>
    public abstract class BaseRenderTextureInput : RecorderInput
    {
        /// <summary>
        /// Enables asynchronous readback of GPU resources if the platform supports it.
        /// Set this property to a valid instance and ensure that ReadbackTexture is not set.
        /// </summary>
        protected internal RenderTexture OutputRenderTexture { get; set; }

        /// <summary>
        /// Indicates the synchronous GPU readback destination.
        /// </summary>
        protected internal Texture2D ReadbackTexture { get; set; }

        /// <summary>
        /// Stores the output image width.
        /// </summary>
        public int OutputWidth { get; protected set; }
        /// <summary>
        /// Stores the output image height.
        /// </summary>
        public int OutputHeight { get; protected set; }

        /// <summary>
        /// Releases all resources allocated by this class instance.
        /// </summary>
        protected void ReleaseBuffer()
        {
            if (OutputRenderTexture != null)
            {
                if (OutputRenderTexture == RenderTexture.active)
                    RenderTexture.active = null;

                OutputRenderTexture.Release();
                OutputRenderTexture = null;
            }
        }

        /// <summary>
        /// Releases all resources allocated by this instance.
        /// </summary>
        /// <param name="disposing">If true, releases buffers allocated by this class as well.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                ReleaseBuffer();
        }
    }
}
