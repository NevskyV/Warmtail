using Entities.UI;
using UnityEngine.InputSystem;
using Zenject;

namespace EditorOnly
{
    public class KeysDebug
    {
        #if UNITY_EDITOR
        [Inject]
        private void Construct(PlayerInput input, UIStateSystem _uiState)
        {
            
            // foreach (var action in input.actions)
            // {
            //     if (action.actionMap.name == "Player")
            //     {
            //         action.performed += _ => popupSystem.ShowPopup(
            //             new NotificationPopup("Action performed", $"Pressed key: {action.name}", 2000));
            //     }
            // }
        }
        #endif
    }
}