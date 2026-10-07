using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.UI;
using Suncheon.WebData;
using UnityEngine;

/// <summary>
/// 광장의 투표함 오브젝트.
/// 클릭하면 투표 진행 여부를 확인하고, 진행 중이면 투표 주제와 항목을 받아 투표 UI를 연다.
/// 이미 투표했거나 투표할 수 없는 경우에는 서버가 보낸 안내 메시지를 띄운다.
/// 게스트 계정은 사용할 수 없다.
/// </summary>
public class Vote : MonoBehaviour, IClickable
{
    [Tooltip("투표 UI (Vote_UI)")]
    [SerializeField] private GameObject UI_Vote;

    /// <summary>게스트가 아니면 투표 진행 여부부터 확인한다.</summary>
    public void OnClick()
    {
        if (GameManager.Instance.IsGuest)
        {
            UIInteractionManager.Instance.ShowSystemPopUp("Guest 계정은 해당 서비스를 이용할 수 없습니다.");
            return;
        }

        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteProcess}";
        StartCoroutine(UTILS.Requset_HttpGetData(url, jsonData =>
        {
            if (bool.TryParse(jsonData, out bool isProcess) && isProcess)
                LoadVoteInfo();
            else
                UIInteractionManager.Instance.ShowSystemPopUp("현재 투표가 진행중이지 않습니다.");
        }));
    }

    /// <summary>마우스를 올렸을 때 표시할 이름을 반환한다.</summary>
    public string Return_ObjName() => "투표함";

    /// <summary>진행 중인 투표의 주제와 항목을 받아, 투표 가능하면 투표 UI를 연다.</summary>
    private void LoadVoteInfo()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteSelectUser}";
        StartCoroutine(UTILS.Requset_HttpGetData(url, jsonData =>
        {
            Response_VoteSelect voteInfo;
            try
            {
                voteInfo = JsonUtility.FromJson<Response_VoteSelect>(JArray.Parse(jsonData).First.ToString());
            }
            catch
            {
                return;
            }

            if (voteInfo == null) return;

            // 메시지가 있으면 투표할 수 없는 상태(이미 투표함 등)이므로 안내만 띄운다
            if (!string.IsNullOrEmpty(voteInfo.msg))
            {
                UIInteractionManager.Instance.ShowSystemPopUp(voteInfo.msg);
                return;
            }

            UIInteractionManager.Instance.OpenPopUp(UI_Vote);
            UI_Vote.GetComponent<Vote_UI>().Set_VoteInfo(voteInfo);
        }));
    }
}
