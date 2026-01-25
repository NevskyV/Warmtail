using System;
using System.Collections.Generic;
using System.Numerics;
using AYellowpaper.SerializedCollections;

namespace Data.Player
{
    [Serializable]
    public class SavablePlayerData : ISavableData {
        public int Stars;
        public int Shells;
        public int ActiveLayers;
        public List<int> SeenReplicas;
        public List<Vector2> RespawnPositions = new(){new Vector2(-26.1f,4)};
        public SerializedDictionary<int, int> Inventory;
        public string TimeLastGame;
        public int TutorState;
        public bool HasBeatenGame;
        public bool IsHomeOpened;
        public string LastScene;
        public SerializedDictionary<int, List<int>> QuestIds;
    }
    
    [Serializable]
    public class RuntimePlayerData : IRuntimeData
    {
        public int CurrentWarmth;
        public bool WasInGame;
        public int Speed;
    }
}
