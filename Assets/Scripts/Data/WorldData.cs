using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace Data
{
    public class WorldData : ISavableData
    {
        public SerializedDictionary<string, bool> SavableObjects = new();
        public SerializedDictionary<string, int> SavableNpcState = new();
        public List<int> CollectedStars = new();
    }

}