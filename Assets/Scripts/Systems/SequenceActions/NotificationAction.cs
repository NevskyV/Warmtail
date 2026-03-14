using Interfaces;
using UnityEngine;
using Entities.Core;
using Data;
using Cysharp.Threading.Tasks;
using Entities.Props;

namespace Systems.SequenceActions
{
    public class NotificationAction : ISequenceAction
    {
        [SerializeField] private GameObject _popup;
        [SerializeField] private string _parentId;
        private GameObject _object;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

        
        public void Invoke()
        {
            RectTransform parent = _eventsData.SceneObjects[_parentId].GetComponent<RectTransform>();
            _object = Object.Instantiate(_popup, parent);
            DestroyPopup();
        }
        private async UniTaskVoid DestroyPopup()
        {
            await UniTask.Delay(3500);
            Object.Destroy(_object);
        }
    }
}