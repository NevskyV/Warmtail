using System;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using System.Collections.Generic;

namespace Entities.Creatures
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Seeker))]
    public class MovableCreature : Creature
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 360f; // градусов в секунду
        [SerializeField] private float _pathUpdateTick = 1f;
        [SerializeField] private float _nextWaypointDist = 3f;

        [SerializeField] private Transform _target;

        [Header("Tween")]
        [SerializeField] private AnimationCurve _curve;

        [Header("Rotation")]
        [SerializeField] private float _rotationOffset = 90f;

        private Seeker _seeker;
        private Rigidbody2D _rb;
        private Tween _moveTween;

        private Vector3 _lastPosition;
        
        public Action<Transform> OnMoveComplete;

        public void SetSpeed(float speed) => _speed = speed;

        private void Awake()
        {
            _seeker = GetComponent<Seeker>();
            _rb = GetComponent<Rigidbody2D>();

            InvokeRepeating(nameof(UpdatePath), _pathUpdateTick, _pathUpdateTick);
        }

        private void UpdatePath()
        {
            if (_target == null) return;
            if (Mathf.Approximately(_target.position.x, transform.position.x) && 
                Mathf.Approximately(_target.position.y, transform.position.y))
            {
                OnMoveComplete?.Invoke(_target);
                _seeker.StartPath(transform.position, _target.position, OnPathComplete);
            }
        }

        private void OnPathComplete(Path p)
        {
            if (p.error || p.vectorPath.Count < 2) return;
            StartMove(p.vectorPath);
        }

        private void StartMove(List<Vector3> points)
        {
            _moveTween?.Kill();

            float length = GetPathLength(points);
            float duration = length / _speed;

            _lastPosition = transform.position;

            _moveTween = transform
                .DOPath(points.ToArray(), duration, PathType.CatmullRom)
                .SetEase(_curve)
                .SetOptions(false)
                .OnUpdate(() =>
                {
                    _rb.MovePosition(transform.position);

                    RotateSmooth();
                });
        }

        private void RotateSmooth()
        {
            Vector2 dir = (Vector2)(transform.position - _lastPosition);

            if (dir.sqrMagnitude < 0.0001f) return;

            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + _rotationOffset;

            float newAngle = Mathf.MoveTowardsAngle(
                _rb.rotation,
                targetAngle,
                _rotationSpeed * Time.deltaTime
            );

            _rb.MoveRotation(newAngle);

            _lastPosition = transform.position;
        }

        private float GetPathLength(List<Vector3> points)
        {
            float length = 0f;

            for (int i = 1; i < points.Count; i++)
            {
                length += Vector3.Distance(points[i - 1], points[i]);
            }

            return length;
        }

        public void UpdateTarget(Transform target)
        {
            _target = target;
            _seeker.StartPath(transform.position, _target.position, OnPathComplete);
        }
    }
}