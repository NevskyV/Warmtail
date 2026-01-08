using Data;
using Entities.Core;
using Systems.DataSystems;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class StartButton : MonoBehaviour
    {
        private UIStateSystem _uiStateSystem;
        private GlobalData _globalData;
        private SceneLoader _loader;
        private SaveSystem _saveSystem;
        
        [Inject]
        private void Construct(GlobalData globalData, SceneLoader loader,
            UIStateSystem uiStateSystem, SaveSystem saveSystem)
        {
            _uiStateSystem = uiStateSystem;
            _globalData = globalData;
            _loader = loader;
            _saveSystem = saveSystem;
        }
        public void Interact()
        {
            if (HaveSaves())
            {
                _uiStateSystem.SwitchCurrentStateAsync(UIState.Saves);
            }
            else
            {
                _globalData.LoadAutoSave();
                _loader.StartSceneProcess("Gameplay");
            }
        }
        
        private bool HaveSaves()
        {
            return _saveSystem.CheckAutoSave();
        }
    }
}