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
        //[SerializeField] private int[] _loopVfxIndexes;
        [SerializeField] private AudioClip _startSfx;
        [SerializeField] private AudioClip _loopSfx;
        [SerializeField] private Vector3 _vfxOffset;
        [SerializeField] private float _normalCamZoom = 3;
        [SerializeField] private float _activeCamZoom = 4;
        
        private Player _player;
        private IAbility _ability;
        private CinemachineCamera _camera;
        private List<GameObject> _loopVfxObjs = new();
        private ParticleSystem[] _loopVfx;
        
        [Inject]
        private void Construct(Player player, PlayerConfig config, CinemachineCamera camera)
        {
            _player = player;
            if (AbilityIndex < 0 || AbilityIndex >= config.Abilities.Count)
            {
                Debug.LogError($"Invalid AbilityIndex {AbilityIndex}");
                return;
            }
            _ability = config.Abilities[AbilityIndex];
            _camera = camera;
            _ability.StartAbility += StartAbility;
            _ability.UsingAbility += UsingAbility;
            _ability.EndAbility += EndAbility;
            _loopVfx = _player.GetComponentsInChildren<ParticleSystem>();
            //UpdatePlayerStats();
        }

        public async void StartAbility()
        {
            if (!_ability.Enabled || _loopVfxObjs.Count > 0) return;
            var startObj = (await ObjectSpawnSystem.Spawn(_startVfx, _player.Rigidbody.position, _player.Rigidbody.transform)).transform;
            startObj.localRotation = Quaternion.Euler(new Vector3(0, 0, 160));
            startObj.localPosition += _vfxOffset;
            _player.ObjectSfx.PlaySfx(_startSfx);
            _player.ObjectSfx.PlayLoopSfx(_loopSfx, 500);
            
            //UpdatePlayerStats();
        }
        
        public void UsingAbility()
        {
            
            if (_ability.Enabled)
            {
                var tempZoom = _camera.Lens.OrthographicSize;
                if (!Mathf.Approximately(tempZoom, _activeCamZoom))
                {
                    DOTween.To(() => tempZoom, x =>
                    {
                        tempZoom = x;
                        _camera.Lens.OrthographicSize = tempZoom;
                    }, _activeCamZoom, 1);
                }

                if (_loopVfxObjs.Count == 0)
                {
                    foreach (var vfx in _loopVfx)
                    {
                        var main = vfx.main;
                        main.loop = true;
                        vfx.Play();
                        _loopVfxObjs.Add(vfx.gameObject);
                    }
                }
            }
        }
        
        public async void EndAbility()
        {
            //UpdatePlayerStats();
            _player.ObjectSfx.StopLoopSfx();
            var tempZoom = _camera.Lens.OrthographicSize;
            DOTween.To(() => tempZoom, x =>
            {
                tempZoom = x;
                _camera.Lens.OrthographicSize = tempZoom;
            }, _normalCamZoom,1);
            for (int i = _loopVfxObjs.Count - 1; i >= 0; i--)
            {
                var obj =  _loopVfxObjs[i];
                if (obj == null)
                {
                    _loopVfxObjs.RemoveAt(i);
                    if (_loopVfxObjs.Count == 0) break;
                    continue;
                }
                var main = obj.GetComponent<ParticleSystem>().main;
                main.loop = false;
                await UniTask.Delay(TimeSpan.FromSeconds(main.duration - obj.GetComponent<ParticleSystem>().time));
                _loopVfxObjs.Remove(obj);
            }
        }

        // private void UpdatePlayerStats()
        // {
        //     Water.SetVector(Position, 
        //         new Vector4(_player.Rigidbody.position.x, _player.Rigidbody.position.y));
        //     Water.SetFloat(Rotation, _player.Rigidbody.rotation - 90);
        // }

        public void Dispose()
        {
            _ability.StartAbility -= StartAbility;
            _ability.EndAbility -= EndAbility;
        }
    }
}
