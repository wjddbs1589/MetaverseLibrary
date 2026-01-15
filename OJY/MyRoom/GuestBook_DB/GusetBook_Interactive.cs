using Suncheon.Player;
using Suncheon.UI;
using UnityEngine;

namespace Suncheon.MyRoom
{
    public class GusetBook_Interactive : MonoBehaviour, Clickable
    {
        [Header("방명록 UI")]
        [SerializeField] GameObject GuestBookUI;

        [Header("포스트잇 게임 오브젝트")]
        [SerializeField] GameObject memo;

        bool newComm = false;


        void Awake()
        {
            Check_NewComm();
        }

        /// <summary>
        /// 방명록에 새글이 등록됬는지 확인하여 Bool값 설정
        /// </summary>
        void Check_NewComm()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.newComm}", 
            (jsonData) => 
            {
                UTILS.Log($"방명록 새글 확인여부:{jsonData}");

                newComm = bool.Parse(jsonData);

                if (newComm)
                {
                    memo.SetActive(true);
                }
            }
            ));
        }

        public void OnClick()
        {
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().IsGuest)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"Guest는 방명록 기능을 사용할 수 없습니다.");
                return;
            }

            UIInteractionManager.Instance.OpenPopUp(GuestBookUI);

            //내 서재일때 방명록 확인시 포스트잇 오브젝트 비 활성화
            if(NetworkManager.Instance.Check_MyRoom())
            {
                memo.SetActive(false);
            }
        }

        [SerializeField] string obj_Name;
        public string Return_ObjName()
        {
            return obj_Name;
        }
    }
    
}
