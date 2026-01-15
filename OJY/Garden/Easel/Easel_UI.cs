using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class Easel_UI : MonoBehaviour
{
    [SerializeField] Button btn_exit;
    [SerializeField] Image image;

    private void Awake()
    {
        btn_exit.onClick.AddListener(() => Btn_Exit());
    }

    public void Set_Material(Material picture)
    {
        image.material = picture;
    }

    public void Set_Sprite(Sprite sp)
    {
        image.sprite = sp;
    }
    void Btn_Exit()
    {
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }
}
