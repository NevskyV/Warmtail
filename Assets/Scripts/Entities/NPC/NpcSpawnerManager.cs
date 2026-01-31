using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using Rng = UnityEngine.Random;
using UnityEngine;
using Zenject;
using Systems;
using Data;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;

namespace Entities.NPC
{
    public class NpcSpawnerManager : MonoBehaviour
    {
        public SerializedDictionary<Character, List<GameObject>> NpcSpawner;
        [Inject] private GlobalData _globalData;
        [SerializeField] private Transform[] _parentNpc;

        private void Awake()
        {
            DailySystem.OnLoadedResources += LoadNpc;
            DailySystem.OnDiscardedResources += DiscardNpc;
        }

        private void OnDestroy()
        {
            DailySystem.OnLoadedResources -= LoadNpc;
            DailySystem.OnDiscardedResources -= DiscardNpc;
        }

        private void LoadNpc()
        {
            foreach (var auto in _globalData.Get<NpcSpawnData>().NpcSpawnerData)
            {
                NpcSpawner[(Character)auto.Key][auto.Value].SetActive(true);
            }
        }
        private void DiscardNpc()
        {
            _globalData.Edit<NpcSpawnData>(data =>
            {
                data.NpcSpawnerData = new();
                for (int i = 0; i < NpcSpawner.Count; i ++)
                {
                    int prefId = (int)NpcSpawner.Keys.GetRandom();
                    int posId = Rng.Range(0, NpcSpawner[(Character)prefId].Count);
                    data.NpcSpawnerData[prefId] = posId;
                }
            });
            LoadNpc();
        }
    }
}
