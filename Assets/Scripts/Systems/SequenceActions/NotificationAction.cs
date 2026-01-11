using Data;
using Interfaces;
using UnityEngine;
using Entities.Core;
using Entities.Probs;
using Entities.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Systems.SequenceActions
{
    public class NotificationAction : ISequenceAction
    {
        [SerializeField] private GameObject _popup;
        [SerializeField] private string _parentId;
        private GameObject _object;
        
        public void Invoke()
        {
            RectTransform parent = SavableObjectsResolver.FindObjectById<SavableStateObject>(_parentId).GetComponent<RectTransform>();
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