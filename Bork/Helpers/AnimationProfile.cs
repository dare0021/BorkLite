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
        public AnimationProfile(string profileName, string imageName)
        {
            name = profileName;
            IsAnimated = false;
            currentItem = imageName;
            currentOffset = 0;
            animationData.Add(new Pair<string, double>(imageName, double.MaxValue));
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


        private string name, currentItem;
        private double currentOffset;
        private List<Pair<string, double>> animationData = new List<Pair<string, double>>();

        public bool IsAnimated { get; set; }
        public int Framerate { get; set; }

        public string getName()
        {
            return name;
        }

        public string getCurrentItem()
        {
            return currentItem;
        }
    }
}
