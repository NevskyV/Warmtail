using Cysharp.Threading.Tasks;
using Systems;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Entities.PlayerScripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HeadCollision : MonoBehaviour
    {
        [SerializeField, Range(0f,20f)] private float _minVelocity = 5f;
        private GamepadRumble _rumble;
        private CinemachineBasicMultiChannelPerlin _camNoise;
        private Rigidbody2D _rb;

        [Inject]
        private void Construct(CinemachineCamera cam, GamepadRumble rumble)
        {
            _rumble = rumble;
            _camNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            _rb = GetComponent<Rigidbody2D>();
        }
        
        private async void OnCollisionEnter2D(Collision2D other)
        {
            if (_rb.linearVelocity.magnitude > _minVelocity)
            {
                _rumble.ShortRumble();
                _camNoise.enabled = true;
                await UniTask.Delay(500);
                _camNoise.enabled = false;
            }
        }
    }
}