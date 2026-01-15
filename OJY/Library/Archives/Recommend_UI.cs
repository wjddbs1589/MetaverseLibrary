using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Recommend_UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI User_Name;
    [SerializeField] TMP_InputField bookNameFeild;
    [SerializeField] TMP_InputField commFeild;

    public bool writeMode = false;
    public bool read = false;

    [Header("글 작성 버튼")]
    [SerializeField] Button btn_save;
    [SerializeField] Button btn_cancel;

    ArchivesPC_UI pc_UI;
    private void Awake()
    {
        pc_UI = GetComponentInParent<ArchivesPC_UI>();
        btn_save.onClick.AddListener(() => Btn_Save());
        btn_cancel.onClick.AddListener(() => Btn_Cancel());
    }

    private void OnEnable()
    {
        //현재 모드가 글 작성 모드일때 인풋필드 활성화
        if (writeMode)
        {
            User_Name.text = GameManager.Instance.loginData.nickname;

            //bookNameFeild.readOnly = false;
            //commFeild.readOnly = false;
            bookNameFeild.interactable = true;
            commFeild.interactable = true;
            btn_save.gameObject.SetActive(true);
        }
        else
        {
            //bookNameFeild.readOnly = true;
            //commFeild.readOnly = true;
            bookNameFeild.interactable = false;
            commFeild.interactable = false;
            btn_save.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        writeMode = false;
        read = true;
    }

    /// <summary>
    /// 추천글을 열람할 때 대상 글이 가지고있는 텍스트 정보를 가져옴, 작성된 글에 들어올때 호출
    /// </summary>
    /// <param name="user_name">닉네임</param>
    /// <param name="book_name">책 제목</param>
    /// <param name="comm">내용</param>
    public void Get_CommText(string user_name, string book_name, string comm)
    {
        read = true;
        User_Name.text = user_name;
        bookNameFeild.text = book_name;
        commFeild.text = comm;
    }

    /// <summary>
    /// 등록버튼 클릭시 실행
    /// </summary>
    void Btn_Save()
    {
        Save_RecmComm(bookNameFeild.text, commFeild.text);        
    }

    /// <summary>
    /// 취소 버튼 클릭시 실행
    /// </summary>
    void Btn_Cancel()
    {
        if (!read)
        {
            pc_UI.UI_CancelPopup.SetActive(true);
        }
        else
        {
            UI_Off();
        }
        
    }
    public void Cancel()
    {
        UI_Off();
    }

    /// <summary>
    /// 작성한 내용 저장
    /// </summary>
    void Save_RecmComm(string title, string comm)
    {
        Request_SaveRecmComm save_recm = new Request_SaveRecmComm(title, comm);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.SaveRecmComm}",
            save_recm, (JsonData) =>
            {
                Response_ReturnMsg response_recm = new Response_ReturnMsg();
                response_recm = JsonUtility.FromJson<Response_ReturnMsg>(JsonData);

                UTILS.Log(response_recm.rtnMsg);
                UTILS.Log(response_recm.rtnCode);

                if (response_recm.rtnCode == "000")
                {
                    UI_Off();
                }
                else if (response_recm.rtnCode == "999")
                {
                    UIInteractionManager.Instance.ShowSystemPopUp("내용이 없거나 부적절한 단어가 포함되어 있습니다.");
                }
            }
            ));
    }

    /// <summary>
    /// 내용 초기화 후 창 닫기
    /// </summary>
    void UI_Off()
    {
        UTILS.Log("창 변환");
        bookNameFeild.text = "추천할 책의 제목을 입력하세요.";
        commFeild.text = "내용을 입력하세요.";
        pc_UI.RecmCommUI_Close();
    }
}
