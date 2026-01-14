using System;
using System.Collections.Generic;
using Data.Player;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Entities.PlayerScripts
{
    public class PlayerAbilityController : MonoBehaviour
    {
        private PlayerConfig _config;
        private DiContainer _container;
        private List<IAbility> _disabledAbilities = new();
        private List<IDisposable> _disposables = new();

        [Inject]
        private void Construct(PlayerConfig config, DiContainer container)
        {
            _config = config;
            _container = container;
        }

        public void Initialize()
        {
            foreach (var ability in _config.Abilities)
            {
                _container.Inject(ability);

                if (ability is IDisposable disposable)
                    _disposables.Add(disposable);

                if (ability.Visual != null)
                {
                    _container.Inject(ability.Visual);
                    if (ability.Visual is IDisposable disposableVisual)
                        _disposables.Add(disposableVisual);
                }
            }
        }
        
        public void DisableAllAbilities()
        {
            foreach (var ability in _config.Abilities)
            {
                if (ability.Enabled)
                {
                    _disabledAbilities.Add(ability);
                    ability.Enabled = false;
                }
            }
        }

        public void EnableLastAbilities()
        {
            foreach (var ability in _disabledAbilities)
            {
                ability.Enabled = true;
            }
            _disabledAbilities.Clear();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
