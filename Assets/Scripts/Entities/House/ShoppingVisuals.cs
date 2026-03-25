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
        [SerializeField,FormerlySerializedAs("allNpcInfo")] private NPCInfoForShop[] _allNpcInfo;

        [SerializeField] private BuyButton[] _itemsButtons;
        [SerializeField] private Button[] _levelButtons;

        private UIStateSystem _uiStateSystem;
        private GlobalData _globalData;
        private NPCMethods _npcMethods;

        [Inject]
        private void Construct(UIStateSystem uiStateSystem, GlobalData globalData, NPCMethods npcMethods)
        {
            _uiStateSystem = uiStateSystem;
            _globalData = globalData;
            _npcMethods = npcMethods;
        }


        public void OpenNpcShop(int num) => OpenNpcShop((Character)num);
        public void OpenNpcShop(Character character)
        {
            _npcMethods.CheckNpcData(character);

            NPCInfoForShop npcInfoForShop = _allNpcInfo[(int)character];
            int levelCount = npcInfoForShop.LevelCount;
            int curLvl = _globalData.Get<NPCData>().Levels[character];
            
            for (int i = 0; i < levelCount; i ++)
            {
                ShopItem item = npcInfoForShop.ShopItemList[i];

                _itemsButtons[i].Initialize(item, character, (item.NeedLevel <= curLvl));
                if (i + 1 == levelCount)
                {
                    ShopItem nextItem = npcInfoForShop.ShopItemList[i+1];
                    _itemsButtons[i+1].Initialize(nextItem, character, 
                    ((nextItem.NeedLevel <= curLvl) && (!_globalData.Get<NPCData>().BoughtLastItem[character])));
                }

                _levelButtons[i].interactable = (i < curLvl);
            }
            if (_uiStateSystem.CurrentState != UIState.Shop) _uiStateSystem.SwitchCurrentStateAsync(UIState.Shop);
        }
    }
}