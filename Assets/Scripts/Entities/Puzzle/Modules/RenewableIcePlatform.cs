using Interfaces;
using Systems;
using UnityEngine;

namespace Entities.Puzzle.Modules
{
    public class RenewableIcePlatform : MonoBehaviour, IDestroyable
    {
        [SerializeField] private float _respawnDelay = 4f;
        [SerializeField] private GameObject _visual;
        [SerializeField] private Collider2D _collider;

        private ResettableTimer _respawnTimer;

        public void Initialize()
        {
            _respawnTimer = new ResettableTimer(_respawnDelay, Respawn);
            SetActive(true);
        }

        public void DestroyObject()
        {
            SetActive(false);
            _respawnTimer?.Start();
        }

        private void Respawn()
        {
            SetActive(true);
        }

        private void SetActive(bool active)
        {
            if (_visual) _visual.SetActive(active);
            if (_collider) _collider.enabled = active;
        }
    }
}
