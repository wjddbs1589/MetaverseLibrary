using Photon.Pun;
using Suncheon;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 다른 사람의 서재에 입장했을 때 방문 인원을 확인한다.
/// 나를 제외한 인원이 최대 인원 이상이면 안내 팝업을 띄우고, 확인을 누르면 광장으로 돌려보낸다.
/// </summary>
public class RoomPlayerCheck : MonoBehaviour
{
    [Tooltip("서재 주인을 포함해 나를 제외하고 머물 수 있는 최대 인원")]
    public int maxPlayerCount = 0;

    [Header("인원 초과 안내 팝업")]
    [SerializeField] private GameObject PopUp;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button btn;

    // Photon 방 접속 정보가 갱신될 때까지 기다리는 시간 (초)
    private const float CHECK_DELAY = 0.2f;

    /// <summary>확인 버튼을 연결하고 잠시 뒤 인원을 확인한다.</summary>
    private void Awake()
    {
        btn.onClick.AddListener(Close_Btn);
        StartCoroutine(CheckAfterDelay());
    }

    /// <summary>방 접속 정보가 갱신된 뒤 인원을 확인한다.</summary>
    private IEnumerator CheckAfterDelay()
    {
        yield return new WaitForSeconds(CHECK_DELAY);
        Check_PeopleCount();
    }

    /// <summary>다른 사람의 서재이고 인원이 가득 찼으면 상호작용을 막고 안내 팝업을 띄운다.</summary>
    private void Check_PeopleCount()
    {
        if (NetworkManager.Instance.Check_MyRoom()) return;
        if (PhotonNetwork.PlayerListOthers.Length < maxPlayerCount) return;

        InteractionManager.Inst.Ray_Off();
        text.text = $"입장 인원 초과로\r\n '{NetworkManager.Instance.user_name}'님의 개인 서재를 \r\n이용하실 수 없습니다.";
        PopUp.SetActive(true);
    }

    /// <summary>방에서 나가 광장 씬으로 이동한다.</summary>
    private void Close_Btn()
    {
        InteractionManager.Inst.Ray_On();
        NetworkManager.Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
        NetworkManager.Instance.OnLeaveRoom();
        UTILS.LoadingSceneLoad("02_Garden_Scene");
    }
}
