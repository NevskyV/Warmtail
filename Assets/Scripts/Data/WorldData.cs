using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace Data
{
    public class WorldData : ISavableData
    {
        public SerializedDictionary<string, bool> SavableObjects = new();
        public SerializedDictionary<string, int> SavableNpcState = new();
        public List<int> CollectedStars = new();
        public HashSet<int> CreaturesIds = new();
        public int MaxCreatures;
        public List<int> ActivatedStatues = new(){0};
        public List<int> ManualWorld;
        public List<int> ManualCharacter;
        public List<int> ManualPuzzle;
    }

}