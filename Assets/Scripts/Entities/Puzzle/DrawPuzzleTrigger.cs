using Systems.Abilities;
using Systems.Abilities.Concrete;
using UnityEngine;
using Interfaces;
using Zenject;

namespace Entities.Puzzle
{
    public class DrawPuzzleTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _body;
        private int _triggerId;
        private bool _isActive = true;
        private DrawPuzzle _drawPuzzle;
        [Inject] [SerializeField] private AbilitiesManager _abilitiesManager;
        [SerializeField] private IAbilityExtended i1;
        [SerializeField] private IAbilityExtended i2;
        
        public void Initialize(int id, DrawPuzzle puzzle)
        {
            _triggerId = id;
            _drawPuzzle = puzzle;
            _drawPuzzle.OnReseted.AddListener(Reset);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            i1 = _abilitiesManager.ActiveAbility;
            i2 = _abilitiesManager.ComboAbility;
            if (other.CompareTag("Player") && _abilitiesManager.ActiveAbility is WarmingAbility or DashAbility && 
                _abilitiesManager.ComboAbility is WarmingAbility or DashAbility)
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
