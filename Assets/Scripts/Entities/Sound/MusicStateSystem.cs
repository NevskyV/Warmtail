using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Entities.Core;
using Systems.Effects;
using TriInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Entities.Sound
{
    public class MusicStateSystem : MonoBehaviour
    {
        [SerializeReference, Range(0.5f, 8f)] private float _crossFadeTime;
        [SerializeField] private AudioSource _source1;
        [SerializeField] private AudioSource _source2;

        [SerializeField, ShowInInspector, SerializedDictionary("Music State", "Audio Clips")]
        private SerializedDictionary<MusicState, List<AudioClip>> _clips = new();

        [Inject] private SceneLoader _sceneLoader;
        private MusicState _currentState;
        private CancellationTokenSource _tokenSource;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
             var clip = _clips[_currentState][Random.Range(0, _clips[_currentState].Count)];
             
             _source1.clip = clip;
             _source1.Play();
             _tokenSource =  new CancellationTokenSource();
             LoopNext(_source1).Forget();
             _sceneLoader.SceneStartLoading += () => ChangeMusicStateAsync(MusicState.None);
             _sceneLoader.SceneLoaded += id =>
             {
                 print(id);
                 switch (id)
                 {
                     case "Gameplay":
                         ChangeMusicStateAsync(MusicState.Bottom);
                         break;
                     case "Home":
                         ChangeMusicStateAsync(MusicState.Home);
                         break;
                     case "Start":
                         ChangeMusicStateAsync(MusicState.Menu);
                         break;
                 }
             };
        }
        
        public void ChangeMusicState(int state) => ChangeMusicStateAsync((MusicState)state);
        
        public async void ChangeMusicStateAsync(MusicState state)
        {
            _tokenSource?.Cancel();
            _tokenSource =  new CancellationTokenSource();
            var sourceFromCross = _source1.clip ? _source1 : _source2;
            var sourceToCross = _source1.clip ? _source2 : _source1;
            try
            {
                var rand = Random.Range(0, _clips[state].Count);
                
                if (sourceFromCross.clip == _clips[state][rand] && _clips[state].Count > 1)
                {
                    if (rand == 0) rand += 1;
                    else rand -= 1; 
                }
                var clip = _clips[state][rand];
                _currentState = state;
            
                sourceToCross.clip = clip;
                sourceToCross.volume = 0;
                sourceToCross.Play();
                await foreach (var (a, b) in CrossfadeEffect.CrossfadeTwins(_crossFadeTime))
                {
                    sourceFromCross.volume = a;
                    sourceToCross.volume = b;
                }
                sourceFromCross.Stop();
                sourceFromCross.clip = null;
                LoopNext(sourceToCross).Forget();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private async UniTaskVoid LoopNext(AudioSource source)
        {
            if (!source.clip) return;
            var time = source.clip.length - _crossFadeTime;
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: _tokenSource.Token);
            ChangeMusicStateAsync(_currentState);
        }
        
    }

    [Serializable]
    public enum MusicState
    {
        Bottom, Middle, High, Menu, Home, End, None
    }
}