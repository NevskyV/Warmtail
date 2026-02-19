using Data;
using Data.Player;
using Entities.Core;
using Entities.UI;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.AbilitiesVisual
{
    public class InteractionVisuals : IAbilityVisual
    {
        [field: SerializeReference] public int AbilityIndex { get; set; }
        [SerializeField] private InputActionAsset _reference;
        [SerializeField] private string _actionName;
        private TipsVisuals _tipsVisuals;
        
        [Inject]
        private void Construct(PlayerConfig config, TipsVisuals tipsVisuals)
        {
            var ability = config.Abilities[AbilityIndex];
            ability.StartAbility += StartAbility;
            ability.EndAbility += EndAbility;
            _tipsVisuals =  tipsVisuals;
        }
        public void StartAbility()
        {
            _tipsVisuals.ShowTip(_reference[_actionName]);
        }

        public void UsingAbility()
        {
        }

        public void EndAbility()
        {
            _tipsVisuals.HideTip(_reference[_actionName]);
        }
    }
}