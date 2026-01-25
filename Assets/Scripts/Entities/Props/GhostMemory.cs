using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Entities.Props
{
    [Serializable]
    public class GhostPoint
    {
        public Vector2 Position;
        public Vector3 Rotation;
    }

    public class GhostMemory : MonoBehaviour
    {
        [SerializeField] private List<GhostPoint> _ghostPoints = new();
        [SerializeField] private GameObject _ghostPrefab;
        [SerializeField] private GameObject _sparklePrefab;
        [SerializeField] private float _sparkleMoveDuration = 1f;
        [SerializeField] private Ease _sparkleEase = Ease.InOutQuad;
        
        private GameObject _currentGhost;
        private Collider2D _currentGhostCollider;
        private int _currentPointIndex = 0;
        private IDisposable _colliderSubscription;
        
        private void Start()
        {
            if (_ghostPoints.Count == 0 || _ghostPrefab == null)
            {
                Debug.LogError("GhostMemory: Ghost points or prefab not set!");
                return;
            }
            
            SpawnGhostAtPoint(0);
        }
        
        private void SpawnGhostAtPoint(int index)
        {
            if (index >= _ghostPoints.Count) return;
            
            var point = _ghostPoints[index];
            _currentGhost = Instantiate(_ghostPrefab, point.Position, Quaternion.Euler(point.Rotation));
            _currentGhostCollider = _currentGhost.GetComponent<Collider2D>();
            
            if (_currentGhostCollider == null)
            {
                Debug.LogWarning("GhostMemory: Ghost prefab doesn't have a Collider2D!");
                return;
            }
            
            SubscribeToCollider();
        }
        
        private void SubscribeToCollider()
        {
            if (_colliderSubscription != null)
            {
                _colliderSubscription.Dispose();
            }
            
            if (_currentGhostCollider == null) return;
            
            _colliderSubscription = _currentGhostCollider.OnTriggerEnter2DAsObservable()
                .Subscribe(OnGhostTriggered);
        }
        
        private void OnGhostTriggered(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                MoveToNextPoint();
            }
        }
        
        private async void MoveToNextPoint()
        {
            if (_currentPointIndex >= _ghostPoints.Count - 1) return;
            
            var oldGhost = _currentGhost;
            var oldPosition = _ghostPoints[_currentPointIndex].Position;
            
            _currentPointIndex++;
            var newPoint = _ghostPoints[_currentPointIndex];
            
            if (oldGhost != null)
            {
                Destroy(oldGhost);
            }
            
            if (_sparklePrefab != null)
            {
                var sparkle = Instantiate(_sparklePrefab, oldPosition, Quaternion.identity);
                var sparkleTransform = sparkle.transform;
                
                sparkleTransform.position = oldPosition;
                var tween = sparkleTransform.DOMove(newPoint.Position, _sparkleMoveDuration)
                    .SetEase(_sparkleEase);
                
                SpawnGhostAtPoint(_currentPointIndex);
                
                await tween.AsyncWaitForCompletion();
                
                Destroy(sparkle);
            }
            else
            {
                SpawnGhostAtPoint(_currentPointIndex);
            }
        }
        
        private void OnDestroy()
        {
            _colliderSubscription?.Dispose();
        }
    }
}
