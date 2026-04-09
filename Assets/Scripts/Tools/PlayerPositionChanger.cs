using Data;
using Systems.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;
using Data.Player;
using Entities.PlayerScripts;

public class PlayerPositionChanger : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Transform _player;
    [SerializeField] private Vector2 _posSpawn;
    [SerializeField] private Vector2 _posHome;

    private void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            SetPosition(_posSpawn);
        }

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            SetPosition(_posHome);
        }
    }
    
    private void SetPosition(Vector2 pos)
    {
        Transform _playerBody = _player.GetComponent<Player>().Rigidbody.transform;
        Vector2 bodyPos = _playerBody.position;
        _player.position = pos - (Vector2)_playerBody.position + (Vector2)_playerBody.parent.position;
    }
#endif
}
