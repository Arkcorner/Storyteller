using UnityEngine;

public class CameraController_01 : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    public Transform target;

    
    private float xVelocity;

    private void LateUpdate()
    {
        
        float targetX = target.position.x + offset.x;

        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref xVelocity, damping);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}