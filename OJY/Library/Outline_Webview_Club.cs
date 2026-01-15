using Suncheon.UI;
using Suncheon;
using Suncheon.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Outline_Webview_Club : MonoBehaviour, Clickable
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

    [SerializeField] bool waiting = false;

    CloseWebView_Club close_webview;

    void Awake()
    {
        btn_exit.onClick.AddListener(() => Btn_Exit());
        close_webview = wv_UI.GetComponent<CloseWebView_Club>();
    }

    void Btn_Exit()
    {
        UIInteractionManager.Instance.CloseLastOpenedPopUp();  //UI 닫기        
    }

    public void OnClick()
    {
        if (!UIInteractionManager.Instance.IsUIInteraction)
        {
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().isClub_Meeting)
            {
                if (!waiting)
                {
                    url = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().club_FileURL;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                waiting = true;
                UIInteractionManager.Instance.OpenPopUp(wv_UI);


                //웹뷰활성화
                wv = Instantiate(WebView_preafab, wv_UI.transform);
                wv.GetComponent<UI_WebView>().ShowWebView(url);

                close_webview.OW = this;
#else
                    Application.OpenURL(url);
#endif
                }
            }
            else
            {
                UIInteractionManager.Instance.ShowSystemPopUp("동아리장이 회의를 시작하지 않았습니다.");
            }
        }
    }

    public string Return_ObjName()
    {        
        return $"{NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().club_MeetingName}\n{obj_Name}";
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