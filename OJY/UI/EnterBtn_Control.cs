using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnterBtn_Control : MonoBehaviour
{
    [SerializeField] TMP_InputField input_ChatBox;
    Image btn_Image;
    Color btn_Color;

    private void Awake()
    {
        input_ChatBox.onSelect.AddListener((str) => OnSelect());
        input_ChatBox.onDeselect.AddListener((str) => DeSelect());

        btn_Image = GetComponent<Image>();
        btn_Color = btn_Image.color;
    }

    public void DeSelect()
    {        
        btn_Color.a = 0.5f;
        btn_Image.color = btn_Color;
    }

    public void OnSelect()
    {
        btn_Color.a = 1f;
        btn_Image.color = btn_Color;
    }
}
