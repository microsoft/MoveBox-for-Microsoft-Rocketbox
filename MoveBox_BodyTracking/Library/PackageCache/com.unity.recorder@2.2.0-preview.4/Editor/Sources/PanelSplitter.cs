using System;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#else
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;
#endif


namespace UnityEditor.Recorder
{
    class PanelSplitter : VisualElement
    {
        readonly VisualElement m_AffectedElement;

        bool m_Grabbed;
        Vector2 m_GrabbedMousePosition;

        float m_ElementOriginalWidth;

        const float k_SplitterWidth = 5.0f;

        void SetWidth(float value)
        {
            m_AffectedElement.style.width = value;
            RecorderOptions.recorderPanelWith = value;
        }

        public PanelSplitter(VisualElement affectedElement)
        {
            m_AffectedElement = affectedElement;

            style.width = k_SplitterWidth;
            style.minWidth = k_SplitterWidth;
            style.maxWidth = k_SplitterWidth;

            UIElementHelper.RegisterTrickleDownCallback<MouseDownEvent>(this, OnMouseDown);
            UIElementHelper.RegisterTrickleDownCallback<MouseMoveEvent>(this, OnMouseMove);
            UIElementHelper.RegisterTrickleDownCallback<MouseUpEvent>(this, OnMouseUp);

            var w = RecorderOptions.recorderPanelWith;
            if (w > 0.0f)
                SetWidth(w);
        }

        void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != (int) MouseButton.LeftMouse)
                return;
            
            if (m_Grabbed)
                return;

#if UNITY_2018_3_OR_NEWER
            this.CaptureMouse();
#else
            this.TakeMouseCapture();
#endif

            m_Grabbed = true;
            m_GrabbedMousePosition = evt.mousePosition;

#if UNITY_2019_1_OR_NEWER
            m_ElementOriginalWidth = m_AffectedElement.resolvedStyle.width;
#else
            m_ElementOriginalWidth = m_AffectedElement.style.width;
#endif
            evt.StopImmediatePropagation();
        }

        void OnMouseMove(MouseMoveEvent evt)
        {
            if (!m_Grabbed)
                return;

            var delta = evt.mousePosition.x - m_GrabbedMousePosition.x;

            #if UNITY_2019_1_OR_NEWER
                var minWidth = m_AffectedElement.resolvedStyle.minWidth.value;
                var maxWidth = m_AffectedElement.resolvedStyle.maxWidth.value;
            #else
                var minWidth = m_AffectedElement.style.minWidth;
                var maxWidth = m_AffectedElement.style.maxWidth;
            #endif
            
            var newWidth = Mathf.Max(m_ElementOriginalWidth + delta, minWidth);
           
            if (maxWidth > 0.0f)
                newWidth = Mathf.Min(newWidth, maxWidth);

            SetWidth(newWidth);
        }
        
        void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.button != (int) MouseButton.LeftMouse)
                return;

            if (!m_Grabbed)
                return;

            m_Grabbed = false;

#if UNITY_2018_3_OR_NEWER
            this.ReleaseMouse();
#else
            this.ReleaseMouseCapture();
#endif
            
            evt.StopImmediatePropagation();
        }
    }
}