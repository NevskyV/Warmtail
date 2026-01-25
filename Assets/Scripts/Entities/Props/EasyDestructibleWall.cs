using Interfaces;
using Systems;
using UnityEngine;

namespace Entities.Props
{
    public class EasyDestructibleWall : MonoBehaviour, IDestroyable
    {
        [SerializeField] private ParticleSystem _particlePrefab;
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            DestroyObject();
        }

        public void DestroyObject()
        {
            if(_particlePrefab) ObjectSpawnSystem.Spawn(_particlePrefab, transform.position);
            Destroy(gameObject);
        }
    }
}