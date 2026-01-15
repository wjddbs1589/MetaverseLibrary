using Suncheon;
using Suncheon.Player;
using Suncheon.UI;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuestBook_WriteComm : MonoBehaviour
{
    [SerializeField] Button btn_typing;

    TMP_InputField comm;

    GuestBook_CommList commList;

    private void Awake()
    {
        comm = GetComponent<TMP_InputField>();
        commList = FindAnyObjectByType<GuestBook_CommList>();
        btn_typing.onClick.AddListener(() => Btn_typing());
    }

    /// <summary>
    /// 방명록에 글을 등록했을때 서버에 저장
    /// </summary>
    public void Btn_typing()
    {
        Request_WriteComm request_WriteComm = new Request_WriteComm(NetworkManager.Instance.user_id, comm.text);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.writeComm}",
            request_WriteComm, (jsonData) =>
            {
                UTILS.Log($"Btn_typing : {jsonData}");
                Response_ReturnMsg response_WriteComm = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                UTILS.Log(NetworkManager.Instance.user_id);
                UTILS.Log(comm.text);

                if (response_WriteComm.rtnCode == "000")
                {
                    comm.text = "";
                    commList.Open_GuestBook();

                    NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.commBoardCnt += 1;

                    //#region 방명록 글쓰기 미션 정보 조회
                    //Response_MissionInfo missionInfo = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().missionInfo;
                    //if (missionInfo.mission5 != 1)
                    //{
                    //    Debug.Log($"Btn_typing : {jsonData}");
                    //    Request_MissionUpdate request_MissionUpdate = new Request_MissionUpdate(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().loginData.user_id, missionInfo.mission1, missionInfo.mission2, missionInfo.mission3, missionInfo.mission4, 1);
                    //    StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.missionInfoUpdate}", request_MissionUpdate, (jsonData) =>
                    //    {
                    //        Response_MissionUpdate response_MissionUpdate = JsonUtility.FromJson<Response_MissionUpdate>(jsonData);
                    //        if (response_MissionUpdate == null) return;

                    //        if (response_MissionUpdate.rtnCode == "000")
                    //        {
                    //            UTILS.Log("방명록 입력 스탬프");
                    //        }
                    //        else
                    //        {
                    //            UTILS.Log($"{response_MissionUpdate.rtnMsg}");
                    //        }
                    //    }));
                    //}
                    //#endregion
                }
                else if (response_WriteComm.rtnCode == "999")
                {
                    UIInteractionManager.Instance.ShowSystemPopUp("부적절한 단어가 포함되어 있습니다.");
                }
            }
            ));
    }
}
