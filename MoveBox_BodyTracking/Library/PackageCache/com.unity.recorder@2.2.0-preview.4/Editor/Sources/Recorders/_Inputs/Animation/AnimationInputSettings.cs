using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    /// <inheritdoc/>
    /// <summary>
    /// Use this class to manage all the information required to record an Animation from a given GameObject.
    /// </summary>
    [Serializable]
    [DisplayName("Animation")]
    public class AnimationInputSettings : RecorderInputSettings
    {
        [SerializeField] string m_BindingId = null;

        /// <summary>
        /// Indicates the GameObject to record from.
        /// </summary>
        public GameObject gameObject
        {
            get
            {
                if (string.IsNullOrEmpty(m_BindingId))
                    return null;

                return BindingManager.Get(m_BindingId) as GameObject;
            }

            set
            {
                if (string.IsNullOrEmpty(m_BindingId))
                    m_BindingId = GenerateBindingId();

                BindingManager.Set(m_BindingId, value);
            }
        }

        /// <summary>
        /// Use this property to record all the gameObject hierarchy (True) or not (False).
        /// </summary>
        public bool Recursive
        {
            get => recursive;
            set => recursive = value;
        }

        [SerializeField] bool recursive = true;

        /// <summary>
        /// Adds a Component to record from the GameObject.
        /// </summary>
        /// <param name="componentType">The type of the Component.</param>
        public void AddComponentToRecord(Type componentType)
        {
            if (componentType == null)
                return;

            var typeName = componentType.AssemblyQualifiedName;
            if (!bindingTypeNames.Contains(typeName))
                bindingTypeNames.Add(typeName);
        }

        [SerializeField]
        internal List<string> bindingTypeNames = new List<string>();

        internal List<Type> bindingType
        {
            get
            {
                var ret = new List<Type>(bindingTypeNames.Count);
                foreach (var t in bindingTypeNames)
                {
                    ret.Add(Type.GetType(t));
                }
                return ret;
            }
        }

        /// <inheritdoc/>
        protected internal override Type InputType
        {
            get { return typeof(AnimationInput); }
        }

        /// <inheritdoc/>
        protected internal override bool ValidityCheck(List<string> errors)
        {
            var ok = true;

            if (bindingType.Count > 0 && bindingType.Any(x => typeof(MonoBehaviour).IsAssignableFrom(x) || typeof(ScriptableObject).IsAssignableFrom(x))
            )
            {
                ok = false;
                errors.Add("MonoBehaviours and ScriptableObjects are not supported inputs.");
            }

            return ok;
        }

        static string GenerateBindingId()
        {
            return GUID.Generate().ToString();
        }

        /// <summary>
        /// Duplicates the existing Scene binding key under a new unique key (useful when duplicating the Recorder input).
        /// </summary>
        public void DuplicateExposedReference()
        {
            if (string.IsNullOrEmpty(m_BindingId))
                return;

            var src = m_BindingId;
            var dst = GenerateBindingId();

            m_BindingId = dst;

            BindingManager.Duplicate(src, dst);
        }

        /// <summary>
        /// Removes the binding value for the current key.
        /// </summary>
        public void ClearExposedReference()
        {
            if (string.IsNullOrEmpty(m_BindingId))
                return;

            BindingManager.Set(m_BindingId, null);
        }
    }
}
