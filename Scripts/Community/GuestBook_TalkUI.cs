using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 방명록 글 항목 하나.
///
/// 권한별 버튼:
///   내 서재       → 삭제 버튼 (방명록 주인)
///   다른 사람 서재 → 신고 버튼 (단, 내가 쓴 글에는 신고 버튼 없음)
///
/// 내 서재에서 글을 표시하면 읽음 처리하고, 신고된 글은 내용을 블라인드 처리한다.
/// </summary>
public class GuestBook_TalkUI : MonoBehaviour
{
    // 신고된 글 표시 문구
    private const string BLIND_TEXT = "--- 해당 글은 신고가 접수되어 블라인드 처리되었습니다 ---";

    // 신고 대상 구분 코드 (C = 방명록)
    private const string REPORT_TYPE_COMMENT = "C";

    [SerializeField] private Button btn_Delete;
    [SerializeField] private Button btn_Report;
    [SerializeField] private TMP_Text text_Name;
    [SerializeField] private TMP_Text text_Comm;

    // 서버의 글 번호
    private int _seq;

    // 삭제 확인 팝업을 가진 서재 UI 매니저
    private UIInteractionManager_Room _roomUI;

    /// <summary>버튼을 연결하고, 내 서재인지에 따라 삭제·신고 버튼을 나눠 표시한다.</summary>
    private void Awake()
    {
        _roomUI = FindAnyObjectByType<UIInteractionManager_Room>();

        btn_Report.onClick.AddListener(Report_Comm);
        btn_Delete.onClick.AddListener(Btn_Delete);

        bool isMyRoom = NetworkManager.Instance.Check_MyRoom();
        btn_Delete.gameObject.SetActive(isMyRoom);
        btn_Report.gameObject.SetActive(!isMyRoom);
    }

    /// <summary>서버에서 받은 글 정보를 표시한다.</summary>
    public void InitGuestBook(Response_CommListResultData data)
    {
        // 내가 쓴 글은 신고할 수 없다
        if (data.fromId == GameManager.Instance.loginData.user_id)
            btn_Report.gameObject.SetActive(false);

        text_Name.text = data.fromNickname;
        _seq = data.commSeq;

        // 방명록 주인이 글을 보면 읽음 처리한다 (새 글 알림 해제)
        if (NetworkManager.Instance.Check_MyRoom())
            Read_Comm(_seq);

        if (data.report == "1")
            Set_Report();
        else
            text_Comm.text = data.commCn;
    }

    /// <summary>글을 읽음으로 표시하도록 서버에 요청한다.</summary>
    private void Read_Comm(int seq)
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.ReadnewComm}";
        StartCoroutine(UTILS.Requset_HttpPostData(url, new Request_ReadComm(seq), jsonData => UTILS.Log(jsonData)));
    }

    /// <summary>글을 블라인드 처리하고 서버에 신고를 접수한다.</summary>
    private void Report_Comm()
    {
        Set_Report();

        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.report}";
        StartCoroutine(UTILS.Requset_HttpPostData(url, new Request_Report(REPORT_TYPE_COMMENT, _seq), jsonData =>
        {
            Response_ReturnMsg result = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
            UTILS.Log($"[방명록 신고] {result.rtnCode} {result.rtnMsg}");
        }));
    }

    /// <summary>신고 버튼을 숨기고 내용을 블라인드 문구로 바꾼다.</summary>
    private void Set_Report()
    {
        btn_Report.gameObject.SetActive(false);
        text_Comm.text = BLIND_TEXT;
    }

    /// <summary>
    /// 삭제 버튼. 공용 삭제 확인 팝업을 열고, 확인·취소 버튼을 이 글에 연결한다.
    /// 팝업 버튼은 모든 글 항목이 함께 쓰므로, 이전에 연결된 다른 글의 리스너를 먼저 지운다.
    /// </summary>
    private void Btn_Delete()
    {
        _roomUI.Comm_DeletUI.SetActive(true);

        _roomUI.btn_Delete_Ok.onClick.RemoveAllListeners();
        _roomUI.btn_Delete_Cancle.onClick.RemoveAllListeners();

        _roomUI.btn_Delete_Ok.onClick.AddListener(Delete_Y);
        _roomUI.btn_Delete_Cancle.onClick.AddListener(Delete_N);
    }

    /// <summary>삭제 확인. 팝업을 닫고 서버에 삭제를 요청한다.</summary>
    private void Delete_Y()
    {
        _roomUI.Comm_DeletUI.SetActive(false);
        Del_Comm();
    }

    /// <summary>삭제 취소. 팝업을 닫는다.</summary>
    private void Delete_N()
    {
        _roomUI.Comm_DeletUI.SetActive(false);
    }

    /// <summary>서버에서 글을 삭제하고, 응답을 받으면 글 항목을 지운다.</summary>
    private void Del_Comm()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.DelComm}";
        StartCoroutine(UTILS.Requset_HttpPostData(url, new Request_Seq(_seq), jsonData =>
        {
            UTILS.Log($"[방명록 삭제] {jsonData}");
            Destroy(gameObject);
        }));
    }
}
