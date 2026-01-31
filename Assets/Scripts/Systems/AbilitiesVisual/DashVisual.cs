using Data.Player;
using Entities.PlayerScripts;
using Entities.Sound;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.AbilitiesVisual
{
    public class DashVisual : IAbilityVisual
    {
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");
        [field: SerializeReference] public int AbilityIndex { get; set; }
        [SerializeField] private Material _waveMaterial;
        [SerializeField] private AudioClip _sfx;
        private ObjectSfx _playerSfx;
        private IAbility _ability;
        
        [Inject]
        private void Construct(PlayerConfig config, Player player)
        {
            var ability = config.Abilities[AbilityIndex];
            ability.StartAbility += StartAbility;
            ability.EndAbility += EndAbility;
            _ability = ability;
            _playerSfx = player.ObjectSfx;
        }

        public void StartAbility()
        {
            if (!_ability.Enabled) return;
            _playerSfx.PlaySfx(_sfx);
            _waveMaterial.SetFloat(Opacity, 1);
        }

        public void UsingAbility()
        {
        }

        public void EndAbility()
        {
            _waveMaterial.SetFloat(Opacity, 0);
        }
    }
}