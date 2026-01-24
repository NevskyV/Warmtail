using Data.Player;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class AbilitiesUI : MonoBehaviour
    {
        [Title("Images to fill")]
        [SerializeField] private Image[] _images;

        [Title("Shader Settings")] 
        [SerializeField] private float _defaultOutWidth;
        [SerializeField] private float _selectedOutWidth;
        [SerializeField] private float _defaultInWidth;
        [SerializeField] private float _selectedInWidth;
        [SerializeField] private float _defaultRhombusSize;
        [SerializeField] private float _selectedRhombusSize;
        
        private InputActionAsset _inputActions;
        private InputAction[] _keys;
        private InputAction _rightClick;

        private bool[] _selected = new bool[4];
        private int _pendingSecond = -1;
        private float _pendingTime;
        private float _comboTime = 0.2f;

        private int _selectedIndex;
        private PlayerConfig _playerConfig;

        [Inject]
        private void Construct(PlayerConfig playerConfig, PlayerInput playerInput)
        {
            _playerConfig = playerConfig;
            _inputActions = playerInput.actions;
        }

        void Awake()
        {
            InitializeAbilityIcons();

            _keys = new []
            {
                _inputActions.FindAction("1"),
                _inputActions.FindAction("2"),
                _inputActions.FindAction("3"),
                _inputActions.FindAction("4")
            };

            foreach (var k in _keys) k.Enable();

            _inputActions["Scroll"].performed += ctx => CycleSelection(ctx.ReadValue<Vector2>().y);

            _rightClick = _inputActions.FindAction("RightMouse");
            _rightClick.Enable();
        }

        private void InitializeAbilityIcons()
        {
            if (_playerConfig == null || _playerConfig.Abilities == null)
                return;
            
            for (int i = 0; i < _images.Length && i < _playerConfig.Abilities.Count; i++)
            {
                var ability = _playerConfig.Abilities[i];
                if (ability != null && ability.Visual != null && ability.Visual.Icon != null && _images[i] != null)
                {
                    _images[i].sprite = ability.Visual.Icon;
                }
            }
        }

        void Update()
        {
            if (_rightClick.IsPressed())
                return;

            int pressedThisFrame = -1;

            for (int i = 0; i < 4; i++)
            {
                if (_keys[i].WasPressedThisFrame())
                {
                    pressedThisFrame = i;
                    break;
                }
            }

            if (pressedThisFrame != -1)
            {
                if (_pendingSecond != -1 && Time.time - _pendingTime <= _comboTime)
                {
                    _selected[pressedThisFrame] = true;
                    _selected[_pendingSecond] = true;
                    _pendingSecond = -1;
                }
                else
                {
                    for (int i = 0; i < 4; i++) _selected[i] = false;

                    _selected[pressedThisFrame] = true;
                    _pendingSecond = pressedThisFrame;
                    _pendingTime = Time.time;
                }

                _selectedIndex = pressedThisFrame;
            }

            int count = 0;
            for (int i = 0; i < 4; i++) if (_selected[i]) count++;
            float scale = count == 1 ? 1.30f : count == 2 ? 1.20f : 1f;

            for (int i = 0; i < 4; i++)
                _images[i].rectTransform.localScale = _selected[i] ? Vector3.one * scale : Vector3.one;

            for (int i = 0; i < 4; i++)
            {
                if (_selected[i])
                {
                    _selectedIndex = i;
                    break;
                }
            }
        }


        private void CycleSelection(float scrollValue)
        {
            if (_rightClick.IsPressed())
                return;

            if (scrollValue == 0) return;

            int newIndex = _selectedIndex + (scrollValue > 0 ? 1 : -1);
            if (newIndex > 3) newIndex = 0;
            if (newIndex < 0) newIndex = 3;

            SelectAbility(newIndex);
        }

        private void SelectAbility(int index)
        {
            for (int i = 0; i < 4; i++) _selected[i] = false;
            _selected[index] = true;
            
            _selectedIndex = index;
        }
    }
}
