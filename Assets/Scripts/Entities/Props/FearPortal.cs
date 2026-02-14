using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Entities.Props
{
    public class FearPortal : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _baseObjects = new();
        [SerializeField] private List<GameObject> _portalObjects = new();
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private AudioClip _sfx;
        [SerializeField] private float _effectDuration = 1f;
        [SerializeField] private bool _reverse;
        [SerializeField] private bool _startInPortalState;
        
        private AudioSource _audioSource;
        
        public void SetReverse(bool reverse)
        {
            _reverse = reverse;
        }
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            ApplyState(_startInPortalState);
        }
        
        public async void Activate()
        {
            await PlayEffect();
            ApplyState(!_reverse);
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
            
            await UniTask.Delay((int)(_effectDuration * 1000));
        }
    }
}
