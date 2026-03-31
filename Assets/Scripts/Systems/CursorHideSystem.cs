using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems
{
    public class CursorHideSystem
    {
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _playerInput.onControlsChanged += CheckDevice;
        }

        private void CheckDevice(PlayerInput input)
        {
            Debug.Log(input.currentControlScheme);
            if(input.currentControlScheme == "Gamepad")
                Cursor.visible = false;
            else if(input.currentControlScheme == "Keyboard&Mouse")
                Cursor.visible = true;
        }
    }
}