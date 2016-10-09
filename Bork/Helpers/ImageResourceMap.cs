using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bork.Helpers
{
    public class ImageResourceMap
    {
        private Dictionary<string, string> resources = new Dictionary<string, string>();

        /// <summary>
        /// Does not load the file again if already present in ImageResourceMaster
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public BitmapImage LoadResource(string name, string path)
        {
            if (!ImageResourceMaster.Contains(path))
                ImageResourceMaster.LoadResource(path);
            resources[name] = path;
            return GetResource(name);
        }

        /// <summary>
        /// Returns null if no such entry is found
        /// </summary>
        public BitmapImage GetResource(string name)
        {
            if (!resources.ContainsKey(name))
                return null;
            return ImageResourceMaster.GetResource(resources[name]);
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
