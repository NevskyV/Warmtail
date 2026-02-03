using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Systems.DataSystems
{
    [Serializable]
    public class SaveContainer
    {
        public Dictionary<string, object> Blocks = new Dictionary<string, object>();
    }

    public class SaveSystem
    {
        private readonly string _autoFileName = "auto_save.json";
        private readonly string _settingsFileName = "settings.json";
        private SaveContainer _autoContainer;
        private SaveContainer _settingsContainer;

        public SaveContainer SettingsContainer => _settingsContainer;
        public SaveContainer AutoContainer => _autoContainer;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        private string AutoFilePath => Path.Combine(Application.persistentDataPath, _autoFileName);
        public string SettingsFilePath => Path.Combine(Application.persistentDataPath, _settingsFileName);
        
        public void Load(ref List<ISavableData> dataList)
        {
            if (dataList == null) return;

            var autoContainer = LoadContainerFromDisk(AutoFilePath);
            var fileExists = autoContainer is { Blocks: { Count: > 0 } };

            if (!fileExists)
            {
                _autoContainer = new SaveContainer();
                foreach (var d in dataList)
                {
                    if (d == null) continue;
                    var key = d.GetType().Name;
                    _autoContainer.Blocks[key] = d;
                }
                WriteContainerToDisk(_autoContainer, AutoFilePath);
            }
            else
            {
                _autoContainer = autoContainer;
            }

            for (int i = 0; i < dataList.Count; i++)
            {
                var proto = dataList[i];
                if (proto == null) continue;
                var key = proto.GetType().Name;
                if (_autoContainer.Blocks.TryGetValue(key, out var stored))
                {
                    var intermediateJson = JsonConvert.SerializeObject(stored, _settings);
                    var loaded = (ISavableData)JsonConvert.DeserializeObject(intermediateJson, proto.GetType(), _settings);
                    if (loaded != null) dataList[i] = loaded;
                }
                else
                {
                    _autoContainer.Blocks[key] = proto;
                }
            }
        }

        public bool CheckAutoSave()
        {
            var autoContainer = LoadContainerFromDisk(AutoFilePath);
            return autoContainer is { Blocks: { Count: > 0 } };
        }

        public void UpdateData(ISavableData data, SaveContainer container)
        {
            if (data == null) return;
            if (container == null) container = new SaveContainer();
            var key = data.GetType().Name;
            container.Blocks[key] = data;
        }

        public void SaveAllToDisk()
        {
            if (_autoContainer == null || _settingsContainer == null) return;
            WriteContainerToDisk(_autoContainer, AutoFilePath);
            WriteContainerToDisk(_settingsContainer, SettingsFilePath);
        }
        
        public T Load<T>(T data) where T : ISavableData 
        {
            var container = LoadContainerFromDisk(SettingsFilePath);
            var fileExists = container is { Blocks: { Count: > 0 } };
            var key = typeof(T).Name;
            if (!fileExists)
            {
                _settingsContainer = new SaveContainer();
                _settingsContainer.Blocks[key] = data;
                WriteContainerToDisk(_settingsContainer, SettingsFilePath);
            }
            else
            {
                _settingsContainer = container;
            }
            if (!_settingsContainer.Blocks.TryGetValue(key, out var stored)) return data;
            var intermediateJson = JsonConvert.SerializeObject(stored, _settings);
            var loaded = (T)JsonConvert.DeserializeObject(intermediateJson, typeof(T), _settings);
            _settingsContainer.Blocks[key] = loaded;
            return loaded ?? data;
        }

        private SaveContainer LoadContainerFromDisk(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return null;
                var txt = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(txt)) return null;
                var container = JsonConvert.DeserializeObject<SaveContainer>(txt, _settings);
                return container ?? null;
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadContainerFromDisk error: {e.Message}");
                return null;
            }
        }

        private void WriteContainerToDisk(SaveContainer container, string filePath)
        {
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonConvert.SerializeObject(container, _settings);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"WriteContainerToDisk error: {e.Message}");
            }
        }
    }
}
