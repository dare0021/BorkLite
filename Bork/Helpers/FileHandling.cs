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

        public static JObject GdoToJson(Controls.GameDisplayObject gdo)
        {
            string output = "";
            return output;
        }
    }
}
