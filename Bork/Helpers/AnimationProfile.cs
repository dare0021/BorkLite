using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    /// <summary>
    /// Stores a list of image names and their duration
    /// The name should be used by other classes as keys for their ImageResourceMap instances
    /// </summary>
    public class AnimationProfile
    {
        public AnimationProfile(string imageName)
        {
            IsAnimated = false;
            currentItem = imageName;
            currentOffset = 0;
            animationData.Add(new Pair<string, double>(imageName, double.MaxValue));
        }

        /// <summary>
        /// Loads a video with the given frame rate using images of name namePrefix#, where # is ints from from to from + frameCount
        /// e.g. namePrefix1, namePrefix2, ... if from is 1
        /// </summary>
        /// <param name="namePrefix"></param>
        /// <param name="duration"></param>
        /// <param name="frameCount"></param>
        /// <param name="from">defaults to 0</param>
        public AnimationProfile(string namePrefix, double duration, int frameCount, int from = 0)
        {
            IsAnimated = true;
            currentItem = namePrefix + from;
            currentOffset = 0;

            for (int i = from; i < frameCount + from; i++)
            {
                animationData.Add(new Pair<string, double>(namePrefix + from, duration));
            }
        }

        public void Update(double dt)
        {
            var dtNew = dt + currentOffset;
            var i = getNodeLocationByName(currentItem);
            while (dtNew >= animationData[i].Y)
            {
                dtNew -= animationData[i].Y;
                i = (i + 1) % animationData.Count;
            }
            currentItem = animationData[i].X;
            currentOffset = dtNew;
        }

        public void setUniformDuration(double duration)
        {
            for (int i=0; i<animationData.Count; i++)
            {
                animationData[0].Y = duration;
            }
        }

        private Pair<string, double> getNodeByName(string name)
        {
            for (int i=0; i<animationData.Count; i++)
            {
                if (animationData[i].X == name)
                    return animationData[i];
            }
            return null;
        }

        private int getNodeLocationByName(string name)
        {
            for (int i = 0; i < animationData.Count; i++)
            {
                if (animationData[i].X == name)
                    return i;
            }
            return -1;
        }


        private string currentItem;
        private double currentOffset;
        private List<Pair<string, double>> animationData = new List<Pair<string, double>>();

        public bool IsAnimated { get; set; }      

        public string getCurrentItem()
        {
            return currentItem;
        }
    }
}
