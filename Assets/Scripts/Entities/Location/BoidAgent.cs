
using Interfaces;
using UnityEngine;

namespace Systems.Swarm
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BoidAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _maxSpeed = 6f;
        [SerializeField] private float _steerForce = 5f;

        private SwarmController _controller;

        public void Initialize(SwarmController controller)
        {
            _controller = controller;
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_controller == null) return;

            var neighbors = _controller.GetNeighbors(this);

            Vector2 cohesion = Vector2.zero;
            Vector2 alignment = Vector2.zero;
            Vector2 separation = Vector2.zero;
            int count = 0;

            foreach (var boid in neighbors)
            {
                if (boid == this) continue;
                float dist = Vector2.Distance(transform.position, boid.transform.position);

                if (dist < _controller.NeighborRadius)
                {
                    cohesion += (Vector2)boid.transform.position;
                    alignment += boid._rb.linearVelocity;
                    separation += ((Vector2)transform.position - (Vector2)boid.transform.position) / Mathf.Max(dist * dist, 0.0001f);
                    count++;
                }
            }

            Vector2 acceleration = Vector2.zero;
            if (count > 0)
            {
                if (!_controller.IsControlled)
                {
                    cohesion = ((cohesion / count) - (Vector2)transform.position).normalized;
                    acceleration += cohesion * _controller.CohesionWeight;
                }

                alignment = (alignment / count).normalized;
                separation = separation.normalized;
                
                float controlFactor = _controller.IsControlled ? 0.3f : 1f;
                acceleration += alignment * _controller.AlignmentWeight * controlFactor;
                acceleration += separation * _controller.SeparationWeight * controlFactor;
            }

            Vector2 controllerPos = (Vector2)_controller.transform.position;
            Vector2 targetDir = (controllerPos - (Vector2)transform.position);

            if (targetDir.sqrMagnitude > 0.0001f)
            {
                float targetFactor = _controller.IsControlled ? 0.3f : 1f;

                // Лёгкое усиление эффекта центра (1.1 = +10%)
                float centerBias = 2f;

                acceleration += targetDir.normalized * _controller.TargetWeight * targetFactor * centerBias;
            }

            
            _rb.linearVelocity += acceleration * Time.fixedDeltaTime * _steerForce;
            _rb.linearVelocity = Vector2.ClampMagnitude(_rb.linearVelocity, _maxSpeed);
            
            Vector2 desiredDir = _rb.linearVelocity;
            if (desiredDir.sqrMagnitude > 0.0001f)
                transform.up = desiredDir.normalized;
        }
        
        public void InteractWithPhysicsObjects()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.attachedRigidbody != null && !hit.CompareTag("Player"))
                {
                    hit.attachedRigidbody.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
                }
            }
        }

        public void WarmObjects()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Warmable>(out var warmable))
                {
                    warmable.Warm();
                }
            }
        }

        public void DestroyObstacles()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDestroyable>(out var destroyable))
                {
                    destroyable.DestroyObject();
                }
            }
        }
    }
}
