using System;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class TextureFlipper : IDisposable
    {
        internal RenderTexture workTexture { private set; get; }
        internal bool inPlace { set; get; }

        internal TextureFlipper(bool flipInPlace = true)
        {
            inPlace = flipInPlace;
        }

        internal void Init(RenderTexture template)
        {
            UnityHelpers.Destroy(workTexture);
            workTexture = new RenderTexture(template);
        }

        internal void Flip(RenderTexture target)
        {
            if (workTexture == null || workTexture.width != target.width || workTexture.height != target.height)
                Init(target);

            var sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = PlayerSettings.colorSpace == ColorSpace.Linear;
            
            Graphics.Blit(target, workTexture, new Vector2(1.0f, -1.0f), new Vector2(0.0f, 1.0f));
            if (inPlace)
                Graphics.Blit(workTexture, target);

            GL.sRGBWrite = sRGBWrite;
        }

        public void Dispose()
        {
            UnityHelpers.Destroy(workTexture);
            workTexture = null;
        }
    }
}
