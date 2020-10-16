using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.Recorder
{
    internal class Wildcard
    {
        readonly string m_Pattern;
        readonly string m_Label;
        readonly Func<RecordingSession, string> m_Resolver;

        public string pattern { get { return m_Pattern; } }
        public string label { get { return m_Label; } }

        internal Wildcard(string pattern, Func<RecordingSession, string> resolver, string info = null)
        {
            m_Pattern = pattern;
            m_Label = m_Pattern;

            if (info != null)
                m_Label += " " + info;

            m_Resolver = resolver;
        }

        internal string Resolve(RecordingSession session)
        {
            return m_Resolver == null ? string.Empty : m_Resolver(session);
        }
    }

    /// <summary>
    /// Helper class for default Wildcards that you can use when constructing the output file name of a Recorder
    /// (see <see cref="RecorderSettings.OutputFile"/>).
    /// </summary>
    public static class DefaultWildcard
    {
        /// <summary>
        /// The Recorder name.
        /// </summary>
        public static readonly string Recorder = GeneratePattern("Recorder");
        /// <summary>
        /// The time the recording session started (in the 00h00m format).
        /// </summary>
        public static readonly string Time = GeneratePattern("Time");
        /// <summary>
        /// The take number (which is incremented every time a new session is started).
        /// </summary>
        public static readonly string Take = GeneratePattern("Take");
        /// <summary>
        /// The date when the recording session started (in the yyyy-MM-dd format).
        /// </summary>
        public static readonly string Date = GeneratePattern("Date");
        /// <summary>
        /// The name of the current Unity Project.
        /// </summary>
        public static readonly string Project = GeneratePattern("Project");
        /// <summary>
        /// The product name from the build settings (a combination of the Unity Project name and the output file extension).
        /// </summary>
        public static readonly string Product = GeneratePattern("Product");
        /// <summary>
        /// The name of the current Unity Scene.
        /// </summary>
        public static readonly string Scene = GeneratePattern("Scene");
        /// <summary>
        /// The output resolution in pixels.
        /// </summary>
        public static readonly string Resolution = GeneratePattern("Resolution");
        /// <summary>
        /// The current frame ID (a four-digit zero-padded number).
        /// </summary>
        public static readonly string Frame = GeneratePattern("Frame");
        /// <summary>
        /// The file extension of the output format.
        /// </summary>
        public static readonly string Extension = GeneratePattern("Extension");

        public static string GeneratePattern(string tag)
        {
            return "<" + tag + ">";
        }
    }

    [Serializable]
    public class FileNameGenerator
    {
        static string s_ProjectName;

        [SerializeField] OutputPath m_Path = new OutputPath();
        [SerializeField] string m_FileName = DefaultWildcard.Recorder;

        readonly List<Wildcard> m_Wildcards;

        internal IEnumerable<Wildcard> wildcards
        {
            get { return m_Wildcards; }
        }

        internal void FromPath(string str)
        {
            str = SanitizePath(str);

            var i = str.LastIndexOf('/');
            if (i != -1 && i < str.Length - 1)
            {
                m_FileName = str.Substring(i + 1);

                if (i == 0)
                {
                    m_Path.root = OutputPath.Root.Absolute;
                    m_Path.leaf = "/";
                }
                else
                {
                    str = str.Substring(0, i);
                    m_Path = OutputPath.FromPath(str);
                }
            }
            else
            {
                m_FileName = str;
                m_Path.root = OutputPath.Root.Absolute;
                m_Path.leaf = string.Empty;
            }
        }

        internal string ToPath()
        {
            var path = m_Path.GetFullPath();

            if (!string.IsNullOrEmpty(path))
                path += "/";

            return SanitizePath(path + SanitizeFilename(m_FileName));
        }

        /// <summary>
        /// Stores the default set of tags that make up the output file name.
        /// </summary>
        public string FileName {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        /// <summary>
        /// Indicates the root location the paths are relative to.
        /// </summary>
        public OutputPath.Root Root
        {
            get { return m_Path.root; }
            set { m_Path.root = value; }
        }

        /// <summary>
        /// Indicates the filename part of the full path (without the extension).
        /// </summary>
        public string Leaf
        {
            get { return m_Path.leaf; }
            set { m_Path.leaf = value; }
        }

        /// <summary>
        /// Use this property to ensure that the generated file is saved in the Assets folder.
        /// </summary>
        public bool ForceAssetsFolder
        {
            get { return m_Path.forceAssetsFolder; }
            set { m_Path.forceAssetsFolder = value; }
        }

        readonly RecorderSettings m_RecorderSettings;

        internal FileNameGenerator(RecorderSettings recorderSettings)
        {
            m_RecorderSettings = recorderSettings;

            m_Wildcards = new List<Wildcard>
            {
                new Wildcard(DefaultWildcard.Recorder, RecorderResolver),
                new Wildcard(DefaultWildcard.Time, TimeResolver),
                new Wildcard(DefaultWildcard.Take, TakeResolver),
                new Wildcard(DefaultWildcard.Date, DateResolver),
                new Wildcard(DefaultWildcard.Project, ProjectNameResolver),
                new Wildcard(DefaultWildcard.Product, ProductNameResolver),
                new Wildcard(DefaultWildcard.Scene, SceneResolver),
                new Wildcard(DefaultWildcard.Resolution, ResolutionResolver),
                new Wildcard(DefaultWildcard.Frame, FrameResolver),
                new Wildcard(DefaultWildcard.Extension, ExtensionResolver)
            };
        }

        /// <summary>
        /// Adds a tag and the corresponding callback to resolve it.
        /// </summary>
        /// <param name="tag">The tag string.</param>
        /// <param name="resolver">Callback invoked to replace the tag with custom content.</param>
        public void AddWildcard(string tag, Func<RecordingSession, string> resolver)
        {
            m_Wildcards.Add(new Wildcard(tag, resolver));
        }

        string RecorderResolver(RecordingSession session)
        {
            return m_RecorderSettings.name;
        }

        static string TimeResolver(RecordingSession session)
        {
            var date = session != null ? session.sessionStartTS : DateTime.Now;
            return string.Format("{0:HH}h{1:mm}m", date, date);
        }

        string TakeResolver(RecordingSession session)
        {
            return m_RecorderSettings.Take.ToString("000");
        }

        static string DateResolver(RecordingSession session)
        {
            var date = session != null ? session.sessionStartTS : DateTime.Now;
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); // ISO 8601
        }

        string ExtensionResolver(RecordingSession session)
        {
            return m_RecorderSettings.Extension;
        }

        string ResolutionResolver(RecordingSession session)
        {
            var input = m_RecorderSettings.InputsSettings.FirstOrDefault() as ImageInputSettings;
            if (input == null)
                return "NA";

            return input.OutputWidth + "x" + input.OutputHeight;
        }

        static string SceneResolver(RecordingSession session)
        {
            return SceneManager.GetActiveScene().name;
        }

        static string FrameResolver(RecordingSession session)
        {
            var i = session != null ? session.frameIndex : 0;
            return i.ToString("0000");
        }

        static string ProjectNameResolver(RecordingSession session)
        {
            if (string.IsNullOrEmpty(s_ProjectName))
            {
                var parts = Application.dataPath.Split('/');
                s_ProjectName = parts[parts.Length - 2];
            }

            return s_ProjectName;
        }

        static string ProductNameResolver(RecordingSession session)
        {
            return PlayerSettings.productName;
        }

        /// <summary>
        /// Builds an absolute path from the list of configured output file tags replaced by the RecordingSession.
        /// </summary>
        /// <param name="session">The Recorder session used to replace the tags.</param>
        /// <returns>An absolute path towards a file.</returns>
        public string BuildAbsolutePath(RecordingSession session)
        {
            var fullPath = ApplyWildcards(ToPath(), session) + "." + ExtensionResolver(session);

            string drive = null;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (fullPath.Length > 2 && char.IsLetter(fullPath[0]) && fullPath[1] == ':' && fullPath[2] == '/')
                {
                    drive = fullPath.Substring(0, 2);
                    fullPath = fullPath.Substring(3);
                }
            }

            fullPath = string.Join(Path.DirectorySeparatorChar.ToString(), fullPath.Split('/').Select(s =>
                Path.GetInvalidFileNameChars().Aggregate(s, (current, c) => current.Replace(c.ToString(), string.Empty))).ToArray());

            if (!string.IsNullOrEmpty(drive))
                fullPath = drive.ToUpper() + Path.DirectorySeparatorChar + fullPath;

            return fullPath;
        }

        /// <summary>
        /// Creates the directory structure containing the output file from the list of tags and a RecordingSession.
        /// </summary>
        /// <param name="session">The Recorder session.</param>
        public void CreateDirectory(RecordingSession session)
        {
            var path = ApplyWildcards(m_Path.GetFullPath(), session);
            if(!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        internal static string SanitizeFilename(string filename)
        {
            filename = filename.Replace("\\", "");
            filename = Regex.Replace(filename, "/", "");
            return filename;
        }
        /// <summary>
        /// Makes the output file path compliant with any OS (replacing any "\" by "/").
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>The full path with slashes "/" as file separators.</returns>
        public static string SanitizePath(string fullPath)
        {
            fullPath = fullPath.Replace("\\", "/");
            fullPath = Regex.Replace(fullPath, "/+", "/");
            return fullPath;
        }

        string ApplyWildcards(string str, RecordingSession session)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            foreach (var w in wildcards)
                str = str.Replace(w.pattern, w.Resolve(session));

            return str;
        }

    }
}
