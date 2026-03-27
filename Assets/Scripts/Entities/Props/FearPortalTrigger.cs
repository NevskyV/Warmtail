using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Entities.Props
{
    [RequireComponent(typeof(Collider2D))]
    public class FearPortalTrigger : MonoBehaviour
    {
        [SerializeField] private FearPortal _portal;
        [SerializeField] private GhostMemory _ghostMemoryToBegin;
        [SerializeField] private bool _disableThisColliderOnEnter = true;
        [SerializeField] private GameObject[] _disableOnEnter;

        private bool _triggered;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
            if (_portal == null) _portal = GetComponent<FearPortal>();
            if (_ghostMemoryToBegin == null) _ghostMemoryToBegin = GetComponent<GhostMemory>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered) return;
            if (other == null || !other.CompareTag("Player")) return;
            _triggered = true;

            if (_disableThisColliderOnEnter)
            {
                var col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }

            if (_disableOnEnter != null)
            {
                for (int i = 0; i < _disableOnEnter.Length; i++)
                {
                    var go = _disableOnEnter[i];
                    if (go != null) go.SetActive(false);
                }
            }

            ActivateFlow().Forget();
        }

        private async UniTaskVoid ActivateFlow()
        {
            if (_portal != null)
            {
                await _portal.ActivateAsync();
            }

            if (_ghostMemoryToBegin != null)
            {
                _ghostMemoryToBegin.Begin();
            }
        }
    }
}

