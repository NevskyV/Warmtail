using Cysharp.Threading.Tasks;
using Data;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems
{
    public class GamepadRumble
    {
        [Inject] private GlobalData _globalData;
        public async void ShortRumble()
        {
            Gamepad.current.SetMotorSpeeds(_globalData.Get<SettingsData>().ShortLowRumble, _globalData.Get<SettingsData>().ShortHighRumble);
            await UniTask.Delay(100);
            Gamepad.current.ResetHaptics();
        }
        
        public void EnableRumble()
        {
            Gamepad.current.SetMotorSpeeds(_globalData.Get<SettingsData>().LongLowRumble, _globalData.Get<SettingsData>().LongHighRumble);
        }

        public void DisableRumble()
        {
            Gamepad.current.ResetHaptics();
        }
    }
}