using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot
{
    static class SettingsAccessor
    {
        /// <summary>
        /// Fetches the setting that is specified by the Key.
        /// First tries to get the value from an environment variable.
        /// If it doesn't exist, tries to get the value from the "settings.txt" file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSetting<T>(string key)
        {
            var envValue = Environment.GetEnvironmentVariable(key);

            if (!envValue.IsNullOrWhiteSpace())
            {
                return envValue.Parse<T>();
            }

            var settingsFilePath = "settings.json";

            if (File.Exists(settingsFilePath))
            {
                var settings = JObject.Parse(File.ReadAllText(settingsFilePath));

                var requestedSettingNode = settings[key];

                if (requestedSettingNode != null)
                {
                    return requestedSettingNode.Value<T>();
                }
            }

            throw new Exception("Unable to locate setting.");
        }
    }
}
