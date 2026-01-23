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
        }
        
        public async void Activate()
        {
            if (_reverse)
            {
                await PlayEffect();
                foreach (var obj in _portalObjects)
                {
                    if (obj != null) obj.SetActive(false);
                }
                foreach (var obj in _baseObjects)
                {
                    if (obj != null) obj.SetActive(true);
                }
            }
            else
            {
                await PlayEffect();
                foreach (var obj in _baseObjects)
                {
                    if (obj != null) obj.SetActive(false);
                }
                foreach (var obj in _portalObjects)
                {
                    if (obj != null) obj.SetActive(true);
                }
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
