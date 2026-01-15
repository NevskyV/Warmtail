using AYellowpaper.SerializedCollections;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Data;
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

        private void Awake()
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

        public void LeaveNpc(int character)
        {
            _globalData.Edit<NpcSpawnData>(data => data.CurrentHomeNpc = (Character)0);
            LeaveNpcAsync((Character)character);
        }
        private async void LeaveNpcAsync(Character character)
        {
            await Task.Delay(10000);
            if (_npc.ContainsKey(character)) _npc[character].ChangeState(false);
        }
    }
}