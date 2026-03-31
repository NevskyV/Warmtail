using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace Entities.Creatures
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Seeker))]
    public class MovableCreature : Creature
    {
        [SerializeField] private float _speed = 100;
        [SerializeField] private float _rotationSpeed = 100;
        [SerializeField] private float _nextWaypointDist = 3;
        [SerializeField] private float _pathUpdateTick = 1;
        [SerializeField] private Vector3 _rotationOffset;
        
        [SerializeField] private Transform _target;
        private Path _path;
        private int _currentWaypoint;
        private bool _pathCompleted;
        
        private Seeker _seeker;
        private Rigidbody2D _rb;
        private Vector2 _lastPos;

        private void Awake()
        {
            _seeker = GetComponent<Seeker>();
            _rb = GetComponent<Rigidbody2D>();

            InvokeRepeating(nameof(UpdatePath), 0, _pathUpdateTick);
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                _path = p;
                _currentWaypoint = 0;
            }
        }
        
        private void UpdatePath()
        {
            if(_seeker.IsDone() && Vector3.Distance(_rb.position, _target.position) > _nextWaypointDist 
                                && Vector3.Distance(_rb.position, _lastPos) > _nextWaypointDist)
                _seeker.StartPath(_rb.position, _target.position, OnPathComplete);
        }

        public void UpdateTarget(Transform target)
        {
            _target = target;
        }

        private void FixedUpdate()
        {
            _lastPos =  _rb.position;
            if (_path == null) return;

            if (_currentWaypoint >= _path.vectorPath.Count)
            {
                _pathCompleted = true;
                return;
            }

            _pathCompleted = false;

            var dir = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(_target.position - transform.position, _rotationOffset);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
            _rb.MovePosition(_rb.position + dir * (_speed * Time.deltaTime));
            
            float distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
            if (distance < _nextWaypointDist)
                _currentWaypoint++;
        }
    }
}