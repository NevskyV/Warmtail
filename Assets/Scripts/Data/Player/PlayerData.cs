using System;
using System.Collections.Generic;
using System.Numerics;
using AYellowpaper.SerializedCollections;

namespace Data.Player
{
    [Serializable]
    public class SavablePlayerData : ISavableData {
        public int OpenedAbilitiesCount;
        public int Stars;
        public int Shells;
        public int ActiveLayers;
        public List<int> SeenReplicas;
        public List<Vector2> RespawnPositions = new(){new Vector2(-25.5f, 1)};
        public SerializedDictionary<int, int> Inventory;
        public string TimeLastGame;
        public string EventsState;
        public string LastScene;
        public SerializedDictionary<int, List<int>> QuestIds;
        public List<int> FearIds = new();
        public int ActiveFearId = -1;
        
        public bool IsHomeOpened;
        public bool IsInFearWorld;
        public bool GotAmulet;
    }
    
    [Serializable]
    public class RuntimePlayerData : IRuntimeData
    {
        public int CurrentCells;
        public float CurrentCellProgress;
        public bool WasInGame;
        public int Speed;
    }
}
