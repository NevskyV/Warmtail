using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Entities.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Entities.Props
{
    [Serializable]
    public struct GhostPoint
    {
        public Vector2 Position;
        public Vector3 Rotation;
    }

    public class GhostMemory : MonoBehaviour
    {
        [SerializeField] private Transform _ghostParent;
        [SerializeField] private RuntimeDialogueGraph _graph;
        [SerializeField] private string _prefix = "fragment_";
        [SerializeField] private bool _autoStart = true;
        [SerializeField, HideInInspector] private List<GhostPoint> _ghostPoints = new();
        [SerializeField] private List<Transform> _ghostPointTransforms = new();
        [SerializeField] private GameObject _ghostPrefab;
        [SerializeField] private GameObject _sparklePrefab;
        [SerializeField] private float _sparkleMoveDuration = 1f;
        [SerializeField] private Ease _sparkleEase = Ease.InOutQuad;
        [SerializeField] private float _sparkleCurveOffset = 1.5f;
        [SerializeField] private bool _forceGhostColliderTrigger = true;
        [SerializeField] private UnityEvent _onFinished;

        [Inject] private MonologueVisuals _monologueVisuals;
        
        private GameObject _currentGhost;
        private Collider2D _currentGhostCollider;
        private int _currentPointIndex = 0;
        private IDisposable _colliderSubscription;
        private IDisposable _collisionSubscription;
        private bool _isMoving;
        
        private void Start()
        {
            if (_autoStart)
            {
                Begin();
            }
        }

        private void OnValidate()
        {
            if (_ghostPointTransforms == null) return;
            
            if (_ghostPoints == null)
                _ghostPoints = new List<GhostPoint>(_ghostPointTransforms.Count);
            else
                _ghostPoints.Clear();

            for (int i = 0; i < _ghostPointTransforms.Count; i++)
            {
                var t = _ghostPointTransforms[i];
                if (t == null) continue;
                _ghostPoints.Add(new GhostPoint
                {
                    Position = transform.InverseTransformPoint(t.position),
                    Rotation = t.localEulerAngles
                });
            }
        }

        public void Begin()
        {
            if (_monologueVisuals == null)
            {
                _monologueVisuals = FindObjectOfType<MonologueVisuals>(true);
            }

            if (_ghostPrefab == null)
            {
                return;
            }

            if ((_ghostPoints == null || _ghostPoints.Count == 0) && _ghostPointTransforms != null && _ghostPointTransforms.Count > 0)
            {
                _ghostPoints = new List<GhostPoint>(_ghostPointTransforms.Count);
                for (int i = 0; i < _ghostPointTransforms.Count; i++)
                {
                    var t = _ghostPointTransforms[i];
                    if (t == null) continue;
                    _ghostPoints.Add(new GhostPoint { Position = transform.InverseTransformPoint(t.position), Rotation = t.localEulerAngles });
                }
            }

            if (_ghostPoints == null || _ghostPoints.Count == 0)
            {
                return;
            }

            if (_graph == null || _graph.AllNodes.Count < _ghostPoints.Count)
            {
            }

            _currentPointIndex = 0;
            SpawnGhostAtPoint(0);
            
            var node = _graph.AllNodes[_currentPointIndex];
            
        }
        
        
        private void SpawnGhostAtPoint(int index)
        {
            if (index >= _ghostPoints.Count) return;
            
            var point = _ghostPoints[index];
            var worldPos = transform.TransformPoint(new Vector3(point.Position.x, point.Position.y, 0f));
            var worldRot = transform.rotation * Quaternion.Euler(point.Rotation);
            var parent = _ghostParent != null ? _ghostParent : transform;
            _currentGhost = Instantiate(_ghostPrefab, worldPos, worldRot, parent);
            _currentGhostCollider = _currentGhost.GetComponent<Collider2D>();
            
            if (_currentGhostCollider == null)
            {
                var added = _currentGhost.AddComponent<CircleCollider2D>();
                added.isTrigger = true;
                _currentGhostCollider = added;
            }

            if (_forceGhostColliderTrigger && _currentGhostCollider is Collider2D col)
            {
                col.isTrigger = true;
            }
            
            SubscribeToCollider();
        }
        
        private void SubscribeToCollider()
        {
            if (!_currentGhostCollider) return;
            
            _colliderSubscription = _currentGhostCollider.OnTriggerEnter2DAsObservable()
                .Subscribe(OnGhostTriggered);
            
            _collisionSubscription = _currentGhostCollider.OnCollisionEnter2DAsObservable()
                .Subscribe(c =>
                {
                    if (c?.collider != null)
                    {
                        OnGhostTriggered(c.collider);
                    }
                });
        }
        
        private void OnGhostTriggered(Collider2D other)
        {
            if (_isMoving || !other || !other.CompareTag("Player")) 
            {
                return;
            }
            
            if (_monologueVisuals && _graph && _currentPointIndex < _graph.AllNodes.Count)
            {
                _monologueVisuals.RequestSingleLine(
                    _graph.AllNodes[_currentPointIndex].NodeId,
                    _prefix);
            }

            MoveToNextPoint();
        }
        
        private async UniTaskVoid MoveToNextPoint()
        {
            if (_isMoving) return;
            if (_currentPointIndex >= _ghostPoints.Count - 1) return;
            _isMoving = true;
            
            var oldGhost = _currentGhost;
            var oldPosition = oldGhost ? (Vector2)oldGhost.transform.position : _ghostPoints[_currentPointIndex].Position;
            
            _currentPointIndex++;
            var newPoint = _ghostPoints[_currentPointIndex];
            var newPosition = (Vector2)transform.TransformPoint(new Vector3(newPoint.Position.x, newPoint.Position.y, 0f));
            var newRotation = transform.rotation * Quaternion.Euler(newPoint.Rotation);
            
            if (oldGhost)
            {
                Destroy(oldGhost);
            }
            
            var parent = _ghostParent != null ? _ghostParent : transform;
            _currentGhost = Instantiate(_ghostPrefab, newPosition, newRotation, parent);
            _currentGhostCollider = _currentGhost.GetComponent<Collider2D>();
            if (_currentGhostCollider)
            {
                var added = _currentGhost.AddComponent<CircleCollider2D>();
                added.isTrigger = true;
                _currentGhostCollider = added;
            }
            if (_forceGhostColliderTrigger)
            {
                _currentGhostCollider.isTrigger = true;
            }
            
            _currentGhost.SetActive(false);
            
            if (_sparklePrefab)
            {
                var sparkle = Instantiate(_sparklePrefab, oldPosition, Quaternion.identity);
                var sparkleTransform = sparkle.transform;
                
                var start = (Vector3)oldPosition;
                var end = (Vector3)newPosition;
                var dir = (end - start);
                var dir2 = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.right;
                var perp = new Vector3(-dir2.y, dir2.x, 0f);
                var mid = (start + end) * 0.5f + perp * _sparkleCurveOffset;
                
                var tween = sparkleTransform.DOPath(new[] { start, mid, end }, _sparkleMoveDuration, PathType.CatmullRom)
                    .SetEase(_sparkleEase);
                
                await tween.AsyncWaitForCompletion();
                
                Destroy(sparkle);
            }
            
            if (_currentGhost)
            {
                _currentGhost.SetActive(true);
            }

            SubscribeToCollider();

            if (_currentPointIndex >= _ghostPoints.Count - 1)
            {
                _onFinished?.Invoke();
            }

            _isMoving = false;
        }
        
        private void OnDestroy()
        {
            _colliderSubscription?.Dispose();
            _collisionSubscription?.Dispose();
        }
    }
}
