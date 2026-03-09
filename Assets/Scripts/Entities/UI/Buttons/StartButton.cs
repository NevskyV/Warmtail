using Data;
using Entities.Core;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class StartButton : MonoBehaviour
    {
        private GlobalData _globalData;
        private SceneLoader _loader;
        
        [Inject]
        private void Construct(GlobalData globalData, SceneLoader loader)
        {
            _globalData = globalData;
            _loader = loader;
        }
        public void Interact()
        {
            _globalData.LoadAutoSave();
            _loader.StartSceneProcess("Gameplay");
        }
    }
}