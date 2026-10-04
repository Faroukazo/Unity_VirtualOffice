using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSwitcher : MonoBehaviour
{
    public Button button;
    [Header("Sprite normal")]
    public Sprite normalSprite;
    [Header("Sprite muteado")]
    public Sprite pressedSprite;

    private Image buttonImage;
    private bool isPressed = false;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(ToggleButtonImage);
    }

    void ToggleButtonImage()
    {
        if (isPressed)
        {
            buttonImage.sprite = normalSprite;
        }
        else
        {
            buttonImage.sprite = pressedSprite;
        }
        isPressed = !isPressed;
    }
}
