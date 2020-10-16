using UnityEngine;

namespace UnityEditor.Recorder
{
    class RecorderComponent : _FrameRequestComponent
    {        
        public RecordingSession session { get; set; }

        public void Update()
        {
            if (session != null && session.isRecording)
            {
                session.PrepareNewFrame();
            }
        }

        public void LateUpdate()
        {
            if (session != null && session.isRecording)
            {
                RequestNewFrame();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (session != null)
                session.Dispose();
        }

        protected override void FrameReady()
        {
            if (session.prepareFrameCalled)
            {
                session.RecordFrame();

                switch (session.recorder.settings.RecordMode)
                {
                    case RecordMode.Manual:
                        break;
                    case RecordMode.SingleFrame:
                    {
                        if (session.recorder.RecordedFramesCount == 1)
                            Destroy(this);
                        break;
                    }
                    case RecordMode.FrameInterval:
                    {
                        if (session.frameIndex > session.settings.EndFrame)
                            Destroy(this);
                        break;
                    }
                    case RecordMode.TimeInterval:
                    {
                        if (session.settings.FrameRatePlayback == FrameRatePlayback.Variable)
                        {
                            if (session.currentFrameStartTS >= session.settings.EndTime)
                                Destroy(this);
                        }
                        else
                        {
                            var expectedFrames = (session.settings.EndTime - session.settings.StartTime) * session.settings.FrameRate;
                            if (session.RecordedFrameSpan >= expectedFrames)
                                Destroy(this);
                        }
                        break;
                    }
                }
            }
        }
    }
}
