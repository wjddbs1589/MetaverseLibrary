using UnityEngine;
using UnityEngine.UI;
using Suncheon.UI;

public class ArchivesPcBtn : MonoBehaviour
{
    [Header("À¥ºä UI")]
    [SerializeField] GameObject WebViewUI;

    [Header("´Ý±â ¹öÆ°")]
    [SerializeField] Button btn_exit;

    private void OnEnable()
    {
        //WebViewUI.SetActive(true);
    }

    private void Awake()
    {
        btn_exit.onClick.AddListener(() => Btn_Exit());
    }

    void Btn_Exit()
    {
        //InteractionManager.Inst.Ray_On();
        //WebViewUI.SetActive(false);
        //this.gameObject.SetActive(false);

        UIInteractionManager.Instance.ClosePopUp(WebViewUI);
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }
}
