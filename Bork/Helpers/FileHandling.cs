using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static Controls.GameDisplayObject JsonToGdo(string path)
        {
            return JsonToGdo(FileReadJson(path));
        }

        /// <summary>
        /// Expand later to use specific objects' separate creation functions
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Controls.GameDisplayObject JsonToGdo(JObject json)
        {
            var collisionType = Modules.CollisionDetection.CollisionTypes.None;
            var val = json["collisionType"];
            if (val != null)
            {
                switch (val.ToString().ToLower())
                {
                    case "none":
                        break;
                    case "projectile":
                        collisionType = Modules.CollisionDetection.CollisionTypes.Projectile;
                        break;
                    case "ship":
                        collisionType = Modules.CollisionDetection.CollisionTypes.Ship;
                        break;
                    default:
                        throw new NotImplementedException("non existent collision type");
                }
                    
            }
            
            var animated = false;
            val = json["animated"];
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

            int from = 0;
            val = json["from"];
            if (val != null)
                from = (int)val;

            var output = new Controls.GameDisplayObject(json["path"].ToString(), collisionType, animated, frameCount, duration, from);
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
