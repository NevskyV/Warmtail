using UnityEngine;
using Interfaces;
using Systems.Tutorial;
using AYellowpaper.SerializedCollections;

namespace Data
{
    public class EventsData
    {
        public SerializedDictionary<string, GameObject> SceneObjects = new();
    }
}