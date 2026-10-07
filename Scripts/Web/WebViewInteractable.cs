using Suncheon;
using Suncheon.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 클릭하면 웹 페이지를 여는 오브젝트의 공통 클래스.
/// 도서관별 안내 페이지, 자료 검색, 배너 등 웹 연결 오브젝트가 이 클래스를 그대로 쓰거나 상속한다.
///
/// 플랫폼별 처리:
///   모바일 (Android/iOS) → 팝업 안에 웹뷰를 생성해 앱 안에서 표시
///   웹 (WebGL)·에디터    → 브라우저 새 창으로 열기
///
/// 확장:
///   CanOpen()        → 열기 전 조건 확인 (예: 동아리 회의 진행 여부)
///   GetUrl()         → 열 주소 결정 (예: 서버에서 받은 회의 자료 주소)
///   Return_ObjName() → 표시 이름 변경
/// </summary>
public class WebViewInteractable : MonoBehaviour, IClickable
{
    [Tooltip("마우스를 올렸을 때 표시할 이름")]
    [SerializeField] private string obj_Name;

    [Tooltip("열 웹 페이지 주소")]
    [SerializeField] protected string url;

    [Header("웹뷰 (모바일)")]
    [Tooltip("웹뷰를 띄울 팝업 UI")]
    [SerializeField] private GameObject webViewPopup;

    [Tooltip("웹뷰 프리팹 (UI_WebView 포함)")]
    [SerializeField] private GameObject webViewPrefab;

    [Tooltip("팝업 닫기 버튼")]
    [SerializeField] private Button btn_exit;

    [Tooltip("웹뷰를 닫은 뒤 다시 열 수 있기까지의 시간 (초). 닫기와 동시에 오브젝트가 다시 클릭되는 것을 막는다.")]
    [SerializeField] private float reopenDelay = 0.5f;

    // 생성된 웹뷰
    private GameObject _webView;

    // 웹뷰가 열려 있거나 닫힌 직후면 true
    private bool _waiting = false;

    /// <summary>닫기 버튼을 연결한다.</summary>
    protected virtual void Awake()
    {
        if (btn_exit != null)
            btn_exit.onClick.AddListener(() => UIInteractionManager.Instance.ClosePopUp(webViewPopup));
    }

    /// <summary>다른 UI를 사용 중이거나 웹뷰가 이미 열려 있으면 무시하고, 열 수 있으면 페이지를 연다.</summary>
    public virtual void OnClick()
    {
        if (UIInteractionManager.Instance.IsUIInteraction || _waiting) return;
        if (!CanOpen()) return;

        OpenPage(GetUrl());
    }

    /// <summary>마우스를 올렸을 때 표시할 이름을 반환한다.</summary>
    public virtual string Return_ObjName() => obj_Name;

    /// <summary>페이지를 열기 전 조건을 확인한다. 열 수 없으면 false를 반환한다.</summary>
    protected virtual bool CanOpen() => true;

    /// <summary>열 페이지 주소를 반환한다.</summary>
    protected virtual string GetUrl() => url;

    /// <summary>플랫폼에 맞는 방식으로 페이지를 연다.</summary>
    private void OpenPage(string targetUrl)
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        _waiting = true;
        UIInteractionManager.Instance.OpenPopUp(webViewPopup);

        _webView = Instantiate(webViewPrefab, webViewPopup.transform);
        _webView.GetComponent<UI_WebView>().ShowWebView(targetUrl);

        // 여러 오브젝트가 같은 팝업을 쓸 수 있으므로, 팝업이 닫힐 때 알릴 대상을 지금 연 오브젝트로 지정한다
        if (webViewPopup.TryGetComponent(out WebViewCloseNotifier notifier))
            notifier.Owner = this;
#else
        Application.OpenURL(targetUrl);
#endif
    }

    /// <summary>팝업이 닫히면 WebViewCloseNotifier에서 호출한다. 웹뷰를 제거하고 잠시 뒤 다시 열 수 있게 한다.</summary>
    public void Close_Web()
    {
        if (_webView != null) Destroy(_webView);
        if (btn_exit != null) btn_exit.GetComponent<ButtonControl>().Btnoff();

        StartCoroutine(ReleaseAfterDelay());
    }

    /// <summary>reopenDelay가 지나면 다시 열 수 있게 한다.</summary>
    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(reopenDelay);
        _waiting = false;
    }
}
