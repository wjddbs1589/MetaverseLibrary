using System.Collections;
using UnityEngine;
using Suncheon;
using Suncheon.WebData;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;

public class GuestBook_CommList : MonoBehaviour
{
    [Header("방명록 코멘트 프리펩")]
    [SerializeField] GameObject go_GuestComm;

    [SerializeField] Transform content_Comm;

    [Header("정렬 버튼")]
    [SerializeField] Button btn_asc;
    [SerializeField] Button btn_desc;

    [Header("이미지 색상")]
    Color ascColor;
    Color descColor;

    TextMeshProUGUI ascText;
    TextMeshProUGUI descText;

    [Header("텍스트 색상")]
    [SerializeField] Color Text_OnColor;
    [SerializeField] Color Text_OffColor;

    string orderBy = "asc";
    ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();

        btn_asc.onClick.AddListener(() => Btn_asc());
        btn_desc.onClick.AddListener(() => Btn_desc());
        ascColor = btn_asc.image.color;
        descColor = btn_desc.image.color;

        ascText = btn_asc.GetComponentInChildren<TextMeshProUGUI>();
        descText = btn_desc.GetComponentInChildren<TextMeshProUGUI>();

        Btn_desc();
    }

    private void OnEnable()
    {
        Open_GuestBook();
    }

    /// <summary>
    /// 최신순 정렬 버튼
    /// </summary>
    void Btn_asc()
    {
        orderBy = "asc";
        Set_Btn_UI();    
        Open_GuestBook();
    }

    /// <summary>
    /// 오래된 순 정렬 버튼
    /// </summary>
    void Btn_desc()
    {
        orderBy = "desc";
        Set_Btn_UI();
        Open_GuestBook();
    }

    /// <summary>
    /// orderBy값에 따라 버튼의 이미지 및 글자 색상 변경
    /// </summary>
    void Set_Btn_UI()
    {
        if (orderBy == "asc")
        {
            ascColor.a = 1;
            descColor.a = 0;
            ascText.color = Text_OnColor;
            descText.color = Text_OffColor;
        }
        else
        {
            ascColor.a = 0;
            descColor.a = 1;
            ascText.color = Text_OffColor;
            descText.color = Text_OnColor;
        }
        btn_asc.image.color = ascColor;
        btn_desc.image.color = descColor;
    }

    /// <summary>
    /// 방명록 상호작용시 방명록 UI 활성화
    /// </summary>
    public void Open_GuestBook()
    {
        if (NetworkManager.Instance.Check_MyRoom())
        {
            //내 서재의 방명록
            GetMyCommLists();
        }
        else
        {
            //타 유저의 서재 방명록
            GetUserCommLists();
        }
    }

    #region 방명록 정보 가져오기
    /// <summary>
    /// 나의 방명록 정보를 가져옴
    /// </summary>
    public void GetMyCommLists()
    {
        Request_MyCommList request_MyCommList = new Request_MyCommList(orderBy);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.myCommList}", 
        request_MyCommList, (jsonData) => 
        {
            UTILS.Log($"GetMyCommLists : {jsonData}");
            Response_CommList response_MyCommList = new Response_CommList();
            JArray jArray = JArray.Parse(jsonData);

            foreach(JObject jObject in jArray)
            {
                Response_CommListResultData resultData = JsonUtility.FromJson<Response_CommListResultData>(jObject.ToString());
                response_MyCommList.response_UserCommListResultDatas.Add(resultData);
            }

            ShowCommLists(response_MyCommList);

            Scroll_Move();
        }));
    }
    
    /// <summary>
    /// 타 유저의 방명록 정보를 가져옴
    /// </summary>
    public void GetUserCommLists()
    {
        Request_UserCommList request_UserCommList = new Request_UserCommList(NetworkManager.Instance.user_id, orderBy);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userCommList}",
            request_UserCommList, (jsonData) => 
            {
                UTILS.Log($"GetUserCommLists : {jsonData}");
                Response_CommList response_UserCommList = new Response_CommList();
                JArray jArray = JArray.Parse(jsonData);

                foreach (JObject jobj in jArray)
                {
                    Response_CommListResultData resultData = JsonUtility.FromJson<Response_CommListResultData>(jobj.ToString());
                    response_UserCommList.response_UserCommListResultDatas.Add(resultData);
                }

                ShowCommLists(response_UserCommList);
                Scroll_Move();
            }
            ));       
    }

    /// <summary>
    /// 코멘트 리스트를 가져와서 프리펩에 정보를 대입하여 생성
    /// </summary>
    /// <param name="resultDatas"></param>
    public void ShowCommLists(Response_CommList resultDatas)
    {
        //기존에 존재하는 방명록 내역 삭제
        foreach (GuestBook_TalkUI child in content_Comm.GetComponentsInChildren<GuestBook_TalkUI>())
        {
            Destroy(child.gameObject);
        }

        foreach (Response_CommListResultData resultData in resultDatas.response_UserCommListResultDatas)
        {
            GameObject go = Instantiate(go_GuestComm, content_Comm); //스크롤뷰의 content_Comm및으로 자식생성
            go.GetComponent<GuestBook_TalkUI>().InitGuestBook(resultData); // 가져온 데이터를 생성된 프리펩에서 사용되도록 값 전달
        }
    }
    #endregion

    /// <summary>
    /// 스크롤뷰의 스크롤을 이동
    /// </summary>
    public void Scroll_Move()
    {
        StartCoroutine(Set_ScrollPos_Co());
    }

    /// <summary>
    /// 스크롤 위치를 가장 하단으로 이동시킴
    /// </summary>
    /// <returns></returns>
    IEnumerator Set_ScrollPos_Co()
    {
        yield return new WaitForSeconds(.2f);
        scrollRect.normalizedPosition = new Vector2(0f, 1f);
    }
}
