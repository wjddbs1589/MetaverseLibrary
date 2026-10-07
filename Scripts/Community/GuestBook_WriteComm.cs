using Suncheon;
using Suncheon.Player;
using Suncheon.UI;
using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 방명록 글 작성 입력창.
/// 등록 버튼을 누르면 서버에 저장하고, 성공하면 입력창을 비운 뒤 목록을 새로 불러온다.
/// 서버가 금칙어를 감지하면(응답 코드 999) 안내 팝업을 띄운다.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class GuestBook_WriteComm : MonoBehaviour
{
    // 서버 응답 코드
    private const string CODE_SUCCESS = "000";
    private const string CODE_BANNED_WORD = "999";

    [Tooltip("등록 버튼")]
    [SerializeField] private Button btn_typing;

    private TMP_InputField _input;
    private GuestBook_CommList _commList;

    /// <summary>입력창과 목록 참조를 가져오고 등록 버튼을 연결한다.</summary>
    private void Awake()
    {
        _input = GetComponent<TMP_InputField>();
        _commList = FindAnyObjectByType<GuestBook_CommList>();
        btn_typing.onClick.AddListener(Btn_typing);
    }

    /// <summary>작성한 글을 서버에 저장한다.</summary>
    public void Btn_typing()
    {
        Request_WriteComm request = new Request_WriteComm(NetworkManager.Instance.user_id, _input.text);
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.writeComm}";

        StartCoroutine(UTILS.Requset_HttpPostData(url, request, jsonData =>
        {
            Response_ReturnMsg response = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);

            if (response.rtnCode == CODE_SUCCESS)
            {
                _input.text = "";
                _commList.Open_GuestBook();

                // 방명록 작성 횟수를 업적 정보에 반영한다
                NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().achievementInfo.commBoardCnt += 1;
            }
            else if (response.rtnCode == CODE_BANNED_WORD)
            {
                UIInteractionManager.Instance.ShowSystemPopUp("부적절한 단어가 포함되어 있습니다.");
            }
        }));
    }
}
