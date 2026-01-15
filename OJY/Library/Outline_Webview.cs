using Suncheon;
using Suncheon.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Outline_Webview : MonoBehaviour, Clickable
{
    [Header("웹뷰 UI")]
    [SerializeField] GameObject wv_UI;

    [Header("웹뷰 프리펩")]
    [SerializeField] GameObject WebView_preafab;

    [Header("오브젝트 이름")]
    [SerializeField] string obj_Name;

    [Header("URL 정보")]
    public string url;

    [Header("닫기 버튼")]
    [SerializeField] Button btn_exit;
    [HideInInspector] public GameObject wv;

    bool waiting = false;

    void Awake()
    {
        btn_exit.onClick.AddListener(() => Btn_Exit());
    }

    void Btn_Exit()
    {
        //UIInteractionManager.Instance.ClosePopUp(wv_UI);  //UI 닫기
        UIInteractionManager.Instance.CloseLastOpenedPopUp();
    }

    public void OnClick()
    {
        if (!UIInteractionManager.Instance.IsUIInteraction)
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        if (!waiting)
        {
            waiting = true;
            UIInteractionManager.Instance.OpenPopUp(wv_UI);

            //웹뷰활성화
            wv = Instantiate(WebView_preafab, wv_UI.transform);
            wv.GetComponent<UI_WebView>().ShowWebView(url);            
        }
#else
            Application.OpenURL(url);
#endif
        }
    }

    public string Return_ObjName()
    {
        return obj_Name;
    }

    public void Close_Web()
    {
        if (wv != null) { Destroy(wv); }  //웹뷰 삭제
        btn_exit.GetComponent<ButtonControl>().Btnoff(); //버튼 이미지 변경

        Click_Delay();
    }

    void Click_Delay()
    {
        StartCoroutine(Delay_Co());
    }

    IEnumerator Delay_Co()
    {
        yield return new WaitForSeconds(.25f);
        waiting = false;
    }
}
