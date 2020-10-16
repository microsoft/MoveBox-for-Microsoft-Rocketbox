using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.Recorder
{
    /// <summary>
    /// Class that allows building file paths relative.
    /// </summary>
    [Serializable]
    public class OutputPath
    {
        /// <summary>
        /// Options specifying which root location the output path is relative to (or if the path is absolute).
        /// </summary>
        public enum Root
        {
            /// <summary>
            /// Relative path to Project file (parent of Assets).
            /// </summary>
            Project,
            /// <summary>
            /// Relative path to Assets.
            /// </summary>
            AssetsFolder,
            /// <summary>
            /// Relative path to StreamingAssets.
            /// </summary>
            StreamingAssets,
            /// <summary>
            /// Relative path to PersistentData.
            /// </summary>
            PersistentData,
            /// <summary>
            /// Relative path to Temporary Cache.
            /// </summary>
            TemporaryCache,
            /// <summary>
            /// Absolute path.
            /// </summary>
            Absolute
        }

        [SerializeField] Root m_Root;
        [SerializeField] string m_Leaf;

        [SerializeField] bool m_ForceAssetFolder;

        internal Root root
        {
            get { return m_Root; }
            set { m_Root = value; }
        }

        internal string leaf
        {
            get { return m_Leaf; }
            set { m_Leaf = value; }
        }

        internal bool forceAssetsFolder
        {
            get { return m_ForceAssetFolder;}
            set
            {
                m_ForceAssetFolder = value;

                if (m_ForceAssetFolder)
                    m_Root = Root.AssetsFolder;
            }
        }

        internal static OutputPath FromPath(string path)
        {
            var result = new OutputPath();

            if (path.Contains(Application.streamingAssetsPath))
            {
                result.m_Root = Root.StreamingAssets;
                result.m_Leaf = path.Replace(Application.streamingAssetsPath, string.Empty);
            }
            else if (path.Contains(Application.dataPath))
            {
                result.m_Root = Root.AssetsFolder;
                result.m_Leaf = path.Replace(Application.dataPath, string.Empty);
            }
            else if (path.Contains(Application.persistentDataPath))
            {
                result.m_Root = Root.PersistentData;
                result.m_Leaf = path.Replace(Application.persistentDataPath, string.Empty);
            }
            else if (path.Contains(Application.temporaryCachePath))
            {
                result.m_Root = Root.TemporaryCache;
                result.m_Leaf = path.Replace(Application.temporaryCachePath, string.Empty);
            }
            else if (path.Contains(ProjectPath()))
            {
                result.m_Root = Root.Project;
                result.m_Leaf = path.Replace(ProjectPath(), string.Empty);
            }
            else
            {
                result.m_Root = Root.Absolute;
                result.m_Leaf = path;
            }

            return result;
        }

        internal static string GetFullPath(Root root, string leaf)
        {
            var ret = string.Empty;
            switch (root)
            {
                case Root.PersistentData:
                    ret = Application.persistentDataPath;
                    break;
                case Root.StreamingAssets:
                    ret = Application.streamingAssetsPath;
                    break;
                case Root.TemporaryCache:
                    ret = Application.temporaryCachePath;
                    break;
                case Root.AssetsFolder:
                    ret = Application.dataPath;
                    break;
                case Root.Project:
                    ret = ProjectPath();
                    break;
            }

            if (root != Root.Absolute && !leaf.StartsWith("/"))
            {
                ret += "/";
            }
            ret += leaf;
            return ret;
        }

        internal string GetFullPath()
        {
            return GetFullPath(m_Root, m_Leaf);
        }

        static string ProjectPath()
        {
            return Regex.Replace(Application.dataPath, "/Assets$", string.Empty);
        }
    }
}
