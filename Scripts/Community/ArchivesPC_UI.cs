using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 추천도서 게시판 UI.
/// 목록 화면과 본문(작성·열람) 화면을 전환하며, 열 때마다 서버에서 추천글 목록을 불러온다.
/// </summary>
public class ArchivesPC_UI : MonoBehaviour
{
    public static ArchivesPC_UI Instance { get; private set; }

    [Header("목록 화면")]
    [SerializeField] private GameObject UI_List;

    [Tooltip("추천글 항목을 생성할 스크롤뷰 Content")]
    [SerializeField] private Transform content;

    [Header("본문 화면")]
    [SerializeField] private GameObject UI_Comm;

    [Tooltip("추천글 항목 프리팹 (BookRecommend)")]
    [SerializeField] private GameObject CommPreafab;

    [Header("버튼")]
    [Tooltip("글 작성 버튼")]
    [SerializeField] private Button btn_typing;

    [Tooltip("게시판 닫기 버튼")]
    [SerializeField] private Button btn_exit;

    [Header("확인 팝업")]
    [Tooltip("추천글 삭제 확인 팝업 (DeletePopUp_UI)")]
    public GameObject UI_DeletePopup;

    [Tooltip("작성 취소 확인 팝업")]
    public GameObject UI_CancelPopup;

    private Recommend_UI _recommendUI;

    /// <summary>싱글톤을 등록하고 버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;

        _recommendUI = UI_Comm.GetComponent<Recommend_UI>();

        btn_typing.onClick.AddListener(Btn_Typing);
        btn_exit.onClick.AddListener(Btn_Exit);
    }

    /// <summary>씬이 바뀌어 파괴되면 싱글톤 참조를 비운다.</summary>
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>게시판을 열면 목록 화면을 표시한다.</summary>
    private void OnEnable()
    {
        Set_ListUI();
    }

    /// <summary>게시판을 닫으면 닫기 버튼 이미지를 되돌린다.</summary>
    private void OnDisable()
    {
        btn_exit.GetComponent<ButtonControl>().Btnoff();
    }

    // ── 버튼 ─────────────────────────────────────────────

    /// <summary>글 작성 버튼. 본문 화면을 작성 모드로 연다.</summary>
    private void Btn_Typing()
    {
        _recommendUI.writeMode = true;
        _recommendUI.read = false;
        ShowCommView();
    }

    /// <summary>목록의 글을 누르면 본문 화면을 열람 모드로 열고 내용을 채운다.</summary>
    public void Btn_OpenComm(string user_name, string book_name, string comm)
    {
        ShowCommView();
        _recommendUI.Get_CommText(user_name, book_name, comm);
    }

    /// <summary>본문 화면을 닫고 목록 화면으로 돌아간다.</summary>
    public void RecmCommUI_Close()
    {
        Set_ListUI();
    }

    /// <summary>게시판을 닫는다.</summary>
    private void Btn_Exit()
    {
        btn_exit.GetComponent<ButtonControl>().Btnoff();
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }

    // ── 화면 전환 ────────────────────────────────────────

    /// <summary>목록 화면으로 전환하고 목록을 새로 불러온다.</summary>
    private void Set_ListUI()
    {
        UI_Comm.SetActive(false);
        UI_List.SetActive(true);
        Get_RecmCommList();
    }

    /// <summary>본문 화면으로 전환한다.</summary>
    private void ShowCommView()
    {
        UI_List.SetActive(false);
        UI_Comm.SetActive(true);
    }

    // ── 목록 ─────────────────────────────────────────────

    /// <summary>서버에서 추천글 목록을 불러와 항목을 다시 생성한다. 글을 삭제한 뒤에도 호출한다.</summary>
    public void Get_RecmCommList()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.GetRecmList}";
        StartCoroutine(UTILS.Requset_HttpGetData(url, jsonData =>
        {
            Response_RecmCommList list = new Response_RecmCommList();

            foreach (JObject jObject in JArray.Parse(jsonData))
                list.RecmCommListData.Add(JsonUtility.FromJson<Response_RecmDatas>(jObject.ToString()));

            Reset_CommList(list);
        }));
    }

    /// <summary>기존 항목을 지우고 받은 데이터로 추천글 항목을 생성한다.</summary>
    private void Reset_CommList(Response_RecmCommList list)
    {
        foreach (BookRecommend child in content.GetComponentsInChildren<BookRecommend>())
            Destroy(child.gameObject);

        foreach (Response_RecmDatas data in list.RecmCommListData)
        {
            GameObject item = Instantiate(CommPreafab, content);
            item.GetComponent<BookRecommend>().Set_Text(data);
        }
    }
}
