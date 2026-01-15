using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using UnityEngine;
using UnityEngine.UI;

public class ArchivesPC_UI : MonoBehaviour
{    
    static ArchivesPC_UI instance = null;
    public static ArchivesPC_UI Instance => instance;

    [Header("추천도서 리스트 UI")]
    [SerializeField] GameObject UI_List;
    [SerializeField] Transform content;

    [Header("코멘트 창 UI")]
    [SerializeField] GameObject UI_Comm;
    [SerializeField] GameObject CommPreafab;

    [Header("글 작성하기 버튼")]
    [SerializeField] Button btn_typing;

    [Header("UI 닫기 버튼")]
    [SerializeField] Button btn_exit;

    public GameObject UI_DeletePopup;
    public GameObject UI_CancelPopup;

    Recommend_UI recommendUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        recommendUI = UI_Comm.GetComponent<Recommend_UI>();

        btn_typing.onClick.AddListener(() => Btn_Typing());
        btn_exit.onClick.AddListener(() => Btn_Exit());
    }

    /// <summary>
    /// 활성화 시 추천도서 리스트 활성화
    /// </summary>
    private void OnEnable()
    {
        Set_ListUI();
    }

    private void OnDisable()
    {
        btn_exit.GetComponent<ButtonControl>().Btnoff();
    }

    #region 버튼 실행 함수
    /// <summary>
    /// 코멘트 작성 UI 활성화
    /// </summary>
    void Btn_Typing()
    {
        recommendUI.writeMode = true;
        recommendUI.read = false;
        Open_Comm();
    }

    /// <summary>
    /// 코멘트 열람 UI 활성화
    /// </summary>
    public void Btn_OpenComm(string user_name, string book_name, string comm)
    {
        Open_Comm(user_name, book_name, comm);
    }

    /// <summary>
    /// 추천 코멘트 UI닫고 리스트UI열기
    /// </summary>
    public void RecmCommUI_Close()
    {
        //작성중이던 글의 내용 초기화 하는 코드 작성
        Set_ListUI();
    }

    /// <summary>
    /// 추천도서 UI 닫기
    /// </summary>
    void Btn_Exit()
    {
        btn_exit.GetComponent<ButtonControl>().Btnoff();
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }
    #endregion    

    #region UI 전환(목록 <-> 본문)
    /// <summary>
    /// 해당 게임오브젝트 닫기
    /// </summary>
    /// <param name="UI">UI 게임오브젝트</param>
    void Close_UI(GameObject UI)
    {
        UI.SetActive(false);
    }

    /// <summary>
    /// 해당 게임오브젝트 열기
    /// </summary>
    /// <param name="UI">UI 게임오브젝트</param>
    void Open_UI(GameObject UI)
    {
        UI.SetActive(true);
    }

    /// <summary>
    /// 추천 도서 목록을 활성화
    /// </summary>
    void Set_ListUI()
    {
        Close_UI(UI_Comm);
        Open_UI(UI_List);
        Get_RecmCommList();
    }

    /// <summary>
    /// 전체 코멘트 리스트 정보를 가져옴
    /// </summary>
    public void Get_RecmCommList()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.GetRecmList}",
            (jsonData) =>
            {
                UTILS.Log(jsonData);
                JArray jArray = JArray.Parse(jsonData);
                Response_RecmCommList response_RecmInfo = new Response_RecmCommList();
                foreach (JObject jobj in jArray)
                {
                    Response_RecmDatas resultData = JsonUtility.FromJson<Response_RecmDatas>(jobj.ToString());
                    response_RecmInfo.RecmCommListData.Add(resultData);
                }                
                Reset_CommList(response_RecmInfo);
            }
            ));
    }

    /// <summary>
    /// 코멘트 UI생성후 내용 삽입
    /// </summary>
    /// <param name="resultDatas"></param>
    void Reset_CommList(Response_RecmCommList resultDatas)
    {
        //기존에 존재하는 방명록 내역 삭제
        foreach (BookRecommend child in content.GetComponentsInChildren<BookRecommend>())
        {
            Destroy(child.gameObject);
        }

        foreach (Response_RecmDatas resultData in resultDatas.RecmCommListData)
        {            
            GameObject go = Instantiate(CommPreafab, content);
            go.GetComponent<BookRecommend>().Set_Text(resultData);
        }
    }

    /// <summary>
    /// 게시글 열람시 실행, 문자열 정보 전달
    /// </summary>
    /// <param name="user_name">유저 닉네임</param>
    /// <param name="book_name">책 제목</param>
    /// <param name="comm">내용</param>
    void Open_Comm(string user_name, string book_name, string comm)
    {        
        //코멘트 UI 활성화 하여 각 정보 대입
        Open_UI(UI_Comm);
        recommendUI.Get_CommText(user_name, book_name, comm);

        //리스트 UI 닫기
        Close_UI(UI_List);
    }

    /// <summary>
    /// 코멘트 작성시 실행
    /// </summary>
    void Open_Comm()
    {
        Close_UI(UI_List);
        Open_UI(UI_Comm);
    }
    #endregion    
}
