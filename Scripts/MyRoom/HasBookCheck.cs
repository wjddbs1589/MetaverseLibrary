using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개인서재의 대출현황 조회 오브젝트.
/// 내 서재에서만 사용할 수 있으며, 확인 팝업 후 순천시립도서관 대출현황 페이지를 연다.
///
/// 플랫폼별 처리:
///   모바일 (Android/iOS) → 앱 안의 웹뷰로 표시
///   웹 (WebGL)·에디터    → 브라우저 새 창으로 열기
/// </summary>
public class HasBookCheck : MonoBehaviour, IClickable
{
    [Tooltip("대출현황 페이지 주소")]
    public string Url = "https://library.suncheon.go.kr/lib/mypage/book/request/loanIndex.do?menuCd=L006001001";

    [Tooltip("마우스를 올렸을 때 표시할 이름")]
    [SerializeField] private string obj_Name;

    [Header("대출현황 조회 확인 팝업")]
    [SerializeField] private GameObject HasBook_PopUp;
    [SerializeField] private Button btn_yes;
    [SerializeField] private Button btn_no;

    [Header("대출현황 웹뷰")]
    [SerializeField] private GameObject HasBook_WebView;

    [Tooltip("서재 오브젝트 상호작용 처리 (팝업을 닫을 때 상호작용을 다시 허용)")]
    [SerializeField] private FurnitureMove furnitureMove;

    private UI_WebView _webView;

    /// <summary>버튼 이벤트를 연결하고 웹뷰를 캐싱한다.</summary>
    private void Awake()
    {
        btn_yes.onClick.AddListener(BookCheck_Yes);
        btn_no.onClick.AddListener(BookCheck_No);
        _webView = HasBook_WebView.GetComponentInChildren<UI_WebView>();
    }

    /// <summary>내 서재면 확인 팝업을 열고, 다른 사람의 서재면 안내 메시지를 띄운다.</summary>
    public void OnClick()
    {
        if (NetworkManager.Instance.Check_MyRoom())
            UIInteractionManager.Instance.OpenPopUp(HasBook_PopUp);
        else
            UIInteractionManager.Instance.ShowSystemPopUp("대출현황 조회는 나의 서재에서만 사용 가능합니다.");
    }

    /// <summary>마우스를 올렸을 때 표시할 이름을 반환한다.</summary>
    public string Return_ObjName() => obj_Name;

    /// <summary>확인을 누르면 플랫폼에 맞는 방식으로 대출현황 페이지를 연다.</summary>
    private void BookCheck_Yes()
    {
        btn_yes.GetComponent<ButtonControl>().Btnoff();
        UIInteractionManager.Instance.ClosePopUp(HasBook_PopUp);

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        UIInteractionManager.Instance.OpenPopUp(HasBook_WebView);
        _webView.ShowWebView(Url);
#else
        Application.OpenURL(Url);
        furnitureMove.UsingUI = false;
#endif
    }

    /// <summary>취소를 누르면 팝업을 닫고 오브젝트 상호작용을 다시 허용한다.</summary>
    private void BookCheck_No()
    {
        btn_no.GetComponent<ButtonControl>().Btnoff();
        UIInteractionManager.Instance.ClosePopUp(HasBook_PopUp);
        InteractionManager.Inst.Ray_On();
        furnitureMove.UsingUI = false;
    }
}
