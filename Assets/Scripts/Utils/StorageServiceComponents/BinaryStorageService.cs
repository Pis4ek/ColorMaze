using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Services.Storage
{
    public class BinaryStorageService : IStorageService
    {
        private BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public void Save(string key, object data, Action<bool> callback = null)
        {
            string path = BuildPath(key);

            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                _binaryFormatter.Serialize(fileStream, data);
                callback?.Invoke(true);
            }
        }

        public void Load<T>(string key, Action<T> callback)
        {
            string path = BuildPath(key);

            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                T data = (T)_binaryFormatter.Deserialize(fileStream);
                callback?.Invoke(data);
            }
        }

        private string BuildPath(string key)
        {
            key += ".save";
            return Path.Combine(Application.persistentDataPath, key);
        }
    }
}

