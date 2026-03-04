using UnityEngine;
using Interfaces;
using Data;
using AYellowpaper.SerializedCollections;
using Zenject;

namespace Systems.Tutorial
{
    public class TutorialSceneContext : MonoBehaviour
    {
        public SerializedDictionary<string, GameObject> SceneObjects;
        [Inject] private EventsData _eventsData;

        void Start()
        {
            foreach (var pair in SceneObjects)
            {
                _eventsData.SceneObjects[pair.Key] = pair.Value;
            }
        }
    }
}