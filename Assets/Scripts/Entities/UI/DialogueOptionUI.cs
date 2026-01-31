using System;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    [ExecuteAlways]
    public class DialogueOptionUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _sdfGroup;
        [Inject] private DialogueVisuals _dialogueVisuals;
        
        public void ChooseOption()
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                {
                    _dialogueVisuals.ChooseOption(i);
                    break;
                }
            }
        }
        
        public void Update()
        {
            _sdfGroup.sizeDelta = new Vector2(_sdfGroup.sizeDelta.x, _sdfGroup.rect.size.x);
        }
    }
}