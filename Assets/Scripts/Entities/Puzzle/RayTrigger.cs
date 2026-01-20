using UnityEngine;
using Interfaces;
using Systems;

namespace Entities.Puzzle
{
    public class RayTrigger : Warmable
    {
        [SerializeField] private RayCollector _parentRayCollector;
        [SerializeField] private bool _isActiveInitially;
        [SerializeField] private GameObject _rayImage;
        
        private bool _isActive;
        private ResettableTimer _timerWarm;

        private void Start()
        {
            if (!_isActiveInitially) _rayImage.SetActive(false);

            _isActive = _isActiveInitially;
            _parentRayCollector.AddIncoming();
        }

        public override void Warm()
        {
            base.Warm();
            if(_warmthAmount > 0)
            {
                _timerWarm ??= new ResettableTimer(3, Reset);
                _timerWarm.Start();
            }
        }

        public override void WarmComplete()
        {
            _isActive = !_isActive;
            _rayImage.SetActive(_isActive);
            if (_isActive != _isActiveInitially) _parentRayCollector.AddCorrectSignal();
            else _parentRayCollector.DicreaseCorrectSignal();
        }
    }
}
