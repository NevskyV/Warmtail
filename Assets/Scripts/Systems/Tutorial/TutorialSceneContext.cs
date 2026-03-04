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
        private EventsData _eventsData;

        [Inject]
        private void Construct(EventsData eventsData)
        {
            Debug.Log("ira _eventsData 1" + _eventsData);
            _eventsData = eventsData;
            foreach (var pair in SceneObjects)
            {
                _eventsData.SceneObjects[pair.Key] = pair.Value;
            }
            Debug.Log("ira _eventsData 2" + _eventsData);
        }
    }
}