using Suncheon.Player;
using Suncheon.UI;
using UnityEngine;

namespace Suncheon.MyRoom
{
    /// <summary>
    /// 개인서재의 방명록 오브젝트.
    /// 서재에 들어오면 읽지 않은 새 글이 있는지 확인해 포스트잇을 붙이고, 클릭하면 방명록을 연다.
    /// 게스트 계정은 사용할 수 없다.
    /// </summary>
    public class GuestBook_Interactive : MonoBehaviour, IClickable
    {
        [Header("방명록 UI")]
        [SerializeField] private GameObject GuestBookUI;

        [Header("새 글 알림 포스트잇")]
        [SerializeField] private GameObject memo;

        [Tooltip("마우스를 올렸을 때 표시할 이름")]
        [SerializeField] private string obj_Name;

        /// <summary>서재 입장 시 새 글 여부를 확인한다.</summary>
        private void Awake()
        {
            Check_NewComm();
        }

        /// <summary>읽지 않은 새 글이 있으면 포스트잇을 표시한다.</summary>
        private void Check_NewComm()
        {
            string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.newComm}";
            StartCoroutine(UTILS.Requset_HttpGetData(url, jsonData =>
            {
                if (bool.TryParse(jsonData, out bool hasNewComm) && hasNewComm)
                    memo.SetActive(true);
            }));
        }

        /// <summary>방명록을 연다. 내 서재면 새 글을 확인한 것으로 보고 포스트잇을 뗀다.</summary>
        public void OnClick()
        {
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().IsGuest)
            {
                UIInteractionManager.Instance.ShowSystemPopUp("Guest는 방명록 기능을 사용할 수 없습니다.");
                return;
            }

            UIInteractionManager.Instance.OpenPopUp(GuestBookUI);

            if (NetworkManager.Instance.Check_MyRoom())
                memo.SetActive(false);
        }

        /// <summary>마우스를 올렸을 때 표시할 이름을 반환한다.</summary>
        public string Return_ObjName() => obj_Name;
    }
}
