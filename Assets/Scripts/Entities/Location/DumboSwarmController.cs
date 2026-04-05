using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Interfaces;
using Systems.Swarm;
using UnityEngine;

namespace Entities.Location
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class DumboSwarmController : MonoBehaviour
    {
        [SerializeField] private SwarmController _swarmController;
        [SerializeField] private CircleCollider2D _ballCollider;
        [SerializeField] private Rigidbody2D _ballRigidbody;
        [SerializeField] private float _compressDuration = 1.5f;
        [SerializeField] private float _ballRadius = 0.8f;
        [SerializeField] private SpriteRenderer _ballSprite;

        private List<Transform> _boidTransforms = new();
        private bool _isCompressed;

        private void Awake()
        {
            if (_ballCollider == null) _ballCollider = GetComponent<CircleCollider2D>();
            SetBallActive(false);
        }

        public void CompressIntoBall()
        {
            if (_isCompressed) return;
            CompressAsync().Forget();
        }

        public void ExpandFromBall()
        {
            if (!_isCompressed) return;
            ExpandAsync().Forget();
        }

        private async UniTaskVoid CompressAsync()
        {
            _isCompressed = true;

            if (_swarmController != null)
            {
                _boidTransforms.Clear();
                var agents = _swarmController.GetNeighbors(null);
                foreach (var a in agents)
                    _boidTransforms.Add(a.transform);

                foreach (var t in _boidTransforms)
                {
                    if (t == null) continue;
                    t.DOMove(transform.position, _compressDuration).SetEase(Ease.InBack);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_compressDuration));

            if (_swarmController != null)
                _swarmController.gameObject.SetActive(false);

            SetBallActive(true);
        }

        private async UniTaskVoid ExpandAsync()
        {
            SetBallActive(false);

            if (_swarmController != null)
                _swarmController.gameObject.SetActive(true);

            foreach (var t in _boidTransforms)
            {
                if (t == null) continue;
                Vector2 scatter = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 2f;
                t.DOMove(scatter, _compressDuration * 0.7f).SetEase(Ease.OutBack);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_compressDuration * 0.7f));
            _isCompressed = false;
        }

        private void SetBallActive(bool active)
        {
            if (_ballCollider) _ballCollider.enabled = active;
            if (_ballRigidbody) _ballRigidbody.simulated = active;
            if (_ballSprite) _ballSprite.enabled = active;
        }

        public bool IsCompressed => _isCompressed;
    }
}
