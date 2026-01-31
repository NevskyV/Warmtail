using Data;
using Data.Player;
using Entities.Core;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.AbilitiesVisual
{
    public class InteractionVisuals : IAbilityVisual
    {
        [field: SerializeReference] public int AbilityIndex { get; set; }
        [SerializeField] private string _imageId;
        private GlobalData _globalData;
        private GameObject _image;
        
        [Inject]
        private void Construct(PlayerConfig config, GlobalData globalData)
        {
            var ability = config.Abilities[AbilityIndex];
            ability.StartAbility += StartAbility;
            ability.UsingAbility += UsingAbility;
            ability.EndAbility += EndAbility;
            _globalData = globalData;
            _image = SavableObjectsResolver.FindObjectById(_imageId);
        }
        public void StartAbility()
        {
            if (_globalData.Get<SavablePlayerData>().HasInteracted) return;
            _image.SetActive(true);
        }

        public void UsingAbility()
        {
            if (_globalData.Get<SavablePlayerData>().HasInteracted) return;
            _globalData.Edit<SavablePlayerData>(x => x.HasInteracted = true);
            _image.SetActive(false);
        }

        public void EndAbility()
        {
            if (_globalData.Get<SavablePlayerData>().HasInteracted) return;
            _image.SetActive(false);
        }
    }
}