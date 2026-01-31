using System.Threading;
using DG.Tweening;
using Entities.PlayerScripts;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class FreezeVisuals : MonoBehaviour
    {
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        [SerializeField] private Material _freezeMaterial;
        [SerializeField] private float _freezeTime = 10f;
        private CancellationTokenSource _token;
        private bool _isFreezing;
        private string _lastObjId;
        [Inject] private PlayerStateController _stateController;
        [Inject] private UIStateSystem _uiStateSystem;
        
        public async void StartDrain(string objId)
        {
            if (_lastObjId == objId || _isFreezing || _uiStateSystem.CurrentState == UIState.Pause) return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _lastObjId = objId;
            
            _isFreezing = true;

            var task =  _freezeMaterial.DOFloat(0, DissolveAmount, _freezeTime).AsyncWaitForCompletion();
            await task;
            if (_freezeMaterial.GetFloat(DissolveAmount) > 0) return;
            _stateController.Die();
            _freezeMaterial.SetFloat(DissolveAmount, 1);
        }
        
        public async void StopDrain(string objId)
        {
            if(_lastObjId != objId) return;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _lastObjId = null;
            _isFreezing = false;
            _freezeMaterial.DOFloat(1, DissolveAmount, _freezeTime + 3);
        }
        
        private void OnDisable()
        {
            _freezeMaterial.SetFloat(DissolveAmount, 1);
        }
    }
}