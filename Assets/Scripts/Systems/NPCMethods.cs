using Entities.NPC;
using Zenject;
using Data;
using Data.NPCShop;
using Entities.House;

namespace Systems
{
    public class NPCMethods 
    {
        [Inject] private ShoppingVisuals _shoppingVisuals;
        [Inject] private GlobalData _globalData;

        public void OpenNPCShop(int num) => _shoppingVisuals.OpenNpcShop((Character)num);

        public void RaiseFriendship(int num) => RaiseFriendship((Character)num);
        public void RaiseFriendship(Character character)
        {
            CheckNpcData(character);
            _globalData.Edit<NPCData>(data =>{data.Levels[character] ++;});
        }

        public void CheckNpcData(Character character)
        {
            if (_globalData.Get<NPCData>().Levels == null)
                _globalData.Edit<NPCData>(data =>{data.Levels = new();});

            if (!_globalData.Get<NPCData>().Levels.ContainsKey(character))
                _globalData.Edit<NPCData>(data =>{data.Levels[character] = 1;});

            if (_globalData.Get<NPCData>().BoughtLastItem == null)
                _globalData.Edit<NPCData>(data =>{data.BoughtLastItem = new();});
            
            if (!_globalData.Get<NPCData>().BoughtLastItem.ContainsKey(character))
                _globalData.Edit<NPCData>(data =>{data.BoughtLastItem[character] = false;});
        }
    }
}
