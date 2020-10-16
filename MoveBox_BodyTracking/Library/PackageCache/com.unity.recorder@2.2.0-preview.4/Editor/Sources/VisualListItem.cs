using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;
#endif

namespace UnityEditor.Recorder
{   
    class VisualListItem<T> : VisualElement where T : VisualElement
    {
        public event Action OnSelectionChanged;
        public event Action OnContextMenu;
        public event Action<T> OnItemContextMenu;
        public event Action<T> OnItemRename;

        int m_SelectionIndex;

        public int selectedIndex
        {
            get { return m_SelectionIndex; }
            set
            {
                m_SelectionIndex = value;
                RecorderOptions.selectedRecorderIndex = value;
                if (OnSelectionChanged != null)
                    OnSelectionChanged.Invoke();
            }
        }

        readonly ScrollView m_ScrollView;
        readonly List<T> m_ItemsCache = new List<T>();

        protected VisualListItem()
        {
            m_ScrollView = new ScrollView
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };
            
            UIElementHelper.SetFlex(m_ScrollView, 1.0f);
            UIElementHelper.ResetStylePosition(m_ScrollView.contentContainer.style);

            Add(m_ScrollView);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            m_SelectionIndex = RecorderOptions.selectedRecorderIndex;
        }

        public void Reload(IEnumerable<T> itemList)
        {
            m_ScrollView.Clear();
            m_ItemsCache.Clear();

            foreach (var item in itemList)
                Add(item);

            if (m_SelectionIndex < 0.0f || m_SelectionIndex >= itemList.Count())
            {
                m_SelectionIndex = itemList.Any() ? 0 : -1;
            }

            // Force OnSelectionChange call
            selectedIndex = m_SelectionIndex;
        }
        
        public List<T> items
        {
            get { return m_ItemsCache; }
        }

        public T selection
        {
            get
            {
                if(selectedIndex < 0 || selectedIndex > m_ItemsCache.Count - 1)
                    return null;
                
                return m_ItemsCache[selectedIndex];
            }
            
            set
            {
                if (selection == value)
                    return;

                selectedIndex = m_ItemsCache.IndexOf(value);
            }
        }

        public void Add(T item)
        {
            item.RegisterCallback<MouseDownEvent>(OnItemMouseDown);
            item.RegisterCallback<MouseUpEvent>(OnItemMouseUp);
            m_ScrollView.Add(item);
            m_ItemsCache.Add(item);
        }
        
        public void Remove(T item)
        {
            var selected = selection == item;
            
            m_ScrollView.Remove(item);
            m_ItemsCache.Remove(item);

            if (selected)
                selectedIndex = Math.Min(selectedIndex, items.Count - 1);
        }
        
        void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.clickCount != 1)
                return;
            
            if (evt.button == (int) MouseButton.RightMouse)
            {
                if (OnContextMenu != null)
                    OnContextMenu.Invoke();
            }
            
            evt.StopImmediatePropagation();
        }

        public bool HasFocus()
        {
            return focusController.focusedElement == this;
        }
        
        void OnItemMouseDown(MouseDownEvent evt)
        {           
            if (evt.clickCount != 1)
                return;

            if (evt.button != (int) MouseButton.LeftMouse && evt.button != (int) MouseButton.RightMouse)
                return;

            var item = (T) evt.currentTarget;
            
            if (evt.modifiers == EventModifiers.None)
            {
                var alreadySelected = selection == item;
                if (evt.button == (int) MouseButton.LeftMouse && alreadySelected)
                {
                    if (HasFocus() && OnItemRename != null)
                        OnItemRename.Invoke(item);
                }
                else
                {
                    selection = item;
                }
            }
            
            evt.StopImmediatePropagation();
        }
        
        void OnItemMouseUp(MouseUpEvent evt)
        {           
            if (evt.clickCount != 1)
                return;

            if (evt.modifiers != EventModifiers.None || evt.button != (int) MouseButton.RightMouse)
                return;

            if (OnItemContextMenu != null)
            {
                var item = (T) evt.currentTarget;
                OnItemContextMenu.Invoke(item);
            }

            evt.StopImmediatePropagation();
        }
    }
}
