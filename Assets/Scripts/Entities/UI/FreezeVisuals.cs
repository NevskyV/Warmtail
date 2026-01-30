using System.Collections;
using System.Threading;
using System.Threading.Tasks;
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
        [SerializeField] private float _freezeRate = 0.2f;
        private float _currentFreeze;
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

            var task =  _freezeMaterial.DOFloat(0, DissolveAmount, 10).AsyncWaitForCompletion();
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
            _freezeMaterial.DOFloat(1, DissolveAmount, 13);
        }
        
        private void OnDisable()
        {
            _currentFreeze = 0;
            _freezeMaterial.SetFloat(DissolveAmount, 1);
        }
    }
}