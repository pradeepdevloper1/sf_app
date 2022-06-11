using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace XF.APP
{
    public class AppSettingsManager
    {
        private static AppSettingsManager _instance;
        private readonly JObject _secrets;

        private const string Namespace = "XF.APP";
        private const string FileName = "appsettings.json";

        private AppSettingsManager()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppSettingsManager)).Assembly;
            var stream = assembly.GetManifestResourceStream($"{Namespace}.{FileName}");
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                _secrets = JObject.Parse(json);
            }
        }

        public static AppSettingsManager Settings
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettingsManager();
                }

                return _instance;
            }
        }

        public string this[string name]
        {
            get
            {
                var path = name.Split(':');

                JToken node = _secrets[path[0]];
                for (int index = 1; index < path.Length; index++)
                {
                    node = node[path[index]];
                }

                return node.ToString();
            }
        }
    }
}
