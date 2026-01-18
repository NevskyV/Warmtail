using Data.Player;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Zenject;

public class AbilitiesUI : MonoBehaviour
{
    public Image[] images;
    public InputActionAsset inputActions;

    private InputAction[] keys;
    private InputAction rightClick;

    private bool[] selected = new bool[4];
    private int pendingSecond = -1;
    private float pendingTime = 0f;
    private float comboTime = 0.2f;

    private int selectedIndex = 0;
    private PlayerConfig _playerConfig;

    [Inject]
    private void Construct(PlayerConfig playerConfig)
    {
        _playerConfig = playerConfig;
    }

    void Awake()
    {
        // Устанавливаем иконки из PlayerConfig
        InitializeAbilityIcons();

        keys = new InputAction[]
        {
            inputActions.FindAction("1"),
            inputActions.FindAction("2"),
            inputActions.FindAction("3"),
            inputActions.FindAction("4")
        };

        foreach (var k in keys) k.Enable();

        inputActions["Scroll"].performed += ctx => CycleSelection(ctx.ReadValue<Vector2>().y);

        rightClick = inputActions.FindAction("RightMouse");
        rightClick.Enable();
    }

    private void InitializeAbilityIcons()
    {
        if (_playerConfig == null || _playerConfig.Abilities == null)
            return;

        // Устанавливаем иконки для первых 4 способностей
        for (int i = 0; i < images.Length && i < _playerConfig.Abilities.Count; i++)
        {
            var ability = _playerConfig.Abilities[i];
            if (ability != null && ability.Visual != null && ability.Visual.Icon != null && images[i] != null)
            {
                images[i].sprite = ability.Visual.Icon;
            }
        }
    }

    void Update()
    {
        if (rightClick.IsPressed())
            return;

        int pressedThisFrame = -1;

        for (int i = 0; i < 4; i++)
        {
            if (keys[i].WasPressedThisFrame())
            {
                pressedThisFrame = i;
                break;
            }
        }

        if (pressedThisFrame != -1)
        {
            if (pendingSecond != -1 && Time.time - pendingTime <= comboTime)
            {
                selected[pressedThisFrame] = true;
                selected[pendingSecond] = true;
                pendingSecond = -1;
            }
            else
            {
                for (int i = 0; i < 4; i++) selected[i] = false;

                selected[pressedThisFrame] = true;
                pendingSecond = pressedThisFrame;
                pendingTime = Time.time;
            }

            selectedIndex = pressedThisFrame;
        }

        int count = 0;
        for (int i = 0; i < 4; i++) if (selected[i]) count++;
        float scale = count == 1 ? 1.30f : count == 2 ? 1.20f : 1f;

        for (int i = 0; i < 4; i++)
            images[i].rectTransform.localScale = selected[i] ? Vector3.one * scale : Vector3.one;

        for (int i = 0; i < 4; i++)
        {
            if (selected[i])
            {
                selectedIndex = i;
                break;
            }
        }
    }


    private void CycleSelection(float scrollValue)
    {
        if (rightClick.IsPressed())
            return;

        if (scrollValue == 0) return;

        int newIndex = selectedIndex + (scrollValue > 0 ? 1 : -1);
        if (newIndex > 3) newIndex = 0;
        if (newIndex < 0) newIndex = 3;

        SelectAbility(newIndex);
    }

    private void SelectAbility(int index)
    {
        for (int i = 0; i < 4; i++) selected[i] = false;
        selected[index] = true;

        selectedIndex = index;
    }
}
