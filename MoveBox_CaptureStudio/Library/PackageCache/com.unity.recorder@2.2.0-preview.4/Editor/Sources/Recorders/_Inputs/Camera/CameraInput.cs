using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Use this class to record from a Scene Camera.
    /// </summary>
    public class CameraInput : BaseRenderTextureInput
    {
        struct CanvasBackup
        {
            public Camera camera;
            public Canvas canvas;
        }

        private InputStrategy  m_InputStrategy;
        private bool           m_ModifiedResolution;
        private TextureFlipper m_VFlipper;
        private Camera         m_UICamera;
        private CanvasBackup[] m_CanvasBackups;

        private abstract class InputStrategy
        {
            private readonly bool m_CaptureAlpha;
            private Camera        m_Camera;
            private bool          m_CameraChanged;
            private Shader        m_CopyShader;
            private Material      m_CopyMaterial;
            private RenderTexture m_RenderTexture;

            public Camera targetCamera
            {
                get { return m_Camera; }
                set
                {
                    m_CameraChanged = value != targetCamera;
                    if (m_CameraChanged)
                    {
                        ReleaseCamera();
                        m_Camera = value;
                    }
                }
            }

            public void SetupCamera(RenderTexture inRenderTexture)
            {
                if (targetCamera == null)
                    return;

                var force = inRenderTexture != m_RenderTexture;
                m_RenderTexture = inRenderTexture;

                // initialize command buffer
                if (m_CameraChanged || force)
                {
                    SetupCommandBuffer(m_RenderTexture);
                    m_CameraChanged = false;
                }

                if (Math.Abs(1 - targetCamera.rect.width) > float.Epsilon ||
                    Math.Abs(1 - targetCamera.rect.height) > float.Epsilon)
                    Debug.LogWarning(
                        $"Recording output of camera '{targetCamera.gameObject.name}' who's rectangle does not cover " +
                        "the viewport: resulting image will be up-sampled with associated quality " + "degradation!");
            }

            public virtual void UnsetupCamera() {}

            public virtual void ReleaseCamera()
            {
                UnityHelpers.Destroy(m_CopyMaterial);
            }

            protected abstract void SetupCommandBuffer(RenderTexture renderTexture);

            protected InputStrategy(bool captureAlpha)
            {
                m_CaptureAlpha = captureAlpha;
            }

            protected void AddCaptureCommands(RenderTargetIdentifier source, CommandBuffer cb)
            {
                if (source == BuiltinRenderTextureType.CurrentActive)
                {
                    var tid = Shader.PropertyToID("_MainTex");
                    cb.GetTemporaryRT(tid, m_RenderTexture.width, m_RenderTexture.height, 0, FilterMode.Bilinear);
                    cb.Blit(source, tid);
                    cb.Blit(tid, m_RenderTexture, copyMaterial);
                    cb.ReleaseTemporaryRT(tid);
                }
                else
                    cb.Blit(source, m_RenderTexture, copyMaterial);
            }

            private Material copyMaterial
            {
                get
                {
                    if (m_CopyMaterial == null)
                    {
                        m_CopyMaterial = new Material(copyShader);
                        if (m_CaptureAlpha)
                            m_CopyMaterial.EnableKeyword("TRANSPARENCY_ON");
                        if (!Options.useCameraCaptureCallbacks)
                            m_CopyMaterial.EnableKeyword("VERTICAL_FLIP");
                    }
                    return m_CopyMaterial;
                }
            }

            private Shader copyShader
            {
                get
                {
                    if (m_CopyShader == null)
                        m_CopyShader = Shader.Find("Hidden/Recorder/Inputs/CameraInput/Copy");
                    return m_CopyShader;
                }
            }
        }

        private class CaptureCallbackInputStrategy : InputStrategy
        {
            public CaptureCallbackInputStrategy(bool captureAlpha) : base(captureAlpha) {}

            protected override void SetupCommandBuffer(RenderTexture renderTexture)
            {
                CameraCapture.AddCaptureAction(targetCamera, AddCaptureCommands);
            }

            public override void ReleaseCamera()
            {
                CameraCapture.RemoveCaptureAction(targetCamera, AddCaptureCommands);
                base.ReleaseCamera();
            }
        }

        private class CameraCommandBufferInputStrategy : InputStrategy
        {
            private CommandBuffer m_cbCopyFB;

            public CameraCommandBufferInputStrategy(bool captureAlpha) : base(captureAlpha) {}

            protected override void SetupCommandBuffer(RenderTexture renderTexture)
            {
                if (m_cbCopyFB != null)
                {
                    targetCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_cbCopyFB);
                    m_cbCopyFB.Release();
                }

                m_cbCopyFB = new CommandBuffer {name = "Recorder: copy frame buffer"};
                AddCaptureCommands(BuiltinRenderTextureType.CurrentActive, m_cbCopyFB);
                targetCamera.AddCommandBuffer(CameraEvent.AfterEverything, m_cbCopyFB);
            }

            public override void ReleaseCamera()
            {
                if (m_cbCopyFB != null)
                {
                    if (targetCamera != null)
                        targetCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, m_cbCopyFB);
                    m_cbCopyFB.Release();
                    m_cbCopyFB = null;
                }

                base.ReleaseCamera();
            }
        }

        CameraInputSettings cbSettings
        {
            get { return (CameraInputSettings)settings; }
        }

        /// <summary>
        /// Indicates the Camera to use to record the Scene.
        /// </summary>
        protected Camera TargetCamera
        {
            get { return m_InputStrategy.targetCamera; }
            set { m_InputStrategy.targetCamera = value; }
        }

        /// <inheritdoc/>
        protected internal override void BeginRecording(RecordingSession session)
        {
            if (cbSettings.FlipFinalOutput)
                m_VFlipper = new TextureFlipper();

            if ( CameraInputSettings.UsingLegacyRP())
                m_InputStrategy = new CameraCommandBufferInputStrategy(cbSettings.AllowTransparency);
            else
                m_InputStrategy = new CaptureCallbackInputStrategy(cbSettings.AllowTransparency);

            switch (cbSettings.Source)
            {
                case ImageSource.ActiveCamera:
                case ImageSource.MainCamera:
                case ImageSource.TaggedCamera:
                {
                    OutputWidth = cbSettings.OutputWidth;
                    OutputHeight = cbSettings.OutputHeight;

                    if (cbSettings.outputImageHeight != ImageHeight.Window)
                    {
                        var size = GameViewSize.SetCustomSize(OutputWidth, OutputHeight);
                        if (size == null)
                            size = GameViewSize.AddSize(OutputWidth, OutputHeight);

                        if (GameViewSize.modifiedResolutionCount == 0)
                            GameViewSize.BackupCurrentSize();
                        else
                        {
                            if (size != GameViewSize.currentSize)
                                Debug.LogError("Requesting a resolution change while a recorder's input has already requested one! Undefined behaviour.");
                        }
                        GameViewSize.modifiedResolutionCount++;
                        m_ModifiedResolution = true;
                        GameViewSize.SelectSize(size);
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (cbSettings.CaptureUI)
            {
                var uiGO = new GameObject();
                uiGO.name = "UICamera";
                uiGO.transform.parent = session.recorderGameObject.transform;

                m_UICamera = uiGO.AddComponent<Camera>();
                m_UICamera.cullingMask = 1 << 5;
                m_UICamera.clearFlags = CameraClearFlags.Depth;
                m_UICamera.renderingPath = RenderingPath.DeferredShading;
                m_UICamera.targetTexture = OutputRenderTexture;
                m_UICamera.enabled = false;
            }
        }

        /// <inheritdoc/>
        protected internal override void NewFrameStarting(RecordingSession session)
        {
            m_InputStrategy.UnsetupCamera();

            switch (cbSettings.Source)
            {
                case ImageSource.ActiveCamera:
                {
                    if (TargetCamera == null)
                    {
                        var displayGO = new GameObject();
                        displayGO.name = "CameraHostGO-" + displayGO.GetInstanceID();
                        displayGO.transform.parent = session.recorderGameObject.transform;
                        var camera = displayGO.AddComponent<Camera>();
                        camera.clearFlags = CameraClearFlags.Nothing;
                        camera.cullingMask = 0;
                        camera.renderingPath = RenderingPath.DeferredShading;
                        camera.targetDisplay = 0;
                        camera.rect = new Rect(0, 0, 1, 1);
                        camera.depth = float.MaxValue;

                        TargetCamera = camera;
                    }
                    break;
                }

                case ImageSource.MainCamera:
                {
                    TargetCamera = Camera.main;
                    break;
                }
                case ImageSource.TaggedCamera:
                {
                    var tag = ((CameraInputSettings) settings).CameraTag;

                    if (TargetCamera == null || !TargetCamera.gameObject.CompareTag(tag))
                    {
                        try
                        {
                            var objs = GameObject.FindGameObjectsWithTag(tag);

                            var cams = objs.Select(obj => obj.GetComponent<Camera>()).Where(c => c != null);
                            if (cams.Count() > 1)
                                Debug.LogWarning("More than one camera has the requested target tag '" + tag + "'");

                            TargetCamera = cams.FirstOrDefault();
                        }
                        catch (UnityException)
                        {
                            Debug.LogWarning("No camera has the requested target tag '" + tag + "'");
                            TargetCamera = null;
                        }
                    }
                    break;
                }
            }

            PrepFrameRenderTexture(session);
            m_InputStrategy.SetupCamera(OutputRenderTexture);
        }

        /// <inheritdoc/>
        protected internal override void NewFrameReady(RecordingSession session)
        {
            if (cbSettings.CaptureUI)
            {
                // Find canvases
                var canvases = UnityObject.FindObjectsOfType<Canvas>();
                if (m_CanvasBackups == null || m_CanvasBackups.Length != canvases.Length)
                    m_CanvasBackups = new CanvasBackup[canvases.Length];

                // Hookup canvase to UI camera
                for (var i = 0; i < canvases.Length; i++)
                {
                    var canvas = canvases[i];
                    if (canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        m_CanvasBackups[i].camera = canvas.worldCamera;
                        m_CanvasBackups[i].canvas = canvas;
                        canvas.renderMode = RenderMode.ScreenSpaceCamera;
                        canvas.worldCamera = m_UICamera;
                    }
                    else
                    {
                        // Mark this canvas as null so we can skip it when restoring.
                        // The array might contain invalid data from a previous frame.
                        m_CanvasBackups[i].canvas = null;
                    }
                }

                m_UICamera.Render();

                // Restore canvas settings
                for (var i = 0; i < m_CanvasBackups.Length; i++)
                {
                    // Skip those canvases that are not roots canvases or are
                    // not using ScreenSpaceOverlay as a render mode.
                    if (m_CanvasBackups[i].canvas == null)
                        continue;

                    m_CanvasBackups[i].canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    m_CanvasBackups[i].canvas.worldCamera = m_CanvasBackups[i].camera;
                }
            }

            if (cbSettings.FlipFinalOutput)
                m_VFlipper.Flip(OutputRenderTexture);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_InputStrategy.ReleaseCamera();
                UnityHelpers.Destroy(m_UICamera);

                if (m_ModifiedResolution)
                {
                    GameViewSize.modifiedResolutionCount--;
                    if (GameViewSize.modifiedResolutionCount == 0)
                        GameViewSize.RestoreSize();
                }

                if (m_VFlipper != null)
                    m_VFlipper.Dispose();
            }

            base.Dispose(disposing);
        }

        void PrepFrameRenderTexture(RecordingSession session)
        {
            if (OutputRenderTexture != null)
            {
                if (OutputRenderTexture.IsCreated() && OutputRenderTexture.width == OutputWidth && OutputRenderTexture.height == OutputHeight)
                    return;

                ReleaseBuffer();
            }

            ImageRecorderSettings s = session.settings as ImageRecorderSettings;
            var fmtRW = RenderTextureReadWrite.Default;
            var fmt = RenderTextureFormat.ARGB32;
            if (s != null && s.CanCaptureHDRFrames() && s.CaptureHDR)
            {
                fmtRW = RenderTextureReadWrite.Linear;
                fmt = RenderTextureFormat.DefaultHDR;
            }

            OutputRenderTexture = new RenderTexture(OutputWidth, OutputHeight, 0, fmt, fmtRW)
            {
                wrapMode = TextureWrapMode.Repeat
            };
            OutputRenderTexture.Create();
            if (m_UICamera != null)
                m_UICamera.targetTexture = OutputRenderTexture;

            return;
        }
    }
}
