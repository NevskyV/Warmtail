using AYellowpaper.SerializedCollections;
using Data;
using Data.Player;
using Systems.SequenceActions;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Tutorial
{
    public class TutorialSystem : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<string, TutorData> _tutors;

        private int _currentIndex;
        private TutorData _currentTutor;
        private GlobalData _globalData;
        private PlayerInput _input;
        
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
            if (_currentTutor.Sequence[_currentIndex].Tasks.Count > 0)
            {
                foreach (var task in _currentTutor.Sequence[_currentIndex].Tasks)
                {
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
