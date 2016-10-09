using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    public class AnimationProfile
    {
        public AnimationProfile()
        {
            IsAnimated = false;

            //TODO: figure out how to fit this in to the image store structure
            // problem: loading a bunch of images safely and painlessly
        }

        public bool IsAnimated { get; set; }
        public int Framerate { get; set; }
    }
}
