using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bork.Helpers
{
    class SoundSystem : MediaElement
    {
        static UIElementCollection addTo = null;

        private bool looping;
        private bool autocull = true;
        private Common.AudioState state = Common.AudioState.Stop;

        /// <summary>
        /// Run before using the class
        /// </summary>
        static public void init(UIElementCollection parent)
        {
            addTo = parent;
        }

        static public UIElementCollection getAddTo()
        {
            return addTo;
        }

        public SoundSystem(string path, bool autoplay = true, bool loop = false, bool autoCull = true)
        {
            LoadedBehavior = MediaState.Manual;
            // audio and video files cannot be embedded in the program as per MS spec
            // can only be reference as separate files
            Source = new Uri(path, UriKind.Relative);
            addTo.Add(this);

            looping = loop;
            this.autocull = autoCull;
            MediaEnded += OnMediaEnded;

            if (autoplay)
                Play();
        }

        public new void Play()
        {
            if (state == Common.AudioState.Done)
                Stop();
            base.Play();
            state = Common.AudioState.Play;
        }

        public new void Pause()
        {
            base.Pause();
            state = Common.AudioState.Pause;
        }

        public new void Stop()
        {
            base.Stop();
            state = Common.AudioState.Stop;
        }

        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            state = Common.AudioState.Done;
            if (looping)
                Play();
        }

        public bool isLooping()
        {
            return looping;
        }

        public Common.AudioState getState()
        {
            return state;
        }

        /// <summary>
        /// Remove when done with playback
        /// No guarantee this will happen immediately or at all 
        /// (if the object remains close to the screen)
        /// </summary>
        public bool AutoCull
        {
            get
            {
                return autocull;
            }
            set
            {
                autocull = value;
            }
        }
    }
}
