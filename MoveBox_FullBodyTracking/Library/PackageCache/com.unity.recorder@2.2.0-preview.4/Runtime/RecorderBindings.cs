using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityObject = UnityEngine.Object;

namespace UnityEngine.Recorder
{
    /// <summary>
    /// Helper component mainly used to save Recorder's GameObject references.
    /// Some recorders (such as the Animation Recorder) require a GameObject reference from a Scene to record from.
    /// In such a case, this component is automatically added to the Scene and binds the selected GameObject to the Recorder Settings.
    /// </summary>
    [ExecuteInEditMode]
    public class RecorderBindings : MonoBehaviour
    {
        [Serializable]
        class PropertyObjects : SerializedDictionary<string, UnityObject> { }

        [SerializeField] PropertyObjects m_References = new PropertyObjects();

        /// <summary>
        /// Binds a particular ID to an Object value.
        /// </summary>
        /// <param name="id">The unique ID key.</param>
        /// <param name="value">The value for the key.</param>
        public void SetBindingValue(string id, UnityObject value)
        {
#if UNITY_EDITOR
            var dirty = !m_References.dictionary.ContainsKey(id) || m_References.dictionary[id] != value;
#endif
            m_References.dictionary[id] = value;

#if UNITY_EDITOR
            if (dirty)
                MarkSceneDirty();
#endif
        }

        /// <summary>
        /// Retrieves a binding for an unique ID.
        /// </summary>
        /// <param name="id">The binding key.</param>
        /// <returns>The value corresponding to the key – or null if the key doesn't exist.</returns>
        public UnityObject GetBindingValue(string id)
        {
            UnityObject value;
            return m_References.dictionary.TryGetValue(id, out value) ? value : null;
        }

        /// <summary>
        /// Tests if a value exists for a given key.
        /// </summary>
        /// <param name="id">The key to test.</param>
        /// <returns>True if the key value exists, False otherwise.</returns>
        public bool HasBindingValue(string id)
        {
            return m_References.dictionary.ContainsKey(id);
        }

        /// <summary>
        /// Removes the binding for a key.
        /// </summary>
        /// <param name="id">The unique key of the binding to remove.</param>
        public void RemoveBinding(string id)
        {
            if (m_References.dictionary.ContainsKey(id))
            {
                m_References.dictionary.Remove(id);

                MarkSceneDirty();
            }
        }

        /// <summary>
        /// Tests if any bindings exist.
        /// </summary>
        /// <returns>True if there are any existing bindings, False otherwise.</returns>
        public bool IsEmpty()
        {
            return m_References == null || !m_References.dictionary.Keys.Any();
        }

        /// <summary>
        /// Duplicates the binding from an existing key to a new one (makes dst point to the same object as src).
        /// </summary>
        /// <param name="src">The key to duplicate the binding from.</param>
        /// <param name="dst">The new key that points to the same object.</param>
        public void DuplicateBinding(string src, string dst)
        {
            if (m_References.dictionary.ContainsKey(src))
            {
                m_References.dictionary[dst] = m_References.dictionary[src];

                MarkSceneDirty();
            }
        }

        void MarkSceneDirty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
        }
    }
}
