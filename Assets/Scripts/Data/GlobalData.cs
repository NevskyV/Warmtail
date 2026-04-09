using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Systems.DataSystems;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace Data
{
    public delegate void DataEventFunc();
    
    public class GlobalData : MonoBehaviour
    {
        [Title("Data")]
        [SerializeReference] private List<ISavableData> _savableData = new();
        [SerializeReference] private List<IRuntimeData> _runtimeData = new();
        [SerializeReference] private SettingsData _settingsData;
        [SerializeField] private InputActionAsset _actionAsset;
        private readonly Dictionary<IData, List<DataEventFunc>> _subs = new();
        public List<ISavableData> SavableData => _savableData;
        
        private SaveSystem _saveSystem;

        [Button("Delete Save Data"), GUIColor("red")]
        public void DeleteSaveData()
        {
            try
            {
                var filePath = Path.Combine(Application.persistentDataPath, "auto_save.json");
                var settingsfilePath = Path.Combine(Application.persistentDataPath, "settings.json");
                var manualSavesPath = Path.Combine(Application.persistentDataPath, "manual_saves");
                if (File.Exists(filePath)) File.Delete(filePath);
                if (File.Exists(settingsfilePath)) File.Delete(settingsfilePath);
                if(Directory.Exists(manualSavesPath)) Directory.Delete(manualSavesPath, true);
                Debug.Log("Data deleted!");
                _savableData = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteSaves error: {e.Message}");
            }
        }
        
        [Inject]
        private void Construct(SaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
            _settingsData = _saveSystem.Load(_settingsData);
            
            var allDataList = _runtimeData.Concat(new List<IData>{_settingsData}).ToList();
            foreach (var data in allDataList)
            {
                _subs.Add(data, new List<DataEventFunc>());
            }
            
            _actionAsset["R"].performed += _ =>
            {
                if (_actionAsset["Control"].IsPressed())
                    DeleteSaveData();
            };
            
#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name != "Start")
            {
                LoadAutoSave();
            } 
#endif
        }
        
        public void SubscribeTo<T>(DataEventFunc selector) where T : class, IData
        {
            IData key = null;
            
            if (typeof(ISavableData).IsAssignableFrom(typeof(T)))
            {
                key = _savableData.Find(x => x.GetType() == typeof(T));
            }
            
            else if (typeof(IRuntimeData).IsAssignableFrom(typeof(T)))
            {
                key = _runtimeData.Find(x => x.GetType() == typeof(T));
            }
            
            if (key != null)
            {
                _subs[key].Add(selector);
                Debug.Log($"Added {typeof(T).Name}");
            }
            else Debug.Log($"No instance of type {typeof(T).Name} found in _savableData or _runtimeData.");
        }

        public void NotifySubscribers<T>() where T : class, IData
        {
            var foundKey = _subs.Keys.First(data => typeof(T) == data.GetType());

            foreach (var sub in _subs[foundKey])
            {
                sub.Invoke();
            }
            if (typeof(ISavableData).IsAssignableFrom(typeof(T)))
                _saveSystem.UpdateData((ISavableData)foundKey, foundKey is not SettingsData? _saveSystem.AutoContainer : _saveSystem.SettingsContainer);
        }
        
        public void Edit<T>(Action<T> mutator) where T : class, IData {
            var foundKey = _subs.Keys.First(data => typeof(T) == data.GetType());
            mutator((T)foundKey);
            NotifySubscribers<T>(); 
        }

        public void LoadAutoSave()
        {
            _saveSystem.Load(ref _savableData);
            UpdateAllData(_savableData);
        }
        
        public void UpdateAllData(List<ISavableData> newList)
        {
            if (newList == null) return;
            foreach (var data in newList)
            {
                List<DataEventFunc> subs = new();
                if (_subs.Keys.FirstOrDefault(x => x.GetType() == data.GetType()) != null)
                {
                    var key = _subs.Keys.First(x => x.GetType() == data.GetType());
                    subs = _subs[key];
                    _subs.Remove(key);
                }
                print(data.GetType().FullName + " " + subs.Count);
                _subs.TryAdd(data, subs);
                foreach (var sub in _subs[data])
                {
                    sub.Invoke();
                }
            }
        }
        public T Get<T>() where T : class, IData
        {
            return (T)_subs.Keys.First(data => typeof(T) == data.GetType());
        }

        public void OnDisable()
        {
            _subs.Clear();
            _saveSystem.SaveAllToDisk();
        }
    }
}
