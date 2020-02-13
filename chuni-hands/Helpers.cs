using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace chuni_hands {
    public static class Helpers {
        public static T Deserialize<T>(string path) {
            var content = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static void Serialize(object o, string path) {
            var content = JsonConvert.SerializeObject(o, Formatting.Indented);
            File.WriteAllText(path, content);
        }

        public static string GetVersion() {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }
    }
}
