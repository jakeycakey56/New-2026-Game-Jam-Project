using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 CameraOffSet = new Vector3(5f, 7f, 5f);
    public Transform PlayerPOS;

    void Update()
    {
        transform.position = PlayerPOS.position + CameraOffSet;
    }
}
