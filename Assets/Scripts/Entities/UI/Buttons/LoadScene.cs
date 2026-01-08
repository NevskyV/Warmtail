using Data;
using Data.Player;
using Entities.Core;
using Systems.DataSystems;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class LoadScene : MonoBehaviour
    {
        private SceneLoader _sceneLoader;
        private SaveSystem _saveSystem;
        private GlobalData _globalData;
        
        [Inject]
        private void Construct(SceneLoader sceneLoader, SaveSystem saveSystem, GlobalData globalData)
        {
            _sceneLoader = sceneLoader;
            _saveSystem = saveSystem;
            _globalData = globalData;
        }

        public void Load(string index)
        {
            _saveSystem.SaveAllToDisk();
            _sceneLoader.StartSceneProcess(index);
        }
        
        public void LoadLastScene()
        {
            _sceneLoader.StartSceneProcess(_globalData.Get<SavablePlayerData>().LastScene);
        }
    }
}