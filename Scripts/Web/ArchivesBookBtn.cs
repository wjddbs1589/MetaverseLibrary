using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    /// <summary>
    /// 자료실 검색 메뉴. 전자책·데이터베이스·오디오북 중 하나를 고르면 해당 페이지를 연다.
    ///
    /// 플랫폼별 처리:
    ///   모바일 (Android/iOS) → 메뉴를 숨기고 웹뷰로 표시
    ///   웹 (WebGL)·에디터    → 브라우저 새 창으로 열기
    /// </summary>
    public class ArchivesBookBtn : MonoBehaviour
    {
        [Header("닫기 버튼")]
        [SerializeField] private Button btn_Exit;

        [Header("검색 메뉴")]
        [SerializeField] private GameObject SelectUI;

        [Header("웹뷰 (모바일)")]
        [SerializeField] private GameObject WebViewUI;
        [SerializeField] private GameObject WebView_Prefab;

        [Header("메뉴 버튼과 주소")]
        [SerializeField] private Button btn_elec;
        [SerializeField] private string url_e;
        [SerializeField] private Button btn_data;
        [SerializeField] private string url_d;
        [SerializeField] private Button btn_audi;
        [SerializeField] private string url_a;

        // 생성된 웹뷰
        private GameObject _webView;

        /// <summary>메뉴 버튼과 닫기 버튼을 연결한다.</summary>
        private void Awake()
        {
            btn_elec.onClick.AddListener(() => OpenPage(url_e, btn_elec));
            btn_data.onClick.AddListener(() => OpenPage(url_d, btn_data));
            btn_audi.onClick.AddListener(() => OpenPage(url_a, btn_audi));
            btn_Exit.onClick.AddListener(Btn_Exit);
        }

        /// <summary>열릴 때 검색 메뉴를 표시한다.</summary>
        private void OnEnable()
        {
            SelectUI.SetActive(true);
        }

        /// <summary>닫힐 때 웹뷰를 제거하고 닫기 버튼 이미지를 되돌린다.</summary>
        private void OnDisable()
        {
            if (_webView != null) Destroy(_webView);
            btn_Exit.GetComponent<ButtonControl>().Btnoff();
        }

        /// <summary>플랫폼에 맞는 방식으로 선택한 페이지를 연다.</summary>
        private void OpenPage(string url, Button pressed)
        {
            pressed.GetComponent<ButtonControl>().Btnoff();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            SelectUI.SetActive(false);
            _webView = Instantiate(WebView_Prefab, WebViewUI.transform);
            _webView.GetComponent<UI_WebView>().ShowWebView(url);
#else
            Application.OpenURL(url);
#endif
        }

        /// <summary>자료실 메뉴를 닫는다.</summary>
        public void Btn_Exit()
        {
            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }
    }
}
