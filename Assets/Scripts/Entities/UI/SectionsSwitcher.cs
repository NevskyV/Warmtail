using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class SectionsSwitcher : Switcher
    {
        [SerializeField] private float _fadeTime = 2;
        [SerializeField] private List<CanvasGroup> _sections;
        [SerializeField] private List<Selectable> _sectionFirstObj;
        [SerializeField] private List<Selectable> _sectionLastObj;
        [SerializeField] private Selectable _leftArrow, _rightArrow, _backButton;

        [Inject]
        private void Construct(PlayerInput playerInput)
        {
            playerInput.actions["BottomButtons"].performed += x =>
            {
                if (x.ReadValue<float>() > 0) SwitchNext();
                else SwitchPrev();
            };
        }
        
        public override void Switch(int value)
        {
            _sections[CurrentValue].DOFade(0, _fadeTime);
            _sections[CurrentValue].blocksRaycasts = false;
            _sections[CurrentValue].interactable = false;
            
            base.Switch(value);
            _sections[value].DOFade(1, _fadeTime);
            _sections[value].blocksRaycasts = true;
            _sections[value].interactable = true;
            if(EventSystem.current.currentSelectedGameObject != _leftArrow.gameObject && 
               EventSystem.current.currentSelectedGameObject != _rightArrow.gameObject && 
               EventSystem.current.currentSelectedGameObject != _backButton.gameObject) EventSystem.current.SetSelectedGameObject(_rightArrow.gameObject);
            var navigation = _leftArrow.navigation;
            navigation.selectOnDown = _sectionFirstObj[value];
            _leftArrow.navigation = navigation;
            
            var rNavigation = _rightArrow.navigation;
            rNavigation.selectOnDown = _sectionFirstObj[value];
            _rightArrow.navigation = rNavigation;
            
            var bNavigation = _backButton.navigation;
            bNavigation.selectOnUp = _sectionLastObj[value];
            _backButton.navigation = bNavigation;
        }
    }
}