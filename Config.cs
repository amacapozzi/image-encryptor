using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageEncryptor
{
    internal class Config
    {
        public static string ConfigPath = "data.json";

        public class ConfigData
        {
            public string name { get; set; }
            public string encryptedImage { get; set; }
            public string date { get; set; }

            public ConfigData(string name, string encryptedImage, string date)
            {
                this.name = name;
                this.encryptedImage = encryptedImage;
                this.date = date;
            }
        }

        public static void SaveEncryptedImage(ConfigData newData)
        {
            List<ConfigData> configDataList = new List<ConfigData>();

            if (File.Exists(ConfigPath))
            {
                string existingJson = File.ReadAllText(ConfigPath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    configDataList = JsonConvert.DeserializeObject<List<ConfigData>>(existingJson);
                }
            }

            configDataList.Add(newData);

            var updatedJsonData = JsonConvert.SerializeObject(configDataList, Formatting.Indented);
            File.WriteAllText(ConfigPath, updatedJsonData);
        }

        public static ConfigData FindDataByName(string imageName)
        {
            if (File.Exists(ConfigPath))
            {
                var deserializedData = JsonConvert.DeserializeObject<ConfigData[]>(File.ReadAllText(ConfigPath));

                return deserializedData.FirstOrDefault(d => d.name == imageName);
            }

            return null;
        }
    }
}