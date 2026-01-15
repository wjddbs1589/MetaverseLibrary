using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class ArchivesBookBtn : MonoBehaviour
    {
        [Header("닫기 버튼")]
        [SerializeField] Button btn_Exit;

        [Header("검색창 UI")]
        [SerializeField] GameObject SelectUI;

        [Header("웹뷰")]
        [SerializeField] GameObject WebViewUI;
        [SerializeField] GameObject WebView_Prefab;

        [Header("버튼")]
        [SerializeField] Button btn_elec;
        [SerializeField] Button btn_data;
        [SerializeField] Button btn_audi;

        GameObject wv;

        [Header("URL")]
        [SerializeField] string url_e;
        [SerializeField] string url_d;
        [SerializeField] string url_a;
        
        private void OnEnable()
        {
            SelectUI.SetActive(true);   //활성시 버튼 켜기  
        }

        private void OnDisable()
        {
            if(wv != null){ Destroy(wv); }            

            //닫기 버튼 이미지 비활성화
            ButtonControl bc = btn_Exit.GetComponent<ButtonControl>();
            bc.Btnoff();     
        }

        private void Awake()
        {
            btn_elec.onClick.AddListener(() => Btn_Elec());
            btn_data.onClick.AddListener(() => Btn_Data());
            btn_audi.onClick.AddListener(() => Btn_Audi());
            btn_Exit.onClick.AddListener(() => Btn_Exit());
        }
        void Btn_Elec()
        {
            Set_WV(url_e, btn_elec);
        }

        void Btn_Data()
        {
            Set_WV(url_d, btn_data);
        }

        void Btn_Audi()
        {
            Set_WV(url_a, btn_audi);
        }
        void Set_WV(string url, Button btn)
        {
            //UI 버튼 이미지 변경
            ButtonControl BC = btn.GetComponent<ButtonControl>();
            BC.Btnoff();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            //버튼 비활성화
            SelectUI.SetActive(false);
            //웹뷰활성화
            wv = Instantiate(WebView_Prefab, WebViewUI.transform);
            wv.GetComponent<UI_WebView>().ShowWebView(url);
#else
            Application.OpenURL(url);
#endif
        }

        public void Btn_Exit()
        {
            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }
    }
}