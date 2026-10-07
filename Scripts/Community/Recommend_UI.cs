using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 추천도서 본문 화면. 작성 모드와 열람 모드를 함께 처리한다.
///
/// 작성 모드 → 입력창 활성화, 등록 버튼 표시. 취소 시 작성 내용이 사라진다는 확인 팝업을 띄운다.
/// 열람 모드 → 입력창 읽기 전용, 등록 버튼 숨김. 취소 시 바로 목록으로 돌아간다.
/// </summary>
public class Recommend_UI : MonoBehaviour
{
    // 서버 응답 코드
    private const string CODE_SUCCESS = "000";
    private const string CODE_INVALID = "999";

    [SerializeField] private TextMeshProUGUI User_Name;
    [SerializeField] private TMP_InputField bookNameFeild;
    [SerializeField] private TMP_InputField commFeild;

    [Header("버튼")]
    [SerializeField] private Button btn_save;
    [SerializeField] private Button btn_cancel;

    /// <summary>작성 모드 여부 (ArchivesPC_UI에서 설정)</summary>
    [HideInInspector] public bool writeMode = false;

    /// <summary>열람 중이면 true. 취소 시 확인 팝업 없이 닫는다.</summary>
    [HideInInspector] public bool read = false;

    private ArchivesPC_UI _pcUI;

    /// <summary>게시판 참조를 가져오고 버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        _pcUI = GetComponentInParent<ArchivesPC_UI>();
        btn_save.onClick.AddListener(() => Save_RecmComm(bookNameFeild.text, commFeild.text));
        btn_cancel.onClick.AddListener(Btn_Cancel);
    }

    /// <summary>모드에 맞게 입력창과 등록 버튼을 설정한다.</summary>
    private void OnEnable()
    {
        if (writeMode)
            User_Name.text = GameManager.Instance.loginData.nickname;

        bookNameFeild.interactable = writeMode;
        commFeild.interactable = writeMode;
        btn_save.gameObject.SetActive(writeMode);
    }

    /// <summary>화면을 닫으면 기본 상태(열람 모드)로 되돌린다.</summary>
    private void OnDisable()
    {
        writeMode = false;
        read = true;
    }

    /// <summary>목록에서 고른 글의 작성자, 제목, 내용을 표시한다.</summary>
    public void Get_CommText(string user_name, string book_name, string comm)
    {
        read = true;
        User_Name.text = user_name;
        bookNameFeild.text = book_name;
        commFeild.text = comm;
    }

    /// <summary>취소 버튼. 작성 중이면 확인 팝업을 띄우고, 열람 중이면 바로 닫는다.</summary>
    private void Btn_Cancel()
    {
        if (!read)
            _pcUI.UI_CancelPopup.SetActive(true);
        else
            UI_Off();
    }

    /// <summary>작성 취소 확인 팝업에서 확인을 누르면 호출된다.</summary>
    public void Cancel()
    {
        UI_Off();
    }

    /// <summary>작성한 추천글을 서버에 저장한다. 성공하면 목록으로 돌아가고, 내용이 없거나 금칙어가 있으면 안내한다.</summary>
    private void Save_RecmComm(string title, string comm)
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.SaveRecmComm}";
        StartCoroutine(UTILS.Requset_HttpPostData(url, new Request_SaveRecmComm(title, comm), jsonData =>
        {
            Response_ReturnMsg response = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
            UTILS.Log($"[추천도서 저장] {response.rtnCode} {response.rtnMsg}");

            if (response.rtnCode == CODE_SUCCESS)
                UI_Off();
            else if (response.rtnCode == CODE_INVALID)
                UIInteractionManager.Instance.ShowSystemPopUp("내용이 없거나 부적절한 단어가 포함되어 있습니다.");
        }));
    }

    /// <summary>
    /// 입력 내용을 비우고 목록 화면으로 돌아간다.
    /// 안내 문구는 입력값이 아닌 InputField의 Placeholder로 표시해, 수정하지 않고 등록해도 안내 문구가 저장되지 않게 한다.
    /// </summary>
    private void UI_Off()
    {
        bookNameFeild.text = "";
        commFeild.text = "";
        _pcUI.RecmCommUI_Close();
    }
}
