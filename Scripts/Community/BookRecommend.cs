using Suncheon;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 추천도서 게시판의 글 항목 하나.
/// 누르면 본문을 열람하고, 내가 쓴 글이면 삭제 버튼을, 다른 사람의 글이면 신고 버튼을 표시한다.
/// 신고된 글은 제목을 블라인드 처리하고 열람할 수 없게 한다.
/// </summary>
[RequireComponent(typeof(Button))]
public class BookRecommend : MonoBehaviour
{
    // 신고된 글 표시 문구
    private const string BLIND_TEXT = "--- 해당 글은 신고가 접수되어 블라인드 처리되었습니다 ---";

    // 신고 대상 구분 코드 (R = 추천도서)
    private const string REPORT_TYPE_RECOMMEND = "R";

    // 서버 응답 성공 코드
    private const string CODE_SUCCESS = "000";

    [Header("텍스트")]
    [SerializeField] private TextMeshProUGUI Text_NickName;
    [SerializeField] private TextMeshProUGUI Text_BookName;

    [Header("버튼")]
    [SerializeField] private Button btn_Delete;
    [SerializeField] private Button btn_Report;

    // 글 정보
    private string _userName;
    private string _bookName;
    private string _content;
    private int _seq;

    // 항목 자체 버튼 (누르면 본문 열람)
    private Button _btnOpen;

    /// <summary>버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        btn_Delete.onClick.AddListener(Btn_Delete);
        btn_Report.onClick.AddListener(Btn_Report);

        _btnOpen = GetComponent<Button>();
        _btnOpen.onClick.AddListener(() => ArchivesPC_UI.Instance.Btn_OpenComm(_userName, _bookName, _content));
    }

    /// <summary>서버에서 받은 글 정보를 표시하고, 작성자 여부에 따라 삭제·신고 버튼을 나눈다.</summary>
    public void Set_Text(Response_RecmDatas data)
    {
        _userName = data.nickname;
        _bookName = data.title;
        _content = data.content;
        _seq = int.Parse(data.recmSeq);

        Text_NickName.text = _userName;
        Text_BookName.text = _bookName;

        if (data.report == "1")
            Set_Report();

        bool isMine = GameManager.Instance.loginData.nickname == _userName;
        btn_Delete.gameObject.SetActive(isMine);
        if (isMine) btn_Report.gameObject.SetActive(false);
    }

    // ── 신고 ─────────────────────────────────────────────

    /// <summary>글을 블라인드 처리하고 서버에 신고를 접수한다.</summary>
    private void Btn_Report()
    {
        Set_Report();

        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.report}";
        StartCoroutine(UTILS.Requset_HttpPostData(url, new Request_Report(REPORT_TYPE_RECOMMEND, _seq), jsonData =>
        {
            Response_ReturnMsg result = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
            UTILS.Log($"[추천도서 신고] {result.rtnCode} {result.rtnMsg}");
        }));
    }

    /// <summary>신고 버튼을 숨기고, 제목을 블라인드 문구로 바꾼 뒤 열람을 막는다.</summary>
    private void Set_Report()
    {
        btn_Report.gameObject.SetActive(false);
        _btnOpen.interactable = false;
        Text_BookName.text = BLIND_TEXT;
    }

    // ── 삭제 ─────────────────────────────────────────────

    /// <summary>
    /// 삭제 버튼. 공용 삭제 확인 팝업을 열고 확인·취소 버튼을 이 글에 연결한다.
    /// 팝업 버튼은 모든 글 항목이 함께 쓰므로, 이전에 연결된 다른 글의 리스너를 먼저 지운다.
    /// </summary>
    private void Btn_Delete()
    {
        GameObject popup = ArchivesPC_UI.Instance.UI_DeletePopup;
        popup.SetActive(true);

        DeletePopUp_UI deletePopUp = popup.GetComponent<DeletePopUp_UI>();
        deletePopUp.btn_Yes.onClick.RemoveAllListeners();
        deletePopUp.btn_No.onClick.RemoveAllListeners();

        deletePopUp.btn_Yes.onClick.AddListener(Btn_Yes);
        deletePopUp.btn_No.onClick.AddListener(ClosePopup);
    }

    /// <summary>삭제 확인. 팝업을 닫고 서버에 삭제를 요청한다.</summary>
    private void Btn_Yes()
    {
        ClosePopup();
        Delete_Data();
    }

    /// <summary>삭제 확인 팝업을 닫는다.</summary>
    private void ClosePopup()
    {
        ArchivesPC_UI.Instance.UI_DeletePopup.SetActive(false);
    }

    /// <summary>
    /// 서버에서 글을 삭제하고, 성공 응답을 받으면 목록을 새로 불러온다.
    /// 응답을 받기 전에 항목을 파괴하면 이 오브젝트에서 실행 중인 요청 코루틴도 함께 중단되므로,
    /// 항목 정리는 목록 갱신에 맡긴다.
    /// </summary>
    private void Delete_Data()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.DeleteRecmComm}";
        StartCoroutine(UTILS.Requset_HttpPostData(url, new Request_RecmSeq(_seq), jsonData =>
        {
            Response_ReturnMsg response = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
            UTILS.Log($"[추천도서 삭제] {response.rtnCode} {response.rtnMsg}");

            if (response.rtnCode == CODE_SUCCESS)
                ArchivesPC_UI.Instance.Get_RecmCommList();
        }));
    }
}
