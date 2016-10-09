using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bork.Helpers
{
    class ResourceMap
    {
        private Dictionary<string, BitmapImage> resources;

        /// <summary>
        /// Replaces existing entry if item with the name already exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public BitmapImage LoadResource(string name, string path)
        {
            if (path[0] == '/')
            {
                path = path.Substring(1);
            }
            var bmp = new BitmapImage(new Uri(@"pack://application:,,,/Bork;component/" + path, UriKind.Absolute));
            resources[name] = bmp;
            return bmp;
        }

        /// <summary>
        /// Returns null if no such entry is found
        /// </summary>
        public BitmapImage GetResource(string name)
        {
            if (!resources.ContainsKey(name))
                return null;
            return resources[name];
        }

        /// <summary>
        /// Returns false if no such entry was found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveResource(string name)
        {
            return resources.Remove(name);
        }

        public bool Contains(string name)
        {
            return resources.ContainsKey(name);
        }
    }
}
