using Suncheon.UI;
using Suncheon;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Suncheon.WebData;

public class Vote : MonoBehaviour, Clickable
{
    [SerializeField] GameObject UI_Vote;

    public void OnClick()
    {
        if (GameManager.Instance.IsGuest)
        {
            UIInteractionManager.Instance.ShowSystemPopUp("Guest 계정은 해당 서비스를 이용할 수 없습니다.");
            return;
        }
        else
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteProcess}", (jsonData) =>
            {
                bool isProcess = bool.Parse(jsonData);

                if (isProcess)
                {
                    //투표 주제에 대한 설명 가져옴
                    StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteSelectUser}", (jsonData) =>
                    {
                        JArray jArray = null;
                        try
                        {
                            jArray = JArray.Parse(jsonData);
                        }
                        catch
                        {
                            jArray = null;
                        }

                        if (jArray == null) return;

                        Response_VoteSelect response_VoteSelect = null;
                        try
                        {
                            response_VoteSelect = JsonUtility.FromJson<Response_VoteSelect>(jArray.First.ToString());
                        }
                        catch
                        {
                            response_VoteSelect = null;
                        }

                        if (response_VoteSelect == null) return;

                        if (string.IsNullOrEmpty(response_VoteSelect.msg))
                        {
                            UIInteractionManager.Instance.OpenPopUp(UI_Vote);
                            UI_Vote.GetComponent<Vote_UI>().Set_VoteInfo(response_VoteSelect);
                        }
                        else
                        {
                            UIInteractionManager.Instance.ShowSystemPopUp($"{response_VoteSelect.msg}");
                        }
                    }));

                }
                else
                {
                    UIInteractionManager.Instance.ShowSystemPopUp("현재 투표가 진행중이지 않습니다.");
                }
            }));
        }
    }

    public string Return_ObjName()
    {
        return "투표함";
    }
}
