using Data;
using Zenject;

namespace Systems
{
    public class BestiarySystem
    {
        private GlobalData _globalData;
        private ShoppingSystem _shoppingSystem;
        private CompanionPassiveSystem _companionPassiveSystem;
        private CompanionItemData _companionItemData;
        
        [Inject] 
        private void Construct(GlobalData globalData, ShoppingSystem shoppingSystem,
            CompanionPassiveSystem companionPassiveSystem)
        {
            _globalData = globalData;
            _shoppingSystem = shoppingSystem;
            _companionPassiveSystem = companionPassiveSystem;
        }
        
        public void AddCreature(int id)
        {
            _globalData.Edit<WorldData>(x => x.CreaturesIds.Add(id));
            CheckCompletion();
        }
        
        private void CheckCompletion()
        {
            var worldData = _globalData.Get<WorldData>();
            if (worldData.CreaturesIds.Count == worldData.MaxCreatures)
            {
                Reward();
            }
        }

        private void Reward()
        {
            _shoppingSystem.BuyItem(_companionItemData, Character.Finix, true);
            if (_companionItemData != null)
                _companionPassiveSystem?.Register(_companionItemData.Type);
        }
    }
}