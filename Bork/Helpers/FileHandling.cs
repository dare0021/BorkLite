using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Bork.Controls;

namespace Bork.Helpers
{
    static class FileHandling
    {

        public static List<string> FileReadAllLines(string path)
        {
            var output = new List<string>();
            using (StreamReader f = new StreamReader(path))
            {
                while (!f.EndOfStream)
                    output.Add(f.ReadLine());
            }
            return output;
        }

        public static void FileWriteAllLines(string path, params string[] data)
        {
            using (StreamWriter f = new StreamWriter(path))
            {
                foreach (var s in data)
                    f.WriteLine(s);
            }
        }

        public static JObject ParseJson(string json)
        {
            return JObject.Parse(json);
        }

        public static JObject FileReadJson(string path)
        {
            string jsonString = "";
            foreach (var s in FileReadAllLines(path))
            {
                jsonString += s + "\n";
            }
            return JObject.Parse(jsonString);
        }

        public static void FileWriteJson(string path, JObject json, bool compact = false)
        {
            var format = Formatting.Indented;
            if (compact)
                format = Formatting.None;
            var jsonString = json.ToString(format);
            FileWriteAllLines(jsonString);
        }

        public static void FileWriteJson(string path, Object input, bool compact = false)
        {
            var format = Formatting.Indented;
            if (compact)
                format = Formatting.None;
            var jsonString = JsonConvert.SerializeObject(input, format);
            FileWriteAllLines(path, jsonString);
        }

        public static List<RichImage> LoadLevelFromJson(string path)
        {
            return LoadLevelFromJson(FileReadJson(path));
        }

        /// <summary>
        /// If necessary expand to process arguments for RichImageProfile functions
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<RichImage> LoadLevelFromJson(JObject json)
        {
            var output = new List<RichImage>();
            var methods = typeof(RichImageProfiles).GetMethods();
            foreach (var key in json.Properties())
            {
                RichImage ri = null;
                var keyString = key.Name;
                var type = key.Value["type"];
                if (type == null)
                {
                    foreach (var method in methods)
                    {
                        if (keyString.Equals(method.Name))
                        {
                            ri = (GameDisplayObject)method.Invoke(null, null);
                        }
                    }
                }
                else
                {
                    var typeString = ((string)type).ToLower();
                    switch (typeString)
                    {
                        case "ri":
                            ri = LoadRiFromJson(JObject.Parse(key.Value.ToString()));
                            break;
                        case "gdo":
                            throw new ArgumentException("GDO should utilize RichImageProfiles.cs");
                        default:
                            throw new NotImplementedException("Unknown object type during JSON load");
                    }
                }

                if (key.Value["position"] != null)
                {
                    var posX = (double)key.Value["position"][0];
                    var posY = (double)key.Value["position"][1];
                    ri.setPosition(posX, posY);
                }

                if (key.Value["scale"] != null)
                {
                    var sX = (double)key.Value["scale"][0];
                    var sY = (double)key.Value["scale"][1];
                    ri.setScale(sX, sY);
                }

                var rot = key.Value["rotation"];
                if (rot != null)
                {
                    ri.setRotation(new Degree((double)rot));
                }

                output.Add(ri);
            }
            return output;
        }

        /// <summary>
        /// Avoid if at all possible
        /// Use RichImageProfiles.cs instead
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static RichImage LoadRiFromJson(JObject json)
        {
            var animated = false;
            var val = json["animated"];
            if (val != null)
                animated = (bool)val;

            int frameCount = 0;
            val = json["frameCount"];
            if (val != null)
                frameCount = (int)val;

            double duration = 0;
            val = json["duration"];
            if (val != null)
                duration = (double)val;
            else
            {
                val = json["frameRate"];
                if (val != null)
                    duration = 1.0 / (double)val;
            }

            int from = 0;
            val = json["from"];
            if (val != null)
                from = (int)val;

            var output = new RichImage(json["path"].ToString(), animated, frameCount, duration, from);
            return output;
        }

/*        public static string GdoToJson(Controls.GameDisplayObject gdo)
        {
            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();

            writer.WriteEndObject();
            return sw.ToString();
        }*/

        private static void addKVP(JsonTextWriter writer, string key, Object value)
        {
            writer.WritePropertyName(key);
            writer.WriteValue(value);
        }

        private static void addArray<T>(JsonTextWriter writer, ICollection<T> collection)
        {
            writer.WriteStartArray();
            foreach (var item in collection)
                writer.WriteValue(item);
            writer.WriteEndArray();
        }
    }
}
