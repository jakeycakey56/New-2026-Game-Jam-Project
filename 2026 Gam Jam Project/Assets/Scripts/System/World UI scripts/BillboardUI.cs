using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
            return;

        //make the world-space UI face the camera
        transform.LookAt(
            transform.position + mainCamera.transform.forward,
            mainCamera.transform.up
        );
    }
}