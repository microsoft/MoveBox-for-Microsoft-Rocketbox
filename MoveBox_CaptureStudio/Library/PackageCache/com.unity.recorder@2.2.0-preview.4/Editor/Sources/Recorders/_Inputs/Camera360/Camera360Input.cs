using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Recorder.Input
{
    class Camera360Input : BaseRenderTextureInput
    {
        bool m_ModifiedResolution;
        TextureFlipper m_VFlipper = new TextureFlipper();

        RenderTexture m_Cubemap1;
        RenderTexture m_Cubemap2;

        Camera360InputSettings settings360
        {
            get { return (Camera360InputSettings)settings; }
        }

        Camera targetCamera { get; set; }

        protected internal override void BeginRecording(RecordingSession session)
        {
            if (settings360.FlipFinalOutput)
                m_VFlipper = new TextureFlipper();
            
            OutputWidth = settings360.OutputWidth;
            OutputHeight = settings360.OutputHeight;
        }

        protected internal override void NewFrameStarting(RecordingSession session)
        {
            switch (settings360.Source)
            {
                case ImageSource.MainCamera:
                {
                    if (targetCamera != Camera.main )
                        targetCamera = Camera.main;
                    break;
                }

                case ImageSource.TaggedCamera:
                {
                    var tag = settings360.CameraTag;

                    if (targetCamera == null || !targetCamera.gameObject.CompareTag(tag) )
                    {
                        try
                        {
                            var cams = GameObject.FindGameObjectsWithTag(tag);
                            if (cams.Length > 0)
                                Debug.LogWarning("More than one camera has the requested target tag:" + tag);
                            targetCamera = cams[0].transform.GetComponent<Camera>();
                            
                        }
                        catch (UnityException)
                        {
                            Debug.LogWarning("No camera has the requested target tag:" + tag);
                            targetCamera = null;
                        }
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PrepFrameRenderTexture(session);

        }

        protected internal override void NewFrameReady(RecordingSession session)
        {
            var eyesEyeSepBackup = targetCamera.stereoSeparation;
            var eyeMaskBackup = targetCamera.stereoTargetEye;
            
            var sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = PlayerSettings.colorSpace == ColorSpace.Linear;
            
            if (settings360.RenderStereo)
            {
                targetCamera.stereoSeparation = settings360.StereoSeparation;
                targetCamera.stereoTargetEye = StereoTargetEyeMask.Both;
                targetCamera.RenderToCubemap(m_Cubemap1, 63, Camera.MonoOrStereoscopicEye.Left);
                targetCamera.stereoSeparation = settings360.StereoSeparation;
                targetCamera.stereoTargetEye = StereoTargetEyeMask.Both;
                targetCamera.RenderToCubemap(m_Cubemap2, 63, Camera.MonoOrStereoscopicEye.Right);
                
                m_Cubemap1.ConvertToEquirect(OutputRenderTexture, Camera.MonoOrStereoscopicEye.Left);
                m_Cubemap2.ConvertToEquirect(OutputRenderTexture, Camera.MonoOrStereoscopicEye.Right);
            }
            else
            {
                targetCamera.RenderToCubemap(m_Cubemap1, 63, Camera.MonoOrStereoscopicEye.Mono);
                m_Cubemap1.ConvertToEquirect(OutputRenderTexture);
            }
            
            if (settings360.FlipFinalOutput)
                m_VFlipper.Flip(OutputRenderTexture);
                
            targetCamera.stereoSeparation = eyesEyeSepBackup;
            targetCamera.stereoTargetEye = eyeMaskBackup;
            
            GL.sRGBWrite = sRGBWrite;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if( m_Cubemap1 )
                    UnityHelpers.Destroy(m_Cubemap1);
                
                if( m_Cubemap2 )
                    UnityHelpers.Destroy(m_Cubemap2);

                if( m_VFlipper!=null )
                    m_VFlipper.Dispose();
            }

            base.Dispose(disposing);
        }

        void PrepFrameRenderTexture(RecordingSession session)
        {
            if (OutputRenderTexture != null)
            {
                if (OutputRenderTexture.IsCreated() && OutputRenderTexture.width == OutputWidth && OutputRenderTexture.height == OutputHeight)
                {
                    return;
                }

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

           
            OutputRenderTexture = new RenderTexture(OutputWidth, OutputHeight, 24, fmt, fmtRW)
            {
                dimension = TextureDimension.Tex2D,
                antiAliasing = 1
            };
            
            m_Cubemap1 = new RenderTexture(settings360.MapSize, settings360.MapSize, 24, fmt, fmtRW)
            {
                dimension = TextureDimension.Cube
                
            };
            
            m_Cubemap2 = new RenderTexture(settings360.MapSize, settings360.MapSize, 24, fmt, fmtRW)
            {
                dimension = TextureDimension.Cube 
            };
        }
    }
}