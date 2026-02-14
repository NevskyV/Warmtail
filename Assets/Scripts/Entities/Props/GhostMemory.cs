using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Entities.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class GhostMemory : MonoBehaviour
    {
        [SerializeField] private RuntimeDialogueGraph _graph;
        [SerializeField] private string _prefix;
        [SerializeField] private List<Transform> _ghostPoints = new();
        [SerializeField] private GameObject _ghostPrefab;
        [SerializeField] private GameObject _sparklePrefab;
        [SerializeField] private float _sparkleMoveDuration = 1f;
        [SerializeField] private Ease _sparkleEase = Ease.InOutQuad;

        [Inject] private MonologueVisuals _monologueVisuals;
        
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
            
            if (_graph == null || _graph.AllNodes.Count < _ghostPoints.Count)
            {
                Debug.LogWarning("GhostMemory: Dialogue graph has fewer nodes than ghost points.");
            }

            SpawnGhostAtPoint(0);
        }
        
        private void SpawnGhostAtPoint(int index)
        {
            if (index >= _ghostPoints.Count) return;
            
            var point = _ghostPoints[index];
            if (point == null)
            {
                Debug.LogWarning("GhostMemory: Ghost point transform is missing.");
                return;
            }

            _currentGhost = Instantiate(_ghostPrefab, point.position, point.rotation);
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
                if (_graph != null && _currentPointIndex < _graph.AllNodes.Count)
                {
                    _monologueVisuals.RequestSingleLine(_graph.AllNodes[_currentPointIndex].NodeId, _prefix);
                }
                MoveToNextPoint();
            }
        }
        
        private async void MoveToNextPoint()
        {
            if (_currentPointIndex >= _ghostPoints.Count - 1) return;
            
            var oldGhost = _currentGhost;
            var oldPoint = _ghostPoints[_currentPointIndex];
            var oldPosition = oldPoint != null ? (Vector2)oldPoint.position : (Vector2)transform.position;
            
            _currentPointIndex++;
            var newPoint = _ghostPoints[_currentPointIndex];
            if (newPoint == null)
            {
                Debug.LogWarning("GhostMemory: Ghost point transform is missing.");
                return;
            }
            
            if (oldGhost != null)
            {
                Destroy(oldGhost);
            }
            
            if (_sparklePrefab != null)
            {
                var sparkle = Instantiate(_sparklePrefab, oldPosition, Quaternion.identity);
                var sparkleTransform = sparkle.transform;
                
                sparkleTransform.position = oldPosition;
                var tween = sparkleTransform.DOMove(newPoint.position, _sparkleMoveDuration)
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
