using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Data.NPCShop;
using Entities.UI;
using Systems;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Entities.House
{
    public class ShoppingVisuals : MonoBehaviour
    {
        [SerializeField,FormerlySerializedAs("allNpcInfo")] private List<NPCInfoForShop> _allNpcInfo;

        [SerializeField] private BuyButton[] _itemsButtons;

        private UIStateSystem _uiStateSystem;
        private GlobalData _globalData;

        [Inject]
        private void Construct(UIStateSystem uiStateSystem, GlobalData globalData)
        {
            _uiStateSystem = uiStateSystem;
            _globalData = globalData;
        }


        public void OpenNpcShop(int num) => OpenNpcShop((Character)num);
        public void OpenNpcShop(Character character)
        {
            CheckNpcData(character);

            NPCInfoForShop npcInfoForShop = _allNpcInfo.Find(x => x.Character == character);
            int levelCount = npcInfoForShop.LevelCount;

            for (int i = 0; i < levelCount; i ++)
            {
                ShopItem item = npcInfoForShop.ShopItemList[i];

                _itemsButtons[i].Initialize(item, character, true);
                if (i == levelCount)
                {
                    ShopItem nextItem = npcInfoForShop.ShopItemList[i+1];
                    _itemsButtons[i+1].Initialize(nextItem, character, !_globalData.Get<NPCData>().BoughtLastItem[character]);
                }
            }
            _uiStateSystem.SwitchCurrentStateAsync(UIState.Shop).Forget();
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