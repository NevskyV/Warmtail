using System.Collections.Generic;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Entities.Sound;
using Systems.AbilitiesVisual;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems.Environment
{
    public class SurfacingSystem : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levelRoots;
        [SerializeField] private List<Material> _waterMaterials;
        [SerializeField] private float _layerCheckRadius = 1f;
        [SerializeField] private LayerMask _surfaceTriggerLayer;
        [SerializeField] private LayerMask _obstacleLayer;

        private GlobalData _globalData;
        private Player _player;
        private PlayerConfig _config;
        private MusicStateSystem _music;
        private int _currentLayerIndex;
        public int CurrentLayerIndex => _currentLayerIndex;

        [Inject]
        public void Construct(GlobalData globalData, PlayerInput input, Player player, PlayerConfig config, MusicStateSystem music)
        {
            _music = music;
            _globalData = globalData;
            _player = player;
            _config = config;
            // UpdateLevelVisibility() перенесен в Start() чтобы вызваться после инициализации abilities
            input.actions["Surfacing"].started += ctx =>
            {
                var direction = (int)ctx.ReadValue<float>();
                TryChangeLayer(direction);
            };
        }

        private void Start()
        {
            // Вызывается после того как PlayerAbilityController.Initialize() проинжектировал все abilities
            UpdateLevelVisibility();
        }

        public void SetNewLevel(int level)
        {
            _currentLayerIndex = level;
            UpdateLevelVisibility();
        }
        
        public bool TryChangeLayer(int direction)
        {
            var newIndex = _currentLayerIndex + (int)direction;
            var maxLayers = _globalData.Get<SavablePlayerData>().ActiveLayers;

            if (newIndex < 0 || newIndex > maxLayers || newIndex >= _levelRoots.Count)
            {
                Debug.Log("TryChangeLayer неудачно: вне диапазона");
                return false;
            }

            if (!CanTransition(direction))
            {
                Debug.Log("TryChangeLayer неудачно: CanTransition вернул false");
                return false;
            }

            _currentLayerIndex = newIndex;
            UpdateLevelVisibility();
            return true;
        }


        private bool CanTransition(float direction)
        {
            if (direction != 0)
            {
                var pos = _player.Rigidbody.transform.position;
                bool inZone = Physics2D.OverlapCircle(pos, _layerCheckRadius, _surfaceTriggerLayer);
                bool blocked = Physics2D.Raycast(pos, Vector2.up, 3f, _obstacleLayer);
                return inZone && !blocked;
            }
            return false;
        }

        private void UpdateLevelVisibility()
        {
            ((MovementVisuals)_config.Abilities[0].Visual).Water = _waterMaterials[_currentLayerIndex];
            _music.ChangeMusicState(_currentLayerIndex);
            for (int i = 0; i < _levelRoots.Count; i++)
            {
                if (_levelRoots[i] != null)
                {
                    _levelRoots[i].SetActive(i == _currentLayerIndex);
                }
            }
        }

        public void AddMaxLevels()
        {
            if(_globalData.Get<SavablePlayerData>().ActiveLayers < 2)
                _globalData.Edit<SavablePlayerData>(data =>
                    data.ActiveLayers += 1);
        }
    }
}
