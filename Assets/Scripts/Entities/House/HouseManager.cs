using AYellowpaper.SerializedCollections;
using Data;
using UnityEngine;
using Zenject;
using Systems;
using Data.House;
using Entities.NPC;

namespace Entities.House
{
    public class HouseManager : MonoBehaviour
    {
        public ItemsHouseIdConf _itemsHouseIdConf;
        [SerializeField] private SerializedDictionary<Character,SpeakableCharacter> _npc;
        [Inject] private PlacementSystem _placementSystem; 
        [Inject] private GlobalData _globalData; 

        void Awake()
        {
            EnableNpc(_globalData.Get<NpcSpawnData>().CurrentHomeNpc);
        }

        public void ApplyAllEditing()
        {
            _placementSystem.ApplyAllEditing();
        }
        public void CancelAll()
        {
            _placementSystem.CancelAll();
        }

        public void ResetInventory()
        {
            _placementSystem.ResetInventory();
        }

        private void EnableNpc(Character? character)
        {
            if(character != null && _npc.ContainsKey(character.Value))
                _npc[character.Value].ChangeState(true);
        }
    }
}
