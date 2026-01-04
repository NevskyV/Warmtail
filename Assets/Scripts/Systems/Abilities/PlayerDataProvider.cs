using Data;
using Data.Player;
using Interfaces;
using Zenject;

namespace Systems.Abilities
{
    public class PlayerDataProvider : IPlayerDataProvider
    {
        private GlobalData _globalData;

        [Inject]
        public void Construct(GlobalData globalData)
        {
            _globalData = globalData;
        }

        public int GetSpeed()
        {
            return _globalData.Get<RuntimePlayerData>().Speed;
        }

        public void SetSpeed(int speed)
        {
            _globalData.Edit<RuntimePlayerData>(data => data.Speed = speed);
        }
    }
}

