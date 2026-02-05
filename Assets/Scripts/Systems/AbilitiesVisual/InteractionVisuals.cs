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
        private GameObject _image;
        
        [Inject]
        private void Construct(PlayerConfig config)
        {
            var ability = config.Abilities[AbilityIndex];
            ability.StartAbility += StartAbility;
            ability.EndAbility += EndAbility;
            _image = SavableObjectsResolver.FindObjectById(_imageId);
        }
        public void StartAbility()
        {
            _image.SetActive(true);
        }

        public void UsingAbility()
        {
            _image.SetActive(false);
        }

        public void EndAbility()
        {
            _image.SetActive(false);
        }
    }
}