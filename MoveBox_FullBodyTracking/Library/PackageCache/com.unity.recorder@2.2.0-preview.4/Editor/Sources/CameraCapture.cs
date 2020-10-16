using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Recorder.Input
{
    internal static class CameraCapture
    {
        private static Dictionary<Camera, HashSet<Action<RenderTargetIdentifier, CommandBuffer> > > actionDict =
            new Dictionary<Camera, HashSet<Action<RenderTargetIdentifier, CommandBuffer> > >();

        public static IEnumerator<Action<RenderTargetIdentifier, CommandBuffer> > GetActions(Camera camera)
        {
            HashSet<Action<RenderTargetIdentifier, CommandBuffer> > actions;
            if (!actionDict.TryGetValue(camera, out actions))
                return null;

            return actions.GetEnumerator();
        }

        public static void AddCaptureAction(Camera camera, Action<RenderTargetIdentifier, CommandBuffer> action)
        {
            HashSet<Action<RenderTargetIdentifier, CommandBuffer> > actions = null;
            actionDict.TryGetValue(camera, out actions);
            if (actions == null)
            {
                actions = new HashSet<Action<RenderTargetIdentifier, CommandBuffer> >();
                actionDict.Add(camera, actions);
            }
            actions.Add(action);
        }

        public static void RemoveCaptureAction(Camera camera, Action<RenderTargetIdentifier, CommandBuffer> action)
        {
            if (camera == null)
                return;

            HashSet<Action<RenderTargetIdentifier, CommandBuffer> > actions;
            if (actionDict.TryGetValue(camera, out actions))
                actions.Remove(action);
        }
    }
}
