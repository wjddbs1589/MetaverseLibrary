using Suncheon;
using UnityEngine;
using UnityEngine.UI;
using Suncheon.UI;

public class OpenWebURL : MonoBehaviour
{
    [SerializeField] GameObject webview;
    [SerializeField] UI_WebView UI_WebView;
    [SerializeField] string URL;

    Button btn;
    ButtonControl bc;
    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => Click());
        bc = GetComponent<ButtonControl>();
    }

    void Click()
    {
        bc.Btnoff();
        Set_WebView();        
    }

    public void Set_WebView()
    {
        webview.SetActive(true);
        UI_WebView.ShowWebView(URL);
        transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
