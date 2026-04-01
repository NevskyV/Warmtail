using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace Entities.Props
{
    public class FearPortal : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _baseObjects = new();
        [SerializeField] private List<GameObject> _portalObjects = new();
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private AudioClip _sfx;
        [SerializeField] private Volume _volume;
        [SerializeField] private float _effectDuration = 1f;
        [SerializeField] private bool _reverse;
        [SerializeField] private bool _startInPortalState;
        [SerializeField] private bool _destroyAfterActivate;
        [SerializeField] private List<GameObject> _disableAfterActivate = new();
        
        private AudioSource _audioSource;
        private bool _isActivating;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            ApplyState(_startInPortalState);
        }
        
        public async UniTaskVoid Activate()
        {
            await ActivateAsync();
        }

        public async UniTask ActivateAsync()
        {
            if (_isActivating) return;
            _isActivating = true;

            await PlayEffect();
            ApplyState(!_reverse);

            for (int i = 0; i < _disableAfterActivate.Count; i++)
            {
                var obj = _disableAfterActivate[i];
                if (obj != null) obj.SetActive(false);
            }

            if (_destroyAfterActivate)
            {
                Destroy(gameObject);
            }

            _isActivating = false;
        }

        private void ApplyState(bool portalActive)
        {
            foreach (var obj in _baseObjects)
            {
                if (obj != null) obj.SetActive(!portalActive);
            }
            foreach (var obj in _portalObjects)
            {
                if (obj != null) obj.SetActive(portalActive);
            }
        }
        
        private async UniTask PlayEffect()
        {
            if (_vfx != null)
            {
                _vfx.Play();
            }
            
            if (_sfx != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_sfx);
            }
            DOTween.To(()=>_volume.weight, x => _volume.weight = x, _reverse? 0:1, _effectDuration);
            await UniTask.Delay((int)(_effectDuration * 1000));
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            Activate();
        }
    }
    
    
}
