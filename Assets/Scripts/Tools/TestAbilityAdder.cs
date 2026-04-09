using Data;
using Systems.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAbilityAdder : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
            AbilitiesSystem.Instance?.AddAbility(AbilityType.Warming);

        if (Keyboard.current.f2Key.wasPressedThisFrame)
            AbilitiesSystem.Instance?.AddAbility(AbilityType.Resonance);

        if (Keyboard.current.f3Key.wasPressedThisFrame)
            AbilitiesSystem.Instance?.AddAbility(AbilityType.Dash);

        if (Keyboard.current.f4Key.wasPressedThisFrame)
            AbilitiesSystem.Instance?.AddAbility(AbilityType.Metabolism);
    }
#endif
}
