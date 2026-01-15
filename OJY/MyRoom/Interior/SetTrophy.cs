using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.WebData;
using UnityEngine;

public class SetTrophy : MonoBehaviour
{
    [SerializeField] GameObject[] TrophyPrefabs;
    [SerializeField] Transform[] TrophyCases_Obj;

    int trophyIndex = 0;
    int caseIndex = 0;

    AchievementCompInfo info = new AchievementCompInfo();

    private void Start()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userRewardInfo}", (jsonData) =>
        {
            JArray jArray = null;
            try { jArray = JArray.Parse(jsonData); }
            catch { jArray = null; }
            if (jArray == null) return;

            SetMissionComp(jArray);
        }));
    }

    private void SetMissionComp(JArray jArray)
    {
        foreach (var jobj in jArray)
        {
            Response_RewardInfo reawrdData = JsonUtility.FromJson<Response_RewardInfo>(jobj.ToString());

            //이미 전체 완료가 되어 있으면 
            if (reawrdData.reward.Equals("all"))
            {
                info.isAllComp = true;
                Set_Trophy();
                return;
            }

            //되어있는 트로피만 정보 저장
            if (reawrdData.reward.Equals("game")) //미니게임
            {
                UTILS.Log($"{info.isMiniGameComp}");
                info.isMiniGameComp = true;
            }
            else if (reawrdData.reward.Equals("lib")) //도서관 방문
            {
                UTILS.Log($"{info.isLibVisiteComp}");
                info.isLibVisiteComp = true;
            } 
            else if (reawrdData.reward.Equals("comm")) //방명록, 추천도서
            {
                UTILS.Log($"{info.isMyRoomComp}");
                info.isMyRoomComp = true;
            }
            else if (reawrdData.reward.Equals("treasure")) //보물 찾기
            {
                UTILS.Log($"{info.isTreasureComp}");
                info.isTreasureComp = true;
            }
        }

        Set_Trophy();
    }

    /// <summary>
    /// 순차적으로 트로피 생성
    /// </summary>
    void Set_Trophy() 
    {
        if (info.isAllComp)
        {
            for (int i = 0; i < TrophyPrefabs.Length; i++)
            {
                Instantiate(TrophyPrefabs[trophyIndex], TrophyCases_Obj[caseIndex]);
                caseIndex++;
                trophyIndex++;
            }
        }
        else
        {
            for (int i = 0; i < TrophyPrefabs.Length; i++)
            {
                if (Comp_Check(i))
                {
                    Instantiate(TrophyPrefabs[trophyIndex], TrophyCases_Obj[caseIndex]);
                    caseIndex++;
                }

                trophyIndex++;
            }
        }
    }

    /// <summary>
    /// 미션이 완료되었는지 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool Comp_Check(int index)
    {
        bool result = false;
        switch (index)
        {
            case 0:
                result = info.isMiniGameComp;
                UTILS.Log($"게임 : {info.isMiniGameComp}");
                return result;
            case 1:
                result = info.isLibVisiteComp;
                UTILS.Log($"도서관방문 : {info.isLibVisiteComp}");
                return result;
            case 2:
                result = info.isMyRoomComp;
                UTILS.Log($"방명록 : {info.isMyRoomComp}");
                return result;
            case 3:
                result = info.isTreasureComp;
                UTILS.Log($"보물 : {info.isTreasureComp}");
                return result;
            default:
                return result;
        }
    }
}
