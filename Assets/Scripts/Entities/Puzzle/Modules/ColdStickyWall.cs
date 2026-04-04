using Data;
using Data.Player;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Modules
{
    public class ColdStickyWall : MonoBehaviour
    {
        [SerializeField] private float _coldThreshold = 35f;

        private ColdWallStickModule _module;
        private Rigidbody2D _playerRb;
        private bool _sticking;

        [Inject] private GlobalData _globalData;

        public void Initialize(ColdWallStickModule module)
        {
            _module = module;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (_globalData == null)
            {
                _globalData = FindObjectOfType<GlobalData>();
                if (_globalData == null) return;
            }

            var temp = _globalData.Get<RuntimePlayerData>().Temperature;
            if (temp < _coldThreshold)
            {
                if (_playerRb == null)
                    other.TryGetComponent<Rigidbody2D>(out _playerRb);

                _sticking = true;
                if (_playerRb != null)
                {
                    _playerRb.linearVelocity = Vector2.zero;
                    _playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }
            else
            {
                Unstick();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                Unstick();
        }

        private void Unstick()
        {
            if (!_sticking) return;
            _sticking = false;
            if (_playerRb != null)
                _playerRb.constraints = RigidbodyConstraints2D.None;
            _playerRb = null;
        }
    }
}
