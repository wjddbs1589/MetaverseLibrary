using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.WebData;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 방명록 글 목록.
/// 내 서재면 내 방명록을, 다른 사람의 서재면 방 주인의 방명록을 불러와 글 항목(GuestBook_TalkUI)을 생성한다.
/// 정렬 버튼으로 순서를 바꿀 수 있다.
/// </summary>
public class GuestBook_CommList : MonoBehaviour
{
    private const string ORDER_ASC = "asc";
    private const string ORDER_DESC = "desc";

    // 목록 생성 후 레이아웃이 갱신될 때까지 기다리는 시간 (초)
    private const float SCROLL_RESET_DELAY = 0.2f;

    [Header("글 항목")]
    [Tooltip("방명록 글 항목 프리팹 (GuestBook_TalkUI)")]
    [SerializeField] private GameObject go_GuestComm;

    [Tooltip("글 항목을 생성할 스크롤뷰 Content")]
    [SerializeField] private Transform content_Comm;

    [Header("정렬 버튼")]
    [SerializeField] private Button btn_asc;
    [SerializeField] private Button btn_desc;

    [Header("정렬 버튼 글자 색상")]
    [SerializeField] private Color Text_OnColor;
    [SerializeField] private Color Text_OffColor;

    private string _orderBy = ORDER_ASC;
    private ScrollRect _scrollRect;

    // 정렬 버튼의 배경 색과 글자
    private Color _ascColor;
    private Color _descColor;
    private TextMeshProUGUI _ascText;
    private TextMeshProUGUI _descText;

    /// <summary>정렬 버튼을 연결하고 기본 정렬(desc)을 적용한다.</summary>
    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();

        btn_asc.onClick.AddListener(() => SetOrder(ORDER_ASC));
        btn_desc.onClick.AddListener(() => SetOrder(ORDER_DESC));

        _ascColor = btn_asc.image.color;
        _descColor = btn_desc.image.color;
        _ascText = btn_asc.GetComponentInChildren<TextMeshProUGUI>();
        _descText = btn_desc.GetComponentInChildren<TextMeshProUGUI>();

        SetOrder(ORDER_DESC);
    }

    /// <summary>방명록을 열 때마다 목록을 새로 불러온다.</summary>
    private void OnEnable()
    {
        Open_GuestBook();
    }

    /// <summary>정렬 순서를 바꾸고 버튼 표시와 목록을 갱신한다.</summary>
    private void SetOrder(string orderBy)
    {
        _orderBy = orderBy;
        Set_Btn_UI();
        Open_GuestBook();
    }

    /// <summary>선택된 정렬 버튼만 배경과 글자를 강조한다.</summary>
    private void Set_Btn_UI()
    {
        bool isAsc = _orderBy == ORDER_ASC;

        _ascColor.a = isAsc ? 1 : 0;
        _descColor.a = isAsc ? 0 : 1;
        btn_asc.image.color = _ascColor;
        btn_desc.image.color = _descColor;

        _ascText.color = isAsc ? Text_OnColor : Text_OffColor;
        _descText.color = isAsc ? Text_OffColor : Text_OnColor;
    }

    /// <summary>서재 주인에 맞는 방명록 목록을 요청한다. 새 글을 등록한 뒤에도 호출한다.</summary>
    public void Open_GuestBook()
    {
        string baseUrl = GameManager.Instance.defaultData.serviceUrl;

        if (NetworkManager.Instance.Check_MyRoom())
        {
            Request_MyCommList request = new Request_MyCommList(_orderBy);
            StartCoroutine(UTILS.Requset_HttpPostData($"{baseUrl}{GameManager.Instance.defaultData.myCommList}", request, OnCommListReceived));
        }
        else
        {
            Request_UserCommList request = new Request_UserCommList(NetworkManager.Instance.user_id, _orderBy);
            StartCoroutine(UTILS.Requset_HttpPostData($"{baseUrl}{GameManager.Instance.defaultData.userCommList}", request, OnCommListReceived));
        }
    }

    /// <summary>서버 응답(JSON 배열)을 글 목록으로 변환해 화면에 표시하고 스크롤을 맨 위로 옮긴다.</summary>
    private void OnCommListReceived(string jsonData)
    {
        Response_CommList commList = new Response_CommList();

        foreach (JObject jObject in JArray.Parse(jsonData))
            commList.response_UserCommListResultDatas.Add(JsonUtility.FromJson<Response_CommListResultData>(jObject.ToString()));

        ShowCommLists(commList);
        StartCoroutine(ResetScrollPosition());
    }

    /// <summary>기존 글 항목을 지우고 받은 데이터로 글 항목을 다시 생성한다.</summary>
    public void ShowCommLists(Response_CommList resultDatas)
    {
        foreach (GuestBook_TalkUI child in content_Comm.GetComponentsInChildren<GuestBook_TalkUI>())
            Destroy(child.gameObject);

        foreach (Response_CommListResultData data in resultDatas.response_UserCommListResultDatas)
        {
            GameObject item = Instantiate(go_GuestComm, content_Comm);
            item.GetComponent<GuestBook_TalkUI>().InitGuestBook(data);
        }
    }

    /// <summary>레이아웃이 갱신된 뒤 스크롤 위치를 맨 위로 옮긴다.</summary>
    private IEnumerator ResetScrollPosition()
    {
        yield return new WaitForSeconds(SCROLL_RESET_DELAY);
        _scrollRect.normalizedPosition = new Vector2(0f, 1f);
    }
}
