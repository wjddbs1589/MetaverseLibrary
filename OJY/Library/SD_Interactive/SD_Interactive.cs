using Suncheon.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class SD_Interactive : MonoBehaviour, Clickable
    {
        [Header("신대도서관 UI")]
        [SerializeField] GameObject SD_UI;

        [Header("웹뷰")]
        [SerializeField] GameObject WebView_preafab;

        [Header("오브젝트 이름")]
        [SerializeField] string obj_Name;

        [Header("URL 정보")]
        public string url = "https://library.suncheon.go.kr/lib/book/character/characterIndex.do?menuCd=L001001005&currentPageNo=1&nPageSize=10&searchShelf=A13&subjectCode=&searchType=&search=";

        [Header("닫기 버튼")]
        [SerializeField] Button btn_exit;
        GameObject wv;

        private void OnDisable()
        {
            Close_Web();
        }

        void Awake()
        {
            btn_exit.onClick.AddListener(() => Btn_Exit());
        }

        void Btn_Exit()
        {
            UIInteractionManager.Instance.ClosePopUp(SD_UI);
        }

        public void OnClick()
        {
            if (!waiting)
            {
                UIInteractionManager.Instance.OpenPopUp(SD_UI);

                //웹뷰활성화
                wv = Instantiate(WebView_preafab, SD_UI.transform);
                wv.GetComponent<UI_WebView>().ShowWebView(url);
            }
        }

        public string Return_ObjName()
        {
            return obj_Name;
        }

        public void Close_Web()
        {
            if (wv != null) { Destroy(wv); }
            Click_Delay();
            btn_exit.GetComponent<ButtonControl>().Btnoff();
        }

        #region 클릭 딜레이 설정
        bool waiting = false;
        void Click_Delay()
        {
            StartCoroutine(Delay_Co());
        }

        IEnumerator Delay_Co()
        {
            waiting = true;
            yield return new WaitForSeconds(.5f);
            waiting = false;
        }
        #endregion
    }
}