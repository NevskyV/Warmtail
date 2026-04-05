using Data;
using Data.Player;
using Entities.Puzzle.Modules;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Creatures
{
    public class PullableCreature : MovableCreature
    {
        [Inject] private GlobalData _globalData;
        private ColdFishLureModule _module;
        
        public void Initialize(ColdFishLureModule module)
        {
            _module = module;
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _globalData.Get<RuntimePlayerData>().Temperature > TemperatureSystem.Neutral)
            {
                UpdateTarget(other.transform);
            }
            else if (other.CompareTag("FishDestination"))
            {
                Solve();
            }
        }
        
        private void Solve()
        {
            UpdateTarget(null);
            _module?.Solve();
        }
    }
}