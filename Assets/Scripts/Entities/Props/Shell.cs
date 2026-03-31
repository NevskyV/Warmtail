using System;
using Data;
using Data.Player;
using DG.Tweening;
using Entities.Sound;
using Interfaces;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class Shell : Warmable
    {
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        public static Action OnShellsChanged = delegate {};
        [SerializeField] private int _shellsAmount;
        [SerializeField] private string _sfx;
        [SerializeField] private ParticleSystem _vfx;
        private GlobalData _globalData;
        private MaterialPropertyBlock _propertyBlock;
        private SpriteRenderer _renderer;
        private Tween _tween;
 
        [Inject]
        public void Construct(GlobalData globalData)
        {
            _globalData = globalData;
            DailySystem.OnLoadedResources += LoadShell;
            DailySystem.OnDiscardedResources += DiscardShell;
        }

        private void Start()
        {
            _propertyBlock = new();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnDestroy()
        {
            DailySystem.OnLoadedResources -= LoadShell;
            DailySystem.OnDiscardedResources -= DiscardShell;
        }
        
        public override void Warm()
        {
            UpdateRenderer((_maxWarmthAmount - _warmthAmount) * 1.0f / _maxWarmthAmount,
                (_maxWarmthAmount - _warmthAmount + _warmFactor) * 1.0f / _maxWarmthAmount);
            base.Warm();
        }

        public override void WarmComplete()
        {
            _globalData.Edit<SavablePlayerData>((playerData) =>
            {
                playerData.Shells += _shellsAmount;
            });
            _globalData.Edit<ShellsData>(data => {
                data.ShellsActive[ConvertFloatsToString (transform.position.x, transform.position.y)] = false;
            });
            OnShellsChanged?.Invoke();
            _timer.Stop();
            GetComponent<ObjectSfx>().PlaySfx(_sfx);
            ObjectSpawnSystem.Spawn(_vfx, transform.position);
            Destroy(gameObject);
        }
        
        public override void Reset()
        {
            UpdateRenderer((_maxWarmthAmount - _warmthAmount) * 1.0f / _maxWarmthAmount, 0);
            base.Reset();
        }

        private void LoadShell()
        {
            float x = transform.position.x;
            float y = transform.position.y;
            CheckShellData(x, y);
            if (!_globalData.Get<ShellsData>().ShellsActive[ConvertFloatsToString(x, y)])
                Destroy(gameObject);
        }

        private void DiscardShell()
        {
            float x = transform.position.x;
            float y = transform.position.y;
            CheckShellData(x, y);
            _globalData.Edit<ShellsData>(data => data.ShellsActive[ConvertFloatsToString(x, y)] = true);
        }

        private void CheckShellData(float x, float y)
        {
            if (!_globalData.Get<ShellsData>().ShellsActive.ContainsKey(ConvertFloatsToString(x, y)))
                _globalData.Edit<ShellsData>(data => data.ShellsActive[ConvertFloatsToString(x, y)] = true);
        }

        private string ConvertFloatsToString(float x, float y)
        {
            return (x + " : " + y);
        }
        
        private async void UpdateRenderer(float lastAmount, float newAmount)
        {
            if (!_renderer) return;
            _tween?.Pause();
            _tween = DOTween.To(() => lastAmount, x =>{
                if (!_renderer)
                {
                    _tween?.Pause();
                    return;
                }

                lastAmount = x;
                _propertyBlock.SetFloat(DissolveAmount, x);
                _renderer.SetPropertyBlock(_propertyBlock);
            }, newAmount, 0.5f);
            await _tween.AsyncWaitForCompletion();
        }
    }
}
