using Entities.Core;
using Interfaces;
using UnityEngine;
using Data;
using UnityEngine.Splines;
using System.Threading.Tasks;
using System;
using Cysharp.Threading.Tasks;

namespace Systems.SequenceActions
{
    public class SplineAnimateAction: ISequenceAction
    {
        [SerializeField] private string _characterId;
        [SerializeField] private string _splineId;
        [SerializeField] private bool _move = true;
        [SerializeField] private bool _loop = false;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }


        public async void Invoke()
        {
            Debug.Log ("ira spline " + _splineId);

            var character = _eventsData.SceneObjects[_characterId].GetComponent<SplineAnimate>();
            character.Container = _eventsData.SceneObjects[_splineId].GetComponent<SplineContainer>();
            if (_loop) character.Loop = SplineAnimate.LoopMode.Loop;
            else character.Loop = SplineAnimate.LoopMode.Once;

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            character.Restart(false);
            if (_move) {
                Debug.Log ("ira spline move " + _splineId);
                character.Play();
            }
        }
    }
}