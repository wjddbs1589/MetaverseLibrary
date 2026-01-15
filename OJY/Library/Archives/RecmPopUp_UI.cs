using UnityEngine;
using UnityEngine.UI;

public class RecmPopUp_UI : MonoBehaviour
{
    [Header("¹öÆ°")]
    [SerializeField] Button btn_Yes;
    [SerializeField] Button btn_No;
    [SerializeField] Recommend_UI recommend;


    private void Awake()
    {
        btn_Yes.onClick.AddListener(() => Btn_Y());
        btn_No.onClick.AddListener(() => Btn_N());
    }

    void Btn_Y()
    {
        recommend.Cancel();
        gameObject.SetActive(false);
    }

    void Btn_N()
    {
        gameObject.SetActive(false);
    }
}
