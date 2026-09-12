using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 CameraOffSet = new Vector3(5f, 7f, 5f);
    public Transform PlayerPOS;

    [Header("Camera Rotation")]
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float rotationSmoothness = 6f;

    [Header("Camera Zoom")]
    [SerializeField] private float zoomSpeed = 3f;
    [SerializeField] private float minZoom = 0.6f;
    [SerializeField] private float maxZoom = 1.8f;
    [SerializeField] private float zoomSmoothness = 8f;

    private float cameraYaw = 0f;
    private float rotationVelocity = 0f;

    private float targetZoom = 1f;
    private float currentZoom = 1f;

    void Update()
    {
        HandleRotation();
        HandleZoom();

        //smoothly move the current zoom toward the target zoom
        currentZoom = Mathf.Lerp(
            currentZoom,
            targetZoom,
            zoomSmoothness * Time.deltaTime
        );

        //rotate the existing camera offset around the player
        Vector3 rotatedOffset =
            Quaternion.Euler(0f, cameraYaw, 0f) * CameraOffSet;

        //apply the current zoom level to the camera offset
        rotatedOffset *= currentZoom;

        //keep the camera centered around the player object
        transform.position = PlayerPOS.position + rotatedOffset;

        //make sure the camera continues looking at the player as it rotates
        transform.LookAt(PlayerPOS);
    }

    private void HandleRotation()
    {
        //while middle mouse is held, rotate based on mouse movement
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");

            rotationVelocity = mouseX * rotationSpeed;
        }
        else
        {
            //when the player releases middle mouse, slightly smooth out
            rotationVelocity = Mathf.Lerp(
                rotationVelocity,
                0f,
                rotationSmoothness * Time.deltaTime
            );
        }

        //apply the rotation velocity every frame
        cameraYaw += rotationVelocity * Time.deltaTime;
    }

    private void HandleZoom()
    {
        //mouse scroll wheel returns positive when scrolling up/negative when down
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            //scroll up zooms in, scroll down zooms out
            targetZoom -= scroll * zoomSpeed;

            //prevent the player from zooming too far in or out
            targetZoom = Mathf.Clamp(
                targetZoom,
                minZoom,
                maxZoom
            );
        }
    }
}