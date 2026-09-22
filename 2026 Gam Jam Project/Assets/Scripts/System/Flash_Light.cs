using UnityEngine;
using UnityEngine.InputSystem;

public class Flash_Light : MonoBehaviour
{
    [SerializeField] GameObject FlashLight;
    private bool FlashLightIsOn;
    public InputAction FlashLightToggle;

    void Start()
    {
        FlashLightToggle.Enable();
    }

    void Update()
    {
        if (FlashLightToggle.WasPressedThisFrame())
        {
            TurnOnFlashLight();
        }
    }

    public void TurnOnFlashLight()
    {

        if (!FlashLightIsOn)
        {
            FlashLight.SetActive(true);
            FlashLightIsOn = true;
            Debug.Log("Flash Light Turned on");
        }
        else if (FlashLightIsOn)
        {
            FlashLightIsOn = false;
            FlashLight.SetActive(false);
        }
    }
}
