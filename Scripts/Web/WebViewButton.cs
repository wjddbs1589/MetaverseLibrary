using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 자료실 메뉴의 웹 페이지 버튼 (전자책, 데이터베이스, 오디오북 등).
/// 누르면 지정한 웹뷰에 페이지를 띄우고, 버튼 메뉴는 닫는다.
/// </summary>
[RequireComponent(typeof(Button))]
public class WebViewButton : MonoBehaviour
{
    [Tooltip("페이지를 표시할 웹뷰 오브젝트 (UI_WebView 포함)")]
    [SerializeField] private GameObject webView;

    [Tooltip("열 웹 페이지 주소")]
    [SerializeField] private string url;

    private ButtonControl _buttonControl;

    /// <summary>클릭 이벤트를 연결한다.</summary>
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenPage);
        _buttonControl = GetComponent<ButtonControl>();
    }

    /// <summary>웹뷰를 켜서 페이지를 표시하고, 버튼 이미지를 되돌린 뒤 버튼 메뉴를 닫는다.</summary>
    private void OpenPage()
    {
        webView.SetActive(true);
        webView.GetComponent<UI_WebView>().ShowWebView(url);

        _buttonControl.Btnoff();
        transform.parent.gameObject.SetActive(false);
    }
}
