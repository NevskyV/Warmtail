using Data;
using Data;
using Entities.Core;
using Entities.UI;
using Interfaces;
using UnityEngine;

namespace Systems.SequenceActions
{
    public class MarkAction : ISequenceAction
    {
        [SerializeField] private bool _spawn;
        [SerializeField] private QuestData _questData;
        [SerializeField] private Vector2 _position;
        [SerializeField] private string _questVisualsId;
        private EventsData _eventsData;

        public void SetEventsData(EventsData data)
        {
            _eventsData = data;
        }

        
        public void Invoke()
        {
            QuestVisuals q = _eventsData.SceneObjects[_questVisualsId].GetComponent<QuestVisuals>();
            if(_spawn) q.SpawnMarks(_questData, _position);
            else q.DestroyMark(_questData, _position);
        }
    }
}