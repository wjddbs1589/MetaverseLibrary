using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Suncheon.UI
{
    /// <summary>
    /// 방문자 목록의 항목 하나.
    /// 우클릭하면 해당 유저의 ID를 서버에서 조회하고, 내보내기 등 서재 주인용 상호작용 메뉴를 연다.
    /// </summary>
    public class LibMemberObject : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Image image_Bg;
        [SerializeField] private TMP_Text text_NickName;

        /// <summary>항목에 닉네임을 표시한다.</summary>
        public void SetObject(string nickName)
        {
            image_Bg.sprite = sprite;
            text_NickName.text = nickName;
        }

        /// <summary>닉네임으로 유저 ID를 조회한다.</summary>
        public virtual void Load_UserID(string playerName)
        {
            string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userIdCheck}";
            StartCoroutine(UTILS.Requset_HttpGetData(url, $"nickname={playerName}", Set_UserID));
        }

        /// <summary>조회한 유저 ID를 상호작용 대상 정보로 저장한다.</summary>
        public virtual void Set_UserID(string userID)
        {
            NetworkManager.Instance.user_id = userID;
        }

        /// <summary>유저 번호를 상호작용 대상 정보로 저장한다.</summary>
        public virtual void Set_UserNo(string userNo)
        {
            NetworkManager.Instance.user_no = userNo;
        }

        /// <summary>우클릭하면 나 자신이 아닌 경우 대상 정보를 저장하고 상호작용 메뉴를 연다.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;

            string targetName = text_NickName.text.Split(':')[0].Trim();
            if (targetName == PhotonNetwork.NickName) return;

            NetworkManager.Instance.user_name = text_NickName.text;
            Load_UserID(targetName);

            UIInteractionManager_Room roomUI = (UIInteractionManager_Room)UIInteractionManager.Instance;
            roomUI.LibPlayerInteractionOn(targetName, eventData.position);
        }
    }
}
