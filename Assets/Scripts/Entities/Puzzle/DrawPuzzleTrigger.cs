using Systems;
using Systems.Abilities;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle
{
    public class DrawPuzzleTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _body;
        private int _triggerId;
        private bool _isActive = true;
        private DrawPuzzle _drawPuzzle;
        [Inject] private ComboSystem _comboSystem;
        
        public void Initialize(int id, DrawPuzzle puzzle)
        {
            _triggerId = id;
            _drawPuzzle = puzzle;
            _drawPuzzle.OnReseted.AddListener(Reset);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _comboSystem.AbilityType1 is WarmingAbility or DashAbility && 
                _comboSystem.AbilityType1 is WarmingAbility or DashAbility)
            {
                if (_drawPuzzle.TriggerConform(_triggerId, _isActive))
                {
                    Disable();
                }
            }
        }

        private void Reset()
        {
            _isActive = true;
            _body.SetActive(true);
        }

        private void Disable()
        {
            _isActive = false;
            _body.SetActive(false);
        }
    }
}
