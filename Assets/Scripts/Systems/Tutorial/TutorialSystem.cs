using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using Systems.SequenceActions;
using UnityEngine.InputSystem;
using UnityEngine;
using Interfaces;
using Zenject;
using Data;
using Data.Player;
using Interfaces;

namespace Systems.Tutorial
{
    public class TutorialSystem : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<string, TutorData> _tutors;

        private int _currentIndex;
        private TutorData _currentTutor;
        private GlobalData _globalData;
        private PlayerInput _input;

        public static List<ITask> TaskActivated = new();
        
        [Inject]
        private void Construct(GlobalData globalData, PlayerInput input)
        {
            _globalData = globalData;
            _input = input;
            _currentIndex = _globalData.Get<SavablePlayerData>().TutorState;
        }
        
        private void Start()
        {
            _tutors.TryGetValue(_input.currentControlScheme, out _currentTutor);
            if (!_currentTutor) return;
            for (int i = 0; i < _currentIndex; i++)
            {
                _currentTutor.Sequence[i].Actions.ForEach(x =>
                {
                    if (x is not StartQuestAction) x.Invoke();
                });
            }
            if (_currentIndex >= _currentTutor.Sequence.Count) return;

            SequenceElement element = _currentTutor.Sequence[_currentIndex];

            if (element.Tasks.Count > 0)
            {                
                foreach (var task in element.Tasks)
                {
                    if (TaskActivated.Contains(task)) return;
                    TaskActivated.Add(task);
                    
                    task.Activate();
                    task.OnComplete += TryIterateSequence;
                }
            }
            else
                TryIterateSequence();
        }
        
        public void TryIterateSequence()
        {
            var state = _globalData.Get<SavablePlayerData>().TutorState;
            SequenceIterationSystem.TryIterateSequence(_currentTutor.Sequence, state, QuestType.Serial,
                x =>
                {
                    _globalData.Edit<SavablePlayerData>(playerData => playerData.TutorState = x[0]);
                });
        }
    }
}
