using UnityEngine;

public class CameraController_01 : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    public Transform target;
    private Vector3 vel = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}
