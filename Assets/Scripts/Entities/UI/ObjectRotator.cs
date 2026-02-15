
using UnityEngine;

namespace Entities.UI
{
    [ExecuteAlways]
    public class ObjectRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _speed;
        private void FixedUpdate()
        {
            transform.Rotate(_speed);
        }
    }
}