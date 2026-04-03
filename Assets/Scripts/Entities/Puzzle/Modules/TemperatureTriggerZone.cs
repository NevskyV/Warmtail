using Data;
using Data.Player;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Puzzle.Modules
{
    public class TemperatureTriggerZone : MonoBehaviour
    {
        public enum TriggerTemperature { Hot, Cold }

        [SerializeField] private TriggerTemperature _requiredTemperature = TriggerTemperature.Cold;
        [SerializeField] private float _activationTime = 5f;

        private TemperatureTriggerModule _module;
        private bool _playerInside;
        private float _timeAccumulator;
        private bool _solved;

        [Inject] private GlobalData _globalData;

        public void Initialize(TemperatureTriggerModule module)
        {
            _module = module;
        }

        private void Update()
        {
            if (_solved || !_playerInside || _module == null) return;

            var temp = _globalData.Get<RuntimePlayerData>().Temperature;
            bool tempMatch = _requiredTemperature == TriggerTemperature.Hot
                ? temp > TemperatureSystem.Neutral
                : temp < TemperatureSystem.Neutral;

            if (tempMatch)
            {
                _timeAccumulator += Time.deltaTime;
                if (_timeAccumulator >= _activationTime)
                    Complete();
            }
            else
            {
                _timeAccumulator = 0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _playerInside = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
                _timeAccumulator = 0f;
            }
        }

        public void ActivateByTail()
        {
            if (!_solved) Complete();
        }

        private void Complete()
        {
            _solved = true;
            _module?.Solve();
        }
    }
}
