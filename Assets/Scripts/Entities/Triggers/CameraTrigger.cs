using Data;
using DG.Tweening;
using Entities.PlayerScripts;
using Entities.Props;
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
        private CinemachinePositionComposer _positionComposer;
        private GlobalData _data;
        private float _lastZoom;
        private Vector2 _targetPos;
        
        [Inject]
        private void Construct(Player player, PlayerAbilityController abilityController, CinemachineCamera cam, GlobalData data)
        {
            _camera = cam;
            _player = player;
            _abilityController = abilityController;
            _data = data;
            _targetPos = _target.position;
            _positionComposer = _camera.GetComponent<CinemachinePositionComposer>();
        }

        private async void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _abilityController.DisableAllAbilities();
                
                _target.position = _player.Rigidbody.transform.position;
                _camera.Target.TrackingTarget = _target;
                _lastZoom = 3.5f;//_camera.Lens.OrthographicSize;
                _positionComposer.Lookahead.Enabled = false;
                _positionComposer.Composition.DeadZone.Enabled = false;
                
                DOTween.To(() => _camera.Lens.OrthographicSize, value => _camera.Lens.OrthographicSize = value, _zoom,
                    _stunTime * 0.5f);

                await _target.DOMove(_targetPos, _stunTime * 0.5f).AsyncWaitForCompletion();
                Disable();
            }
        }

        private async void Disable()
        {
            _positionComposer.Lookahead.Enabled = true;
            _positionComposer.Composition.DeadZone.Enabled = true;
            
            DOTween.To(() => _camera.Lens.OrthographicSize, value => _camera.Lens.OrthographicSize = value, _lastZoom,
                _stunTime * 0.5f);
            await _target.DOMove(_player.Rigidbody.transform.position, _stunTime * 0.5f).AsyncWaitForCompletion();
            _camera.Target.TrackingTarget = _player.Rigidbody.transform;
            _abilityController.EnableLastAbilities();
            if (_destroyAfter)
            {
                ChangeState(false);
            }
        }
    }
}