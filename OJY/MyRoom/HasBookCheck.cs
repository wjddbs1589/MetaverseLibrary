using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class HasBookCheck : MonoBehaviour, Clickable
{
    public string Url = "https://library.suncheon.go.kr/lib/mypage/book/request/loanIndex.do?menuCd=L006001001";
    
    [Header("대출현황 실행확인 UI")]
    [SerializeField] GameObject HasBook_PopUp;
    [SerializeField] Button btn_yes;
    [SerializeField] Button btn_no;

    [Header("대출현황 웹뷰")]
    [SerializeField] GameObject HasBook_WebView;
    UI_WebView webView;

    [SerializeField] string obj_Name;

    [SerializeField] FurnitureMove furnitureMove;
    private void Awake()
    {
        btn_yes.onClick.AddListener(() => BookCheck_Yes());
        btn_no.onClick.AddListener(() => BookCheck_No());
        webView = HasBook_WebView.GetComponentInChildren<UI_WebView>(); 
    }

    public void OnClick()
    {
        if (NetworkManager.Instance.Check_MyRoom())
        {
            UIInteractionManager.Instance.OpenPopUp(HasBook_PopUp);
        }
        else
        {
            UIInteractionManager.Instance.ShowSystemPopUp("대출현황 조회는 나의 서재에서만 사용 가능합니다.");
        }
    }
    public string Return_ObjName()
    {
        return obj_Name;
    }

    void BookCheck_Yes()
    {
        Clicked_Btn_Reset(btn_yes);
        UIInteractionManager.Instance.ClosePopUp(HasBook_PopUp);
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        UIInteractionManager.Instance.OpenPopUp(HasBook_WebView);
        webView.ShowWebView(Url);
#else
        Application.OpenURL(Url);
        furnitureMove.UsingUI = false;
#endif

    }

    void BookCheck_No()
    {        
        Clicked_Btn_Reset(btn_no);
        UIInteractionManager.Instance.ClosePopUp(HasBook_PopUp);
        InteractionManager.Inst.Ray_On();
        furnitureMove.UsingUI = false;
    }

    void Clicked_Btn_Reset(Button btn)
    {
        ButtonControl buttonControl = btn.gameObject.GetComponent<ButtonControl>();

        buttonControl.Btnoff();        
    }
}
