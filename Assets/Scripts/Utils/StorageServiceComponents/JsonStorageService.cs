using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Services.Storage
{
    public class JsonStorageService : IStorageService
    {
        public void Save(string key, object data, Action<bool> callback = null)
        {
            string path = BuildPath(key);
            string file = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (var fileStream = new StreamWriter(path))
            {
                fileStream.Write(file);
            }

            callback?.Invoke(true);
        }

        public void Load<T>(string key, Action<T> callback)
        {
            string path = BuildPath(key);


            using (var fileStream = new StreamReader(path))
            {
                string file = fileStream.ReadToEnd();
                T data = JsonConvert.DeserializeObject<T>(file);
                callback?.Invoke(data);
            }
        }

        private string BuildPath(string key)
        {
            key += ".json";
            return Path.Combine(Application.persistentDataPath, key);
        }
    }
}
