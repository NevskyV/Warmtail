using System;
using Cysharp.Threading.Tasks;
using Data;
using Entities.PlayerScripts;
using Entities.Probs;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Entities.Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class CameraTrigger : SavableStateObject
    {
        [SerializeField] private bool _destroyAfter;
        [SerializeField] private float _stunTime;
        [SerializeField] private Transform _target;
        [SerializeField] private float _zoom;
        private Player _player;
        private PlayerAbilityController _abilityController;
        private CinemachineCamera _camera;
        private GlobalData _data;
        private float _lastZoom;
        
        [Inject]
        private void Construct(Player player, PlayerAbilityController abilityController, CinemachineCamera cam, GlobalData data)
        {
            _camera = cam;
            _player = player;
            _abilityController = abilityController;
            _data = data;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _abilityController.DisableAllAbilities();
                _camera.Target.TrackingTarget = _target;
                _lastZoom = _camera.Lens.OrthographicSize;
                _camera.Lens.Lerp(new LensSettings
                {
                    OrthographicSize = _zoom,
                    FarClipPlane = _camera.Lens.FarClipPlane,
                    NearClipPlane = _camera.Lens.NearClipPlane,
                },500);
                Disable();
            }
        }

        private async void Disable()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_stunTime));
            
            _abilityController.EnableLastAbilities();
            _camera.Target.TrackingTarget = _player.Rigidbody.transform;
            _camera.Lens.Lerp(new LensSettings
            {
                OrthographicSize = _lastZoom,
                FarClipPlane = _camera.Lens.FarClipPlane,
                NearClipPlane = _camera.Lens.NearClipPlane,
            }, 500);
            if (_destroyAfter)
            {
                ChangeState(false);
            }
        }
    }
}