using UnityEngine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    public Vector3 offset = new Vector3(0, 2, -10);
    public float smoothSpeed = 5f;

    private Transform target;

    void Start()
    {
        
        if (IsOwner)
        {
            target = transform;
        }
    }

    void LateUpdate()
    {
        if (!IsOwner || target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            Camera.main.transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        Camera.main.transform.position = smoothedPosition;
    }
}