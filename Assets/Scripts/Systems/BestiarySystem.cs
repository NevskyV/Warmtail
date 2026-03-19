using Data;
using Zenject;

namespace Systems
{
    public class BestiarySystem
    {
        private GlobalData _globalData;
        private ShoppingSystem _shoppingSystem;
        private CompanionItemData _companionItemData;
        
        [Inject] 
        private void Construct(GlobalData globalData, ShoppingSystem shoppingSystem)
        {
            _globalData = globalData;
            _shoppingSystem = shoppingSystem;
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
            _shoppingSystem.BuyItem(_companionItemData,Character.Finix,true,0);
        }
    }
}