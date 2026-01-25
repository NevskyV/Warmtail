using Entities.Props;
using Interfaces;
using UnityEngine.Events;
using UnityEngine;
using Systems;

namespace Entities.Puzzle
{
    public class DrawPuzzle : SavableStateObject, IPuzzle
    {
        [SerializeField] private float _during;
        [SerializeField] private DrawPuzzleTrigger[] _triggers;

        [HideInInspector] public UnityEvent OnReseted;
        public UnityEvent OnSolved = new();

        private ResettableTimer _timer;
        private bool _isComplete;
        private int _currentTrigger = -1;
        private int _activeTrigger;

        public void Start()
        {
            for (int i = 0; i < _triggers.Length; i ++)
                _triggers[i].Initialize(i, this);

            Reset();
        }

        public void Reset()
        {
            _currentTrigger = -1;
            if (_isComplete) return;
            if (_timer != null) _timer.Stop();
            OnReseted.Invoke();
        }

        public void Solve()
        {
            _isComplete = true;
            OnSolved.Invoke();
            Debug.Log("DrawPuzzle выполнено");
            Invoke("DestroyPuzzle", 0.5f);
        }

        private void DestroyPuzzle()
        {
           ChangeState(false);
        }

        public bool TriggerConform(int id, bool active)
        {
            if (!active) return false;
            else if (_currentTrigger == -1)
            {
                _activeTrigger = 1;
                _currentTrigger = id;
                if (_timer == null) _timer = new ResettableTimer(_during, Reset);
                _timer.Start();
                return true;
            }
            else if (id == _currentTrigger + 1 ||  (id == 0 && _currentTrigger == _triggers.Length - 1))
            {
                _activeTrigger ++;
                _currentTrigger ++;
                if (_currentTrigger >= _triggers.Length) _currentTrigger = 0;
                _timer.Start();
                if (_activeTrigger == _triggers.Length) Solve();
                return true;
            }
            else if (id == _currentTrigger - 1 || (_currentTrigger == 0 && id == _triggers.Length - 1))
            {
                _activeTrigger ++;
                _currentTrigger --;
                if (_currentTrigger < 0) _currentTrigger = _triggers.Length - 1;
                _timer.Start();
                if (_activeTrigger == _triggers.Length) Solve();
                return true;
            }
            else return false;
        }
    }
}
