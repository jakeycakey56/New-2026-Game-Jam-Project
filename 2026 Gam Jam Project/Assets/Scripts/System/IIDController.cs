using UnityEngine;
using UnityEngine.InputSystem;

public class IIDController : MonoBehaviour
{
    [SerializeField] GameObject FlashLight;
    private bool FlashLightIsOn;
    public InputAction FlashLightToggle;

    //added this so we can check if the player actually owns the IID
    //eventually the shop will change this when the player buys one
    [SerializeField] private bool ownsIID = true;

    [Header("Battery Settings")]

    //how much charge a fresh battery has
    [SerializeField] private float maxBatteryCharge = 100f;

    //how much charge is currently left in the battery inside the IID
    [SerializeField] private float batteryCharge = 100f;

    //how much battery the flashlight uses every second while turned on
    [SerializeField] private float batteryDrainPerSecond = 5f;

    //how many extra batteries the player is currently carrying
    //eventually we'll get these from vending machines
    [SerializeField] private int spareBatteries = 3;

    [Header("Reload Settings")]

    //keeping reload as its own InputAction so we can change the button later if we want
    public InputAction ReloadIID;

    void Start()
    {
        FlashLightToggle.Enable();

        //enable the reload input too
        ReloadIID.Enable();

        //making sure the flashlight always starts turned off
        FlashLight.SetActive(false);
        FlashLightIsOn = false;
    }

    void Update()
    {
        if (FlashLightToggle.WasPressedThisFrame())
        {
            TurnOnFlashLight();
        }

        //if the flashlight is currently on, drain the loaded battery
        if (FlashLightIsOn)
        {
            DrainBattery();
        }

        //reload is completely manual
        //running out of charge will NOT automatically use another battery
        if (ReloadIID.WasPressedThisFrame())
        {
            ReloadBattery();
        }
    }

    public void TurnOnFlashLight()
    {
        //don't let the player turn it on if they haven't bought the IID
        if (!ownsIID)
        {
            Debug.Log("Player does not own an IID");
            return;
        }

        //don't let the player turn the flashlight on with a dead battery
        if (!FlashLightIsOn && batteryCharge <= 0f)
        {
            Debug.Log("IID battery is dead - Reload!");
            return;
        }

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

    private void DrainBattery()
    {
        //drain battery based on how long the flashlight has actually been running
        batteryCharge -= batteryDrainPerSecond * Time.deltaTime;

        //don't allow the battery charge to go below 0
        if (batteryCharge <= 0f)
        {
            batteryCharge = 0f;

            //the battery died, so shut the light off
            //IMPORTANT: we are NOT automatically reloading here
            FlashLightIsOn = false;
            FlashLight.SetActive(false);

            Debug.Log("IID battery died!");
        }
    }

    private void ReloadBattery()
    {
        //can't reload something we don't own
        if (!ownsIID)
        {
            Debug.Log("Player does not own an IID");
            return;
        }

        //don't use a spare battery if the current battery is already full
        if (batteryCharge >= maxBatteryCharge)
        {
            Debug.Log("IID battery is already full");
            return;
        }

        //can't reload if we don't have another battery
        if (spareBatteries <= 0)
        {
            Debug.Log("No spare batteries!");
            return;
        }

        //use one spare battery
        spareBatteries--;

        //replace the current battery with a completely fresh one
        //any charge left in the old battery is lost
        batteryCharge = maxBatteryCharge;

        Debug.Log("IID reloaded! Spare batteries remaining: " + spareBatteries);
    }
}