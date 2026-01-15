using UnityEngine;
using UnityEngine.UI;

public class FurnitureBtnImage : MonoBehaviour
{
    [SerializeField] Sprite On_Image;
    [SerializeField] Sprite Off_Image;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Set_OnImage()
    {
        Set_Image(true);        
    }

    public void Set_OffImage() 
    {
        Set_Image(false);        
    }

    void Set_Image(bool IsOn)
    {
        image = GetComponent<Image>();
        image.sprite = IsOn ? On_Image : Off_Image;
    }
}
