using DG.Tweening;
using UnityEngine.Events;
using UnityEngine;
using Interfaces;
using Systems;

namespace Entities.Puzzle
{
    public class Gear : Warmable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _twistedAngle = 90;
        
        private int _gearId;
        private ResettableTimer _timerWarm;

        public static UnityEvent<int> OnTwisted = new();

        public void Initialize(int id)
        {
            _gearId = id;
            Reset();
            GearsPuzzle.OnReseted.AddListener(Reset);
            var color = _spriteRenderer.color;
            color.a = 0.5f;
            _spriteRenderer.color = color;
        }
        
        public override void Warm()
        {
            base.Warm();
            if (_warmthAmount > 0 && _timerWarm != null) _timerWarm.Start();
            else if(_warmthAmount > 0) _timerWarm = new ResettableTimer(_maxWarmthAmount, WarmLost);
        }

        public override void WarmComplete()
        {
            _spriteRenderer.transform.DOLocalRotate(new Vector3(0,0,_twistedAngle), 1f);
            var color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;
            OnTwisted.Invoke(_gearId);
        }

        private void WarmLost()
        {
            if (_warmthAmount > 0) Reset();
        }

        public override void Reset()
        {
            base.Reset();
            var color = _spriteRenderer.color;
            color.a = 0.5f;
            _spriteRenderer.color = color;
            _spriteRenderer.transform.DOLocalRotate(new Vector3(0,0,0), 1f);
        }
    }
}
