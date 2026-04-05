using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
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
        [SerializeField] private float _speed;
        [SerializeField] private float _radius;

        [Inject] private GlobalData _globalData;
        [Inject] private UIStateSystem _uiStateSystem;
        [Inject] private PlayerInput _playerInput;
        [Inject] private CinemachineCamera _cam;
        private Vector2 _center;
        private TrailRenderer _currentTrail;

        private void Start()
        {
            _playerInput.actions["FearMenu"].performed += _ =>
            {
                if(_globalData.Get<DialogueVarData>().Variables.Find(x => x.Name == "fearMenuOpen").Value == "true")
                    _uiStateSystem.SwitchCurrentStateAsync(UIState.FearMenu).Forget();
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

            _currentTrail = _trails[_globalData.Get<SavablePlayerData>().CurrentTrail];
        }

        private void HideVisuals()
        {
            foreach (var sprite in _playerSpites)
            {
                sprite.enabled = true;
            }
            
            _currentTrail = null;
        }

        public void SwitchCurrentTrail(int offset)
        {
            var newTrail = _globalData.Get<SavablePlayerData>().CurrentTrail + offset;
            _globalData.Edit<SavablePlayerData>(data => data.CurrentTrail = newTrail);
            for (int i = 0; i < _trails.Count; i++)
            {
                _trails[i].gameObject.SetActive(i == newTrail);
            }
            _currentTrail = _trails[newTrail];
        }

        private void Update()
        {
            if(_currentTrail)
                _currentTrail.transform.localPosition = 
                    new Vector2(_center.x + Mathf.Cos(_speed * Time.deltaTime) * _radius,
                                _center.y + Mathf.Sin(_speed * Time.deltaTime) * _radius);
        }
    }
}