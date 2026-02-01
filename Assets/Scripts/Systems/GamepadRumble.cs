using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems
{
    public class GamepadRumble
    {
        [Inject] private GlobalData _globalData;
        [Inject] private PlayerInput _playerInput;
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
    }
}