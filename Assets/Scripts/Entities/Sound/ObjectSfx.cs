
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Entities.Sound
{
    public class ObjectSfx : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private SerializedDictionary<string, AudioClip> _clips = new();
        private Task _awaitable;
        public void PlaySfx(string sfxName)
        {
            if (_awaitable != null)
            {
                _awaitable.Dispose();
                _awaitable = null;
            }
            _source.PlayOneShot(_clips[sfxName]);
        }
        
        public void PlaySfx(AudioClip sfx)
        {
            if (_awaitable != null)
            {
                DOTween.Kill(_awaitable);
                _awaitable = null;
            }
            _source.PlayOneShot(sfx);
        }
        
        public async void PlayLoopSfx(AudioClip sfx, int delay = 0)
        {
            await UniTask.Delay(delay);
            if (_awaitable != null)
            {
                DOTween.Kill(_awaitable);
                _awaitable = null;
            }
            _source.clip = sfx;
            _source.Play();
        }
        
        public async void StopLoopSfx()
        {
            _awaitable = _source.DOFade(0f, 0.2f).AsyncWaitForCompletion();
            await _awaitable;
            _source.Stop();
            _source.volume = 1;
            _source.clip = null;
        }
    }
}