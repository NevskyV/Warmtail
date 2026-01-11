using System.Collections.Generic;
using System.Linq;
using Data.Player;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Entities.UI;
using Interfaces;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Probs
{
    public class Star : SavableStateObject, IInteractable
    {
        private MonologueVisuals _monologueVisuals;
        private List<int> _ids = new();
        
        [Inject]
        public void Construct(MonologueVisuals monologueVisuals)
        {
            _monologueVisuals = monologueVisuals;
            for (int i = 0; i < 30; i++)
            {
                _ids.Add(i);
            }
        }
    
        public void Interact()
        {
            if (_globalData == null) return;
            var newId = _ids.Except(_globalData.Get<SavablePlayerData>().SeenReplicas).GetRandom();
            _monologueVisuals.RequestSingleLine(newId);
            _globalData.Edit<SavablePlayerData>((playerData) =>
            {
                playerData.Stars += 1;
                playerData.SeenReplicas.Add(newId);
                var v = new Vector2(transform.position.x, transform.position.y);
                playerData.RespawnPositions.Add(v.ToNumerics());
            });
            ChangeState(false);
        }
    }
}
