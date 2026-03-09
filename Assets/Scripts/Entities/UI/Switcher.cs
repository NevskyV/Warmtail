using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Entities.UI
{
    public class Switcher : MonoBehaviour
    {
        [SerializeField] private LocalizedText _localizedText;
        [SerializeField] private List<string> _id = new();
        [SerializeField] private RectTransform _switchesParent;
        [SerializeField] private RectTransform _transitionDot;
        [SerializeField, Range(0,5f)] private float _transitionTime;
        public UnityEvent<int> Event { get; set; } = new();
        public int CurrentValue { get; set; }
        private List<Transform> _images = new();

        private async void Start()
        {
            for (int i = 0; i < _switchesParent.childCount; i++)
            {
                _images.Add(_switchesParent.GetChild(i));
            }
            Switch(CurrentValue);

            _switchesParent.gameObject.SetActive(false);
            _switchesParent.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_switchesParent);
            Canvas.ForceUpdateCanvases();
            SceneView.RepaintAll();
            await UniTask.WaitForSeconds(_transitionTime);
            _transitionDot.localPosition = new Vector3(_images[CurrentValue].transform.localPosition.x, 0,0);
        }

        public void SwitchNext()
        {
            var newValue = CurrentValue + 1;
            if (newValue == _images.Count) newValue = 0;
            Event.Invoke(newValue);
            Switch(newValue);
        }
        
        public void SwitchPrev()
        {
            var newValue = CurrentValue - 1;
            if (newValue < 0) newValue = _images.Count - 1;
            Event.Invoke(newValue);
            Switch(newValue);
        }
        
        public virtual void Switch(int value)
        {
            CurrentValue = value;
            _transitionDot.DOLocalMoveX(_images[CurrentValue].transform.localPosition.x, _transitionTime);
            _localizedText.SetNewKey(_id[value]);
        }
    }
}