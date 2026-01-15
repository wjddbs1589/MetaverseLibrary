using Suncheon;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookRecommend : MonoBehaviour
{
    [HideInInspector]public string user_name;
    [HideInInspector]public string book_name;
    [HideInInspector]public string comm;
    int seq;
    string reported; //신고당한건 1, 아니면 0

    [Header("텍스트")]
    [SerializeField] TextMeshProUGUI Text_NickName;
    [SerializeField] TextMeshProUGUI Text_BookName;

    [Header("버튼")]
    [SerializeField] Button btn_Delete;
    [SerializeField] Button btn_Report;
    Button btn_comm;

    private void Awake()
    {
        btn_Delete.onClick.AddListener(() => Btn_Delete());
        btn_Report.onClick.AddListener(() => Btn_Report());

        btn_comm = GetComponent<Button>();
        btn_comm.onClick.AddListener(() => Btn_OpenComm());
    }

    /// <summary>
    /// 받아온 데이터에서 뽑아 책의 제목과 내용 입력
    /// </summary>
    public void Set_Text(Response_RecmDatas datas)
    {        
        user_name = datas.nickname;
        Text_NickName.text = user_name;
        book_name = datas.title;
        Text_BookName.text = book_name;        
        comm = datas.content;           
        seq = int.Parse(datas.recmSeq);
        reported = datas.report;

        //신고당한 글이면 블라인더 처리
        if (reported == "1")
        {
            Set_Report();
        }

        //내가 쓴 글이 아닐때 삭제 버튼 비 활성화
        if (GameManager.Instance.loginData.nickname != user_name)
        {            
            btn_Delete.gameObject.SetActive(false);
        }
        else
        {            
            btn_Report.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 게시글 열람
    /// </summary>
    void Btn_OpenComm()
    {
        ArchivesPC_UI.Instance.Btn_OpenComm(user_name, book_name, comm);
    }

    /// <summary>
    /// 신고버튼
    /// </summary>
    void Btn_Report()
    {
        Set_Report();

        Request_Report report_Text = new Request_Report("R", seq);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.report}",
            report_Text, (jsonData) =>
            {
                Response_ReturnMsg result = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                UTILS.Log(result.rtnMsg);
                UTILS.Log(result.rtnCode);
            }
            ));
    }

    void Set_Report()
    {
        btn_Report.gameObject.SetActive(false); //신고버튼 비 활성화
        btn_comm.interactable = false;          //추천글 열람 불가
        Text_BookName.text = "--- 해당 글은 신고가 접수되어 블라인드 처리되었습니다 ---"; //제목 변경
    }

    #region 추천도서 삭제
    void Btn_Delete()
    {
        ArchivesPC_UI pC_UI = ArchivesPC_UI.Instance;
        pC_UI.UI_DeletePopup.SetActive(true);

        DeletePopUp_UI deletePopUp_UI = pC_UI.UI_DeletePopup.GetComponent<DeletePopUp_UI>();
        deletePopUp_UI.btn_Yes.onClick.AddListener(() => Btn_Yes());
        deletePopUp_UI.btn_No.onClick.AddListener(() => Btn_No());
    }
    void Btn_Yes()
    {
        ArchivesPC_UI.Instance.UI_DeletePopup.SetActive(false);
        Delete_Data(seq);
        Destroy(gameObject);
    }
    void Btn_No()
    {
        ArchivesPC_UI.Instance.UI_DeletePopup.SetActive(false);
    }    

    /// <summary>
    /// 서버에 저장된 데이터 삭제
    /// </summary>
    /// <param name="seq"></param>
    void Delete_Data(int seq)
    {
        UTILS.Log($"{seq}");
        //데이터삭제 부분
        Request_RecmSeq Sequence = new Request_RecmSeq(seq);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.DeleteRecmComm}",
            Sequence, (jsonData) =>
            {
                Response_ReturnMsg response_Recm = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                UTILS.Log(response_Recm.rtnMsg); 

                ArchivesPC_UI.Instance.Get_RecmCommList(); //리스트 초기화
            }
            ));
    }
    #endregion
}
