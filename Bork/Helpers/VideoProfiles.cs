using Bork.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bork.Helpers
{
    static class VideoProfiles
    {
        static public GameDisplayObject explosion()
        {
            return new GameDisplayObject("videos/explosion", Modules.CollisionDetection.CollisionTypes.None, true, 16, 1.0 / 60);
        }

        internal static GameDisplayObject redLaser()
        {
            return new GameDisplayObject("videos/beam", Modules.CollisionDetection.CollisionTypes.None, true, 7, 1.0 / 60);
        }
    }
}
