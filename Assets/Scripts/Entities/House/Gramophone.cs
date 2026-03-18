using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using Data.House;
using Entities.Sound;
using Entities.UI;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Entities.House
{
    public class Gramophone : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<AudioClip> _tracks;
        private GlobalData _globalData;
        private UIStateSystem _uiStateSystem;
        private MusicStateSystem _musicStateSystem;
        
        [Inject]
        private void Construct(GlobalData globalData, UIStateSystem uiStateSystem,MusicStateSystem musicStateSystem)
        {
            _globalData = globalData;
            _uiStateSystem = uiStateSystem;
            _musicStateSystem = musicStateSystem;
            
            _musicStateSystem.HomeTrack =  _tracks[_globalData.Get<HouseData>().HouseTrack];
        }
        
        public void Interact()
        {
            _uiStateSystem.SwitchCurrentStateAsync(UIState.MusicSelection).Forget();
        }

        public void ChangeTrack(int trackInd)
        {
            _musicStateSystem.HomeTrack =  _tracks[trackInd];
            _globalData.Edit<HouseData>(x => x.HouseTrack = trackInd);
        }
    }
}