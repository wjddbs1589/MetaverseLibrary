using UnityEngine;
using UnityEngine.UI;
using Suncheon;
using Suncheon.WebData;
using TMPro;
using Suncheon.UI;

public class GuestBook_TalkUI : MonoBehaviour
{
    [SerializeField] Button btn_Delete;
    [SerializeField] Button btn_Report;

    [SerializeField] TMP_Text text_Name;
    [SerializeField] TMP_Text text_Comm;
    int seq;

    UIInteractionManager_Room uIInteractionManager_Room;
    private void Awake()
    {
        uIInteractionManager_Room = FindAnyObjectByType<UIInteractionManager_Room>();

        //버튼 등록
        btn_Report.onClick.AddListener(() => Btn_Report());
        btn_Delete.onClick.AddListener(() => Btn_Delete());

        if (!NetworkManager.Instance.Check_MyRoom())
        {
            //내 방이 아니면 삭제 비 활성화
            btn_Delete.gameObject.SetActive(false);
        }
        else
        {
            //내 방이면 신고 비 활성화
            btn_Report.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 가져온 방명록 코멘트 정보를 UI에 대입
    /// </summary>
    /// <param name="_resultData"></param>
    public void InitGuestBook(Response_CommListResultData _resultData)
    {
        //내가 쓴 글이면 신고버튼 비활성화
        if (_resultData.fromId == GameManager.Instance.loginData.user_id)
        {
            btn_Report.gameObject.SetActive(false);
        }

        text_Name.text = _resultData.fromNickname; //작성자 이름 등록
        seq = _resultData.commSeq;                 //seq값 가져옴

        //내 방이면 게시글 확인여부 변경
        if (NetworkManager.Instance.Check_MyRoom())
        {
            Read_Comm(seq);
        }

        //글이 신고된 글이면 블라인더 처리
        if (_resultData.report == "1")
        {
            Set_Report();
            return;
        }
        else 
        {
            text_Comm.text = _resultData.commCn; //아니면 방명록 내용 저장
        } 
    }

    /// <summary>
    /// 글 확인여부 변경
    /// </summary>
    /// <param name="seq"></param>
    void Read_Comm(int seq)
    {     
        Request_ReadComm request_ReadComm = new Request_ReadComm(seq);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.ReadnewComm}",
            request_ReadComm ,(JsonData) => 
            {
                UTILS.Log(JsonData);
                Response_ReturnMsg readComm = new Response_ReturnMsg();
                readComm = JsonUtility.FromJson<Response_ReturnMsg>(JsonData);
            }
            ));
    }

    /// <summary>
    /// 신고 버튼을 눌렀을 때
    /// </summary>
    void Btn_Report()
    {
        Report_Comm();
    }

    /// <summary>
    /// 신고기능
    /// </summary>
    void Report_Comm()
    {
        Set_Report();
        Request_Report report_Text = new Request_Report("C", seq);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.report}",
            report_Text, (jsonData) =>
            {
                Response_ReturnMsg result = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                UTILS.Log(result.rtnMsg);
                UTILS.Log(result.rtnCode);                
            }
            ));
    }

    /// <summary>
    /// 신고 되었을 때 조치
    /// </summary>
    void Set_Report()
    {
        btn_Report.gameObject.SetActive(false); //신고버튼 비 활성화
        text_Comm.text = "--- 해당 글은 신고가 접수되어 블라인드 처리되었습니다 ---"; //제목 변경
    }

    /// <summary>
    /// 삭제 버튼을 눌렀을 때 실행
    /// </summary>
    void Btn_Delete()
    {
        uIInteractionManager_Room.Comm_DeletUI.SetActive(true);
        uIInteractionManager_Room.btn_Delete_Ok.onClick.AddListener(() => Delete_Y());
        uIInteractionManager_Room.btn_Delete_Cancle.onClick.AddListener(() => Delete_N());
    }

    /// <summary>
    /// 방명록 삭제시 실행
    /// </summary>
    void Delete_Y()
    {
        Del_Comm();
        uIInteractionManager_Room.Comm_DeletUI.SetActive(false);
        //Destroy(this.gameObject);
    }

    /// <summary>
    /// 방명록 데이터를 지우고 이 UI도 지움
    /// </summary>
    void Del_Comm()
    {
        UTILS.Log("글 삭제");
        Request_Seq request_Seq = new Request_Seq(seq);

        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.DelComm}", request_Seq,
            (jsonData) =>
            {
                UTILS.Log($"Del_Comm : {jsonData}");
                Response_ReturnMsg response_Seq = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);

                Destroy(this.gameObject);
            }
            ));
    }

    /// <summary>
    /// 방명록 삭제 취소 시 실행
    /// </summary>
    void Delete_N()
    {
        uIInteractionManager_Room.Comm_DeletUI.SetActive(false);
    }


}
