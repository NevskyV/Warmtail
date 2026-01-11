using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Player;
using DG.Tweening;
using Entities.PlayerScripts;
using Interfaces;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Systems.AbilitiesVisual
{
    [Serializable]
    public class MovementVisuals : IAbilityVisual, IDisposable
    {
        private static readonly int Rotation = Shader.PropertyToID("_Rotation");
        private static readonly int Position = Shader.PropertyToID("_Position");
        
        [field: SerializeReference] public int AbilityIndex {get; set;}
        [field: SerializeReference] public Material Water {get; set;}
        [SerializeField] private ParticleSystem _startVfx;
        [SerializeField] private ParticleSystem _loopVfx;
        [SerializeField] private AudioClip _startSfx;
        [SerializeField] private AudioClip _loopSfx;
        [SerializeField] private Vector3 _vfxOffset;
        
        private Player _player;
        private IAbility _ability;
        private CinemachineCamera _camera;
        private LensSettings _lastSettings;
        private List<GameObject> _loopVfxObjs = new();
        
        [Inject]
        private void Construct(Player player, PlayerConfig config, CinemachineCamera camera)
        {
            _player = player;
            _ability = config.Abilities[AbilityIndex];
            _camera = camera;
            _ability.StartAbility += StartAbility;
            _ability.UsingAbility += UsingAbility;
            _ability.EndAbility += EndAbility;
            
            _lastSettings = _camera.Lens;
            UpdatePlayerStats();
        }

        public async void StartAbility()
        {
            if (!_ability.Enabled) return;
            var startObj = (await ObjectSpawnSystem.Spawn(_startVfx, _player.Rigidbody.position, _player.Rigidbody.transform)).transform;
            startObj.localRotation = Quaternion.Euler(new Vector3(0, 0, 160));
            startObj.localPosition += _vfxOffset;
            _player.ObjectSfx.PlaySfx(_startSfx);
            _player.ObjectSfx.PlayLoopSfx(_loopSfx, 500);
            
            UpdatePlayerStats();

            DOTween.To(() => _camera.Lens.OrthographicSize, x => _camera.Lens.OrthographicSize = x, 11,1);
            var loopVfxObj = (await ObjectSpawnSystem.Spawn(_loopVfx, _player.Rigidbody.position, _player.Rigidbody.transform,200)).gameObject;
            loopVfxObj.transform.localRotation = Quaternion.Euler(new Vector3(0,0,160));
            loopVfxObj.transform.localPosition += _vfxOffset;
            _loopVfxObjs.Add(loopVfxObj);
        }
        
        public void UsingAbility()
        {
            UpdatePlayerStats();
        }
        
        public async void EndAbility()
        {
            UpdatePlayerStats();
            _player.ObjectSfx.StopLoopSfx();
            DOTween.To(() => _camera.Lens.OrthographicSize, x =>
            {
                if(_ability.Enabled)_camera.Lens.OrthographicSize = x;
            }, 9,1);
            await UniTask.Delay(200);
            foreach (var obj in _loopVfxObjs)
            {
                Object.Destroy(obj);
            }
        }

        private void UpdatePlayerStats()
        {
            Water.SetVector(Position, 
                new Vector4(_player.Rigidbody.position.x, _player.Rigidbody.position.y));
            Water.SetFloat(Rotation, _player.Rigidbody.rotation - 90);
        }

        public void Dispose()
        {
            _ability.StartAbility -= StartAbility;
            _ability.EndAbility -= EndAbility;
        }
    }
}