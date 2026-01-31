using Data.Player;
using Entities.PlayerScripts;
using Entities.Sound;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.AbilitiesVisual
{
    public class WarmingVisual : IAbilityVisual
    {
        private static readonly int OpacityMax = Shader.PropertyToID("_Opacity");
        [field: SerializeReference] public int AbilityIndex {get; set;}
        [SerializeField] private Material _warmthMaterial;
        [SerializeField, Range(0, 1f)] private float _opacityNotUsing;
        [SerializeField, Range(0, 1f)] private float _opacityUsing;
        [SerializeField] private AudioClip _sfx;
        private ObjectSfx _playerSfx;
        private IAbility _ability;

        [Inject]
        private void Construct(PlayerConfig config, Player player)
        {
            var ability = config.Abilities[AbilityIndex];
            ability.StartAbility += StartAbility;
            ability.UsingAbility += UsingAbility;
            ability.EndAbility += EndAbility;
            _playerSfx = player.ObjectSfx;
            _ability = ability;
        }
        
        public void StartAbility()
        {
            if (!_ability.Enabled) return;
            _warmthMaterial.SetFloat(OpacityMax, _opacityNotUsing);
            _playerSfx.PlayLoopSfx(_sfx);
        }

        public void UsingAbility()
        {
            _warmthMaterial.SetFloat(OpacityMax, _opacityUsing);
        }

        public void EndAbility()
        {
            _warmthMaterial.SetFloat(OpacityMax, 0);
            _playerSfx.StopLoopSfx();
        }
    }
}