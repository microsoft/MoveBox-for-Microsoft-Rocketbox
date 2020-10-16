using System;

namespace UnityEditor.Recorder
{
    public class RecorderInput : IDisposable
    {
        public RecorderInputSettings settings { get; set; }

        ~RecorderInput()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
        }

        protected internal virtual void SessionCreated(RecordingSession session) {}

        protected internal virtual void BeginRecording(RecordingSession session) {}

        protected internal virtual void NewFrameStarting(RecordingSession session) {}

        protected internal virtual void NewFrameReady(RecordingSession session) {}

        protected internal virtual void FrameDone(RecordingSession session) {}

        protected internal virtual void EndRecording(RecordingSession session) {}
    }
}
