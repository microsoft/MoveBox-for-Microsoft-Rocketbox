using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityEditor.Recorder.Input
{
    /// <summary>
    /// Use this class to record the frames rendered in the Game View window.
    /// </summary>
    [DisplayName("Game View")]
    [Serializable]
    public class GameViewInputSettings : StandardImageInputSettings
    {
        /// <summary>
        /// Use this property if you need to vertically flip the final output.
        /// </summary>
        public bool FlipFinalOutput
        {
            get { return flipFinalOutput; }
            set { flipFinalOutput = value; }
        }
        [SerializeField] private bool flipFinalOutput;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GameViewInputSettings()
        {
            outputImageHeight = ImageHeight.Window;
        }

        /// <inheritdoc/>
        protected internal override Type InputType
        {
            get { return typeof(GameViewInput); }
        }

        /// <inheritdoc/>
        public override bool SupportsTransparent
        {
            get { return false; }
        }
    }
}
