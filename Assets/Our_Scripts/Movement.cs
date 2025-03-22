using UnityEngine;
using Whisper.Samples;

public class PlayerMovement : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveSpeed = 5f;

    private bool isMoving = false;
    private float distanceMoved = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving && StreamingSampleMic.CanAdvance)
        {
            isMoving = true;
            distanceMoved = 0f;
            StreamingSampleMic.CanAdvance = false;
        }

        if (isMoving)
        {
            float movement = moveSpeed * Time.deltaTime;
            transform.position += new Vector3(movement, 0, 0);
            distanceMoved += movement;

            if (distanceMoved >= moveDistance)
            {
                isMoving = false;
            }
        }
    }
}