
using UnityEngine;
using UnityEngine.UI;

public class SpriteToggler : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite alternateSprite;

    private Button button;
    private Image buttonImage;
    private bool isAlternate = false;

    private void Start()
    {
        button = GetComponent <Button>();
        buttonImage = GetComponent<Image>();

     
        if (buttonImage.sprite == null && defaultSprite != null)
            buttonImage.sprite = defaultSprite;

     
        button.onClick.AddListener(ToggleSprite);
    }

    private void ToggleSprite()
    {
        if (isAlternate)
        {
            buttonImage.sprite = defaultSprite;
            isAlternate = false;
        }
        else
        {
            buttonImage.sprite = alternateSprite;
            isAlternate = true;
        }
    }
}
