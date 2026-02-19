using System.Collections.Generic;
using Data;
using Data.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Entities.UI.Buttons
{
    public class NewAbilityCheckButton : MonoBehaviour
    {
        [SerializeField] private List<InputActionReference> _inputActions;
        [Inject] private TipsVisuals _tipsVisuals;
        [Inject] private GlobalData _globalData;

        public void Check()
        {
            if (_globalData.Get<SavablePlayerData>().OpenedAbilitiesCount == 2)
            {
                foreach (var action in _inputActions)
                {
                    _tipsVisuals.ShowTip(action);
                }
            }
        }
    }
}