using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Use this class to specify a particular input type (it allows for each recorder to support only a subset of the input settings).
    /// </summary>
    [Serializable]
    public abstract class InputSettingsSelector
    {
        [SerializeField] string m_Selected;

        readonly Dictionary<string, RecorderInputSettings> m_RecorderInputSettings = new Dictionary<string, RecorderInputSettings>();

        /// <summary>
        /// The currently selected RecorderInputSettings.
        /// </summary>
        public RecorderInputSettings Selected
        {
            get
            {
                if (string.IsNullOrEmpty(m_Selected) || !m_RecorderInputSettings.ContainsKey(m_Selected))
                    m_Selected = m_RecorderInputSettings.Keys.First();

                return m_RecorderInputSettings[m_Selected];
            }

            protected set
            {
                foreach (var field in InputSettingFields())
                {
                    var input = (RecorderInputSettings)field.GetValue(this);

                    if (input.GetType() == value.GetType())
                    {
                        field.SetValue(this, value);
                        m_Selected = field.Name;
                        m_RecorderInputSettings[m_Selected] = value;
                        break;
                    }
                }
            }
        }

        internal IEnumerable<FieldInfo> InputSettingFields()
        {
            return GetInputFields(GetType()).Where(f => typeof(RecorderInputSettings).IsAssignableFrom(f.FieldType));
        }

        internal static IEnumerable<FieldInfo> GetInputFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// The default constructor.
        /// </summary>
        protected internal InputSettingsSelector()
        {
            foreach (var field in InputSettingFields())
            {
                var input = (RecorderInputSettings)field.GetValue(this);
                m_RecorderInputSettings.Add(field.Name, input);
            }
        }
    }
}
