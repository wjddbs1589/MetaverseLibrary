using UnityEngine;

/// <summary>
/// 웹뷰 팝업에 붙여, 팝업이 닫힐 때 웹뷰를 연 오브젝트(WebViewInteractable)에 알린다.
/// 팝업을 여러 오브젝트가 함께 쓸 수 있도록 Owner는 웹뷰를 연 오브젝트가 그때그때 지정한다.
/// </summary>
public class WebViewCloseNotifier : MonoBehaviour
{
    /// <summary>현재 이 팝업에 웹뷰를 연 오브젝트</summary>
    [HideInInspector] public WebViewInteractable Owner;

    /// <summary>팝업이 닫히면 웹뷰 정리를 요청한다.</summary>
    private void OnDisable()
    {
        if (Owner != null) Owner.Close_Web();
    }
}
