using Suncheon.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class KR_interactive : MonoBehaviour, Clickable
    {
        [Header("어린이실 UI")]
        [SerializeField] GameObject KR_UI;

        [Header("웹뷰")]
        [SerializeField] GameObject WebView_UI;

        [Header("오브젝트 이름")]
        [SerializeField] string obj_Name;

        [Header("URL 정보")]
        public string URL;

        [Header("닫기 버튼")]
        [SerializeField] Button btn_exit;

        void Awake()
        {
            btn_exit.onClick.AddListener(() => Btn_Exit());
        }

        public string Return_ObjName()
        {
            return obj_Name;
        }

        public void OnClick()
        {
            if (!waiting)
            {
                UIInteractionManager.Instance.OpenPopUp(KR_UI);
                WebView_UI.SetActive(true);
                WebView_UI.GetComponent<UI_WebView>().ShowWebView(URL);
            }
        }

        void Btn_Exit()
        {
            Close_Web();
            UIInteractionManager.Instance.ClosePopUp(KR_UI);
        }
        public void Close_Web()
        {
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