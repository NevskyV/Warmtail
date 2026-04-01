
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Props;
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
            var obj = Instantiate(new GameObject("sfx")).AddComponent<AudioSource>();
            obj.PlayOneShot(_clips[sfxName]);
            obj.gameObject.AddComponent<AutoDestroy>().Destroy(5f);
        }
        
        public void PlaySfx(AudioClip sfx)
        {
            if (_awaitable != null)
            {
                DOTween.Kill(_awaitable);
                _awaitable = null;
            }
            var obj = Instantiate(new GameObject("sfx")).AddComponent<AudioSource>();
            obj.PlayOneShot(sfx);
            obj.gameObject.AddComponent<AutoDestroy>().Destroy(5f);
        }
        
        public async void PlayLoopSfx(AudioClip sfx, int delay = 0)
        {
            await UniTask.Delay(delay);
            if (_source == null) return;
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