using System;
using Entities.Core;
using Entities.Probs;
using Interfaces;
using UnityEngine;

namespace Systems.Tasks
{
    public class SavabaleStateTask : ITask
    {
        public bool Completed { get; set; }
        public Action OnComplete { get; set; }
        private SavableStateObject _obj;
        [SerializeField] private string _objId;
        [SerializeField] private bool _needToBe;
        [TextArea] [SerializeField] private string description;

        public void Activate()
        {
            _obj = SavableObjectsResolver.FindObjectById<SavableStateObject>(_objId);
            _obj.OnStateChanged += b =>
            {
                if(b == _needToBe) MarkComplete();
            };
        }

        private void MarkComplete()
        {
            Completed = true;
            OnComplete?.Invoke();
            _obj.OnStateChanged -= b =>
            {
                if(b == _needToBe) MarkComplete();
            };
        }
    }
}