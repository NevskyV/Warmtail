using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Entities.UI
{
    public class WiggleRotator: MonoBehaviour
    {
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private float _speed;
        private Vector3 _startRotation;

        private void Start()
        {
            _startRotation = transform.localEulerAngles;
            Tick();
        }

        private async void Tick()
        {
            transform.DOLocalRotate(_startRotation + _rotation, _speed / 2);
            await UniTask.Delay(TimeSpan.FromSeconds(_speed / 2));
            while (true)
            {
                transform.DOLocalRotate(_startRotation - _rotation, _speed);
                await UniTask.Delay(TimeSpan.FromSeconds(_speed));
                transform.DOLocalRotate(_startRotation + _rotation, _speed);
                await UniTask.Delay(TimeSpan.FromSeconds(_speed));
            }
        }
    }
}