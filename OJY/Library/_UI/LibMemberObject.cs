using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Suncheon.UI
{
    public class LibMemberObject : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Image image_Bg;
        [SerializeField] private TMP_Text text_NickName;

        UI_Interaction_Room room;

        public void SetObject(string nickName)
        {
            image_Bg.sprite = sprite;

            text_NickName.text = nickName;
        }

        /// <summary>
        /// 클릭했을때 닉네임 반환
        /// </summary>
        public void Return_Nicname()
        {
            room = FindAnyObjectByType<UI_Interaction_Room>();
        }

        /// <summary>
        /// 유저의 ID 정보를 가져옴
        /// </summary>
        /// <param name="playerName">ID를 가져올 유저의 닉네임</param>
        public virtual void Load_UserID(string playerName)
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userIdCheck}", $"nickname={playerName}",
               (jsonData) =>
               {
                   UTILS.Log($"SLoad_UserID : {jsonData}");

                   Set_UserID(jsonData);
               }
               ));
        }

        /// <summary>
        /// 가져온 userID값을 변수에 저장
        /// </summary>
        /// <param name="userID"></param>
        public virtual void Set_UserID(string userID)
        {
            NetworkManager.Instance.user_id = userID;
        }

        public virtual void Set_UserNo(string userNo)
        {
            NetworkManager.Instance.user_no = userNo;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {

            }
            else
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    string hitPlayerName = text_NickName.text.Split(':')[0].Trim();

                    if (hitPlayerName == PhotonNetwork.NickName) return;

                    Vector3 canvasPos = eventData.position;
                    NetworkManager.Instance.user_name = text_NickName.text;
                    Load_UserID(hitPlayerName);

                    UIInteractionManager_Room uiinter = (UIInteractionManager_Room)UIInteractionManager.Instance;
                    uiinter.LibPlayerInteractionOn(hitPlayerName, canvasPos);
                }
            }
        }
    }
}
