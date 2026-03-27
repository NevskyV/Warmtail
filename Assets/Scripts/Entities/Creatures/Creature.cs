using System;
using Cysharp.Threading.Tasks;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Creatures
{
    public enum CreatureTemperature { Warm, Neutral, Cold }

    public class Creature : MonoBehaviour
    {
        [SerializeField] private CreatureTemperature _temperature = CreatureTemperature.Neutral;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _interactionCooldown = 1f;

        private static readonly int InteractHash = Animator.StringToHash("Interact");

        private WarmthSystem _warmthSystem;
        private bool _onCooldown;

        [Inject]
        private void Construct(WarmthSystem warmthSystem)
        {
            _warmthSystem = warmthSystem;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_onCooldown || !other.CompareTag("Player")) return;
            Interact().Forget();
        }

        private async UniTaskVoid Interact()
        {
            _onCooldown = true;

            if (_animator != null)
                _animator.SetTrigger(InteractHash);

            switch (_temperature)
            {
                case CreatureTemperature.Warm:
                    _warmthSystem.AddCell();
                    break;
                case CreatureTemperature.Cold:
                    break;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_interactionCooldown));
            _onCooldown = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var col = GetComponent<Collider2D>();
            if (col == null) return;
            Gizmos.color = _temperature switch
            {
                CreatureTemperature.Warm => new Color(1f, 0.4f, 0f, 0.35f),
                CreatureTemperature.Cold => new Color(0.3f, 0.7f, 1f, 0.35f),
                _ => new Color(0.8f, 0.8f, 0.8f, 0.25f)
            };
            Gizmos.DrawSphere(transform.position, col.bounds.extents.magnitude);
        }
#endif
    }
}
