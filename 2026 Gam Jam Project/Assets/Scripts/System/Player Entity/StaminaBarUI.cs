using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Slider staminaSlider;

    private void Start()
    {
        if (playerStamina == null || staminaSlider == null)
            return;

        staminaSlider.minValue = 0f;
        staminaSlider.maxValue = playerStamina.MaxStamina;
        staminaSlider.value = playerStamina.CurrentStamina;
    }

    private void Update()
    {
        if (playerStamina == null || staminaSlider == null)
            return;

        staminaSlider.value = playerStamina.CurrentStamina;
    }
}