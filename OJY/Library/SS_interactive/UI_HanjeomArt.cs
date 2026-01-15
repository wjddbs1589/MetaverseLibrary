using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_HanjeomArt : MonoBehaviour
{
    [SerializeField] Button btn_exit;
    [SerializeField] Image image;

    private void Awake()
    {
        btn_exit.onClick.AddListener(() => Btn_Exit());
    }

    public void Set_Image(Material picture)
    {
        image.material = picture;
    }

    void Btn_Exit()
    {
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }
}
