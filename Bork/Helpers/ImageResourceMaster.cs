using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bork.Helpers
{
    static class ImageResourceMaster
    {
        static private Dictionary<string, BitmapImage> resources = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// Updates existing entry if item at the path was already loaded
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        static public BitmapImage LoadResource(string path)
        {
            if (path[0] == '/')
            {
                path = path.Substring(1);
            }
            var bmp = new BitmapImage(new Uri(@"pack://application:,,,/Bork;component/" + path, UriKind.Absolute));
            resources[path] = bmp;
            return bmp;
        }

        /// <summary>
        /// Returns null if no such entry is found
        /// </summary>
        static public BitmapImage GetResource(string path)
        {
            if (!resources.ContainsKey(path))
                return null;
            return resources[path];
        }

        /// <summary>
        /// Returns false if no such entry was found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public bool UnloadResource(string path)
        {
            return resources.Remove(path);
        }

        static public bool Contains(string path)
        {
            return resources.ContainsKey(path);
        }
    }
}
