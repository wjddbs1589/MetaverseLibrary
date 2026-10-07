using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.WebData;
using System;
using UnityEngine;

/// <summary>
/// 서버에서 업적 달성 정보를 받아 개인서재 진열장에 트로피를 놓는다.
/// 달성한 트로피만 진열장 앞칸부터 순서대로 채우며, 전체 달성("all")이면 모든 트로피를 놓는다.
/// </summary>
public class SetTrophy : MonoBehaviour
{
    // 서버 보상 코드. 배열 순서는 TrophyPrefabs 순서와 같다.
    // game = 미니게임, lib = 도서관 방문, comm = 방명록·추천도서, treasure = 보물 찾기
    private static readonly string[] RewardCodes = { "game", "lib", "comm", "treasure" };

    // 모든 업적을 달성했을 때의 보상 코드
    private const string ALL_COMPLETE_CODE = "all";

    [Tooltip("트로피 프리팹 (RewardCodes 순서)")]
    [SerializeField] private GameObject[] TrophyPrefabs;

    [Tooltip("트로피를 놓을 진열장 칸 (앞칸부터 순서대로 채움)")]
    [SerializeField] private Transform[] TrophyCases_Obj;

    /// <summary>업적 정보를 요청한다.</summary>
    private void Start()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userRewardInfo}";
        StartCoroutine(UTILS.Requset_HttpGetData(url, jsonData =>
        {
            JArray jArray;
            try { jArray = JArray.Parse(jsonData); }
            catch { return; }

            PlaceTrophies(GetCompletedRewards(jArray));
        }));
    }

    /// <summary>서버 응답에서 트로피별 달성 여부를 계산한다.</summary>
    private bool[] GetCompletedRewards(JArray jArray)
    {
        bool[] completed = new bool[TrophyPrefabs.Length];

        foreach (JToken token in jArray)
        {
            string reward = JsonUtility.FromJson<Response_RewardInfo>(token.ToString()).reward;

            // 전체 달성이면 모든 트로피를 달성으로 처리한다
            if (reward == ALL_COMPLETE_CODE)
            {
                for (int i = 0; i < completed.Length; i++) completed[i] = true;
                break;
            }

            int index = Array.IndexOf(RewardCodes, reward);
            if (index >= 0 && index < completed.Length)
                completed[index] = true;
        }

        return completed;
    }

    /// <summary>달성한 트로피를 진열장 앞칸부터 순서대로 생성한다.</summary>
    private void PlaceTrophies(bool[] completed)
    {
        int caseIndex = 0;

        for (int i = 0; i < TrophyPrefabs.Length; i++)
        {
            if (!completed[i]) continue;
            Instantiate(TrophyPrefabs[i], TrophyCases_Obj[caseIndex++]);
        }
    }
}
