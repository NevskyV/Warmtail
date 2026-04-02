using System.Collections.Generic;
using UnityEngine;

namespace Entities.Creatures
{
    public class CreaturesSwarm : MonoBehaviour
    {
        [SerializeField] private MovableCreature[] _creatures;
        [SerializeField] private uint _count;
        [SerializeField] private Vector2 _bounds;

        [SerializeField] private Vector2 _minMaxSpeed;
        
        private List<Transform> _targetsList = new();

        private void Start()
        {
            for(int i = 0; i < _count; i++)
            {
                var creature = Instantiate(_creatures[Random.Range(0, _creatures.Length)], transform);
                creature.transform.localPosition = Vector3.zero;
                creature.SetSpeed(Random.Range(_minMaxSpeed.x, _minMaxSpeed.y));
                var newTarget = new GameObject("Target");
                newTarget.transform.parent = transform;
                UpdatePosition(newTarget.transform);
                _targetsList.Add(newTarget.transform);
                creature.UpdateTarget(newTarget.transform);
                creature.OnMoveComplete += UpdatePosition;
            }
        }

        private void UpdatePosition(Transform target)
        {
            var newPosition = new Vector2(Random.Range(transform.position.x -  _bounds.x /2, transform.position.x + _bounds.x /2),
                Random.Range(transform.position.y -  _bounds.y /2, transform.position.y + _bounds.y /2));
            target.position = newPosition;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, _bounds);
        }
#endif
    }
}