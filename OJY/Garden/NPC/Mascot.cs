using Suncheon.UI;
using Suncheon;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mascot : MonoBehaviour, Clickable
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ui_TalkBox;
    [SerializeField] private Animator anim;
    private static readonly int _Idle = Animator.StringToHash("Idle");

    [SerializeField] private GameObject ui_NpcChat;

    //[Range(0.0f, 300.0f)]
    //[SerializeField] private float maxDistance;
    float maxDistance = 15.0f;

    [SerializeField] private bool isNpcChat;
    [SerializeField] private string url = $"https://library.suncheon.go.kr/board/lib_board_note/boardList.do?menuCd=L011002";


    [Header("¿•∫‰ UI")]
    [SerializeField] GameObject wv_UI;

    [Header("¿•∫‰ «¡∏Æ∆È")]
    [SerializeField] GameObject WebView_preafab;
    [HideInInspector] public GameObject wv;

    [Header("¥›±‚ πˆ∆∞")]
    [SerializeField] Button btn_exit;
    [SerializeField] bool waiting = false;

    private void Start()
    {
        anim.SetTrigger(_Idle);
    }

    void Awake()
    {
        btn_exit.onClick.AddListener(() => Btn_Exit());
    }

    void Btn_Exit()
    {
        UIInteractionManager.Instance.CloseLastOpenedPopUp();  //UI ¥›±‚        
    }

    private void Update()
    {
        if (NetworkManager.Instance == null) return;

        if (player == null)
        {
            player = NetworkManager.Instance.Go_Player;
        }

        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > maxDistance)
            {
                ui_TalkBox.SetActive(false);
            }
            else
            {
                ui_TalkBox.SetActive(true);
                Vector3 direction = player.transform.position - transform.position;
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;
            }
        }
    }

    public void OnClick()
    {
        if (isNpcChat)
        {
            UIInteractionManager.Instance.OpenPopUp(ui_NpcChat);
        }
        else
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            if (!waiting)
            {
                waiting = true;
                UIInteractionManager.Instance.OpenPopUp(wv_UI);

                //¿•∫‰»∞º∫»≠
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
        return "";
    }

    public void Close_Web()
    {
        if (wv != null) { Destroy(wv); }  //¿•∫‰ ªË¡¶
        btn_exit.GetComponent<ButtonControl>().Btnoff(); //πˆ∆∞ ¿ÃπÃ¡ˆ ∫Ø∞Ê

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
