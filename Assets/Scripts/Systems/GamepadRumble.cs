using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems
{
    public class GamepadRumble
    {
        private GlobalData _globalData;
        private PlayerInput _playerInput;
        [Inject] 
        private void Construct(GlobalData globalData, PlayerInput playerInput)
        {
            _globalData = globalData;
            _playerInput = playerInput;
            _playerInput.onControlsChanged += CheckDevice;
        }
        public async void ShortRumble()
        {
            if (Gamepad.current == null || _playerInput.currentControlScheme != "Gamepad") return;
            Gamepad.current.SetMotorSpeeds(_globalData.Get<SettingsData>().ShortLowRumble, _globalData.Get<SettingsData>().ShortHighRumble);
            await UniTask.Delay(100);
            Gamepad.current.ResetHaptics();
        }
        
        public void EnableRumble()
        {
            if (Gamepad.current == null || _playerInput.currentControlScheme != "Gamepad") return;
            Gamepad.current.SetMotorSpeeds(_globalData.Get<SettingsData>().LongLowRumble, _globalData.Get<SettingsData>().LongHighRumble);
        }

        public void DisableRumble()
        {
            if (Gamepad.current == null || _playerInput.currentControlScheme != "Gamepad") return;
            Gamepad.current.ResetHaptics();
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