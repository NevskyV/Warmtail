using System.Collections.Generic;
using UnityEngine;

namespace Systems.Swarm
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SwarmController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("Swarm Settings")]
        [SerializeField] private BoidAgent _boidPrefab;
        [SerializeField] private int _count = 15;
        [SerializeField] private float _spawnRadius = 2f;

        [Header("Boid Rules")]
        public float CohesionWeight = 1f;
        public float AlignmentWeight = 1f;
        public float SeparationWeight = 1.5f;
        public float TargetWeight = 2f;
        public float NeighborRadius = 3f;

        private readonly List<BoidAgent> _agents = new();
        private bool _isControlled = false;
        private Vector2 _controlInput = Vector2.zero;
        private float _baseSpeed;
        private float _speedMultiplier = 1f;

        public bool IsControlled => _isControlled;

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            _baseSpeed = _speed;
            Initialize();
        }

        public void Initialize()
        {
            if (_agents.Count > 0) return;

            for (int i = 0; i < _count; i++)
            {
                Vector2 pos = (Vector2)transform.position + Random.insideUnitCircle * _spawnRadius;
                var boid = Instantiate(_boidPrefab, pos, Quaternion.identity, transform);
                boid.Initialize(this);
                _agents.Add(boid);
            }
        }
        
        public void SetControlled(bool controlled)
        {
            _isControlled = controlled;
            if (!controlled)
                _controlInput = Vector2.zero;
        }
        
        public void SetControlInput(Vector2 input)
        {
            _controlInput = input;
        }

        private void FixedUpdate()
        {
            if (!_isControlled) return;

            if (_controlInput.sqrMagnitude > 0.01f)
            {
                var targetVelocity = _controlInput.normalized * (_speed * _speedMultiplier);
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 5f);

                float angle = Mathf.Atan2(_controlInput.y, _controlInput.x) * Mathf.Rad2Deg;
                _rb.rotation = Mathf.LerpAngle(_rb.rotation, angle, Time.fixedDeltaTime * _rotationSpeed);
            }
            else
            {
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * 2f);
            }
        }
        
        public List<BoidAgent> GetNeighbors(BoidAgent agent)
        {
            return _agents;
        }
        
        public void SetSpeedMultiplier(float multiplier)
        {
            _speedMultiplier = multiplier;
        }
    }
}
