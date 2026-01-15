using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class Aud_Book : MonoBehaviour
{
    [SerializeField] GameObject aud_wv;
    public string url;

    Button btn;
    ButtonControl btnControl;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => Btn_Click());

        btnControl = GetComponent<ButtonControl>();
    }

    void Btn_Click()
    {
        //웹뷰 활성화
        aud_wv.SetActive(true);
        aud_wv.GetComponent<UI_WebView>().ShowWebView(url);

        btnControl.Btnoff();                          //버튼 이미지 변경
        transform.parent.gameObject.SetActive(false); //버튼 UI 끄기
    }
}
