using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using Entities.Localization;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.UI
{
    public class TailsVisuals : MonoBehaviour
    {
        [SerializeField] private List<FearConfig> _fears;
        [SerializeField] private List<SpriteRenderer> _playerSpites;
        [SerializeField] private List<TrailRenderer> _trails;
        [SerializeField] private LocalizedText _nameText;
        [SerializeField] private LocalizedText _descriptionText;
        [SerializeField] private float _speed;
        [SerializeField] private float _radius;

        [Inject] private GlobalData _globalData;
        [Inject] private UIStateSystem _uiStateSystem;
        [Inject] private PlayerInput _playerInput;
        [Inject] private CinemachineCamera _cam;
        private Vector2 _center;
        private TrailRenderer _currentTrail;
        private float _angle;

        private void Start()
        {
            _playerInput.actions["FearMenu"].performed += _ =>
            {
                if (_globalData.Get<DialogueVarData>().Variables.Find(x => x.Name == "fearMenuOpen").Value == "true")
                {
                    if(_uiStateSystem.CurrentState == UIState.Normal)
                        _uiStateSystem.SwitchCurrentStateAsync(UIState.FearMenu).Forget();
                    else if (_uiStateSystem.CurrentState == UIState.FearMenu)
                        _uiStateSystem.SwitchCurrentStateAsync(UIState.Normal).Forget();
                }
                    
                    
            };
            _uiStateSystem.OnStateChange += state =>
            {
                if(state == UIState.FearMenu) ShowVisuals();
                else HideVisuals();
            };

            SwitchCurrentTrail(0);
        }
        
        private void ShowVisuals()
        {
            foreach (var sprite in _playerSpites)
            {
                sprite.enabled = false;
            }
            
            var data = _globalData.Get<SavablePlayerData>();
            _currentTrail = _trails[data.FearIds[data.CurrentTrail]];
            _center = _currentTrail.transform.position;
        }

        private void HideVisuals()
        {
            foreach (var sprite in _playerSpites)
            {
                sprite.enabled = true;
            }

            if(_currentTrail)_currentTrail.transform.position = _center;
            _currentTrail = null;
        }

        public void SwitchCurrentTrail(int offset)
        {
            var data = _globalData.Get<SavablePlayerData>();
            var newTrail = data.FearIds[data.CurrentTrail + offset];
            if(newTrail >= data.FearIds.Count) newTrail = 0;
            else if(newTrail < 0) newTrail =  data.FearIds.Count - 1;
            _globalData.Edit<SavablePlayerData>(data => data.CurrentTrail = newTrail);
            for (int i = 0; i < _trails.Count; i++)
            {
                _trails[i].gameObject.SetActive(i == newTrail);
            }
            _currentTrail = _currentTrail? _trails[newTrail] : null;
            _nameText.SetNewKey(_fears[newTrail].Name);
            _descriptionText.SetNewKey(_fears[newTrail].Description);
        }

        private void Update()
        {
            if (_currentTrail)
            {
                _angle += _speed * Time.deltaTime;
                
                float x = Mathf.Cos(_angle) * _radius;
                float y = Mathf.Sin(_angle) * _radius;
                
                _currentTrail.transform.position = _center + new Vector2(x, y);
            }
        }
    }
}