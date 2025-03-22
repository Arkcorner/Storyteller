using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("How fast this layer moves relative to the player (lower = further away)")]
    [SerializeField] private float parallaxEffectMultiplier = 0.5f;

    [Tooltip("If true, will wrap this layer when it moves off screen")]
    [SerializeField] private bool infiniteHorizontal = true;

    private float spriteWidth;
    private Transform cameraTransform;
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;
    private float startPositionX;

    private void Start()
    {
 
        cameraTransform = Camera.main.transform;

    
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Parallax Layer requires a player with tag 'Player'!");
            enabled = false;
            return;
        }

        lastPlayerPosition = playerTransform.position;
        startPositionX = transform.position.x;

        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            spriteWidth = spriteRenderer.bounds.size.x;
        }
        else
        {
            Debug.LogError("Parallax Layer requires a SpriteRenderer component!");
            enabled = false;
        }
    }

    private void LateUpdate()
    {
      
        Vector3 playerMovement = playerTransform.position - lastPlayerPosition;

      
        float moveAmount = playerMovement.x * parallaxEffectMultiplier;
        transform.position += new Vector3(-moveAmount, 0, 0);

        
        if (infiniteHorizontal)
        {
            float cameraRightEdge = cameraTransform.position.x + (Camera.main.orthographicSize * Camera.main.aspect);
            float cameraLeftEdge = cameraTransform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);

            float spriteLeftEdge = transform.position.x - spriteWidth / 2;
            float spriteRightEdge = transform.position.x + spriteWidth / 2;

         
            if (spriteRightEdge < cameraLeftEdge)
            {
                transform.position = new Vector3(transform.position.x + spriteWidth, transform.position.y, transform.position.z);
            }
           
            else if (spriteLeftEdge > cameraRightEdge)
            {
                transform.position = new Vector3(transform.position.x - spriteWidth, transform.position.y, transform.position.z);
            }
        }

    
        lastPlayerPosition = playerTransform.position;
    }
}