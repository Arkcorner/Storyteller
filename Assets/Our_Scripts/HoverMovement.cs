using UnityEngine;

public class HoverMovement : MonoBehaviour
{
    
    [SerializeField] private float amplitude = 1f;

  
    [SerializeField] private float speed = 1f;

    
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
     
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;

      
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}