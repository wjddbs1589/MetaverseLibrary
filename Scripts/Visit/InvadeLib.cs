using Photon.Pun;
using Suncheon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 다른 유저의 개인서재 방문 확인 팝업.
/// 플레이어 상호작용 메뉴에서 "서재 방문"을 고르면 열리며, 확인 시 현재 방을 나가 해당 유저의 서재 씬으로 이동한다.
/// </summary>
public class InvadeLib : MonoBehaviour
{
    [Tooltip("방문 확인 팝업")]
    [SerializeField] private GameObject UI;
    [SerializeField] private Button Ok;
    [SerializeField] private Button Cancel;
    [SerializeField] private TextMeshProUGUI text;

    /// <summary>버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        Ok.onClick.AddListener(Btn_Ok);
        Cancel.onClick.AddListener(Btn_Cancel);
    }

    /// <summary>선택한 유저의 이름으로 방문 확인 팝업을 연다.</summary>
    public void On_UI()
    {
        UI.SetActive(true);
        GameManager.Instance.VisitPlayerName = NetworkManager.Instance.user_name;
        text.text = $"'{GameManager.Instance.VisitPlayerName}'님의 개인 서재를 \r\n이용하시겠습니까?";
    }

    /// <summary>현재 방을 나가 선택한 유저의 서재로 이동한다.</summary>
    private void Btn_Ok()
    {
        UI.SetActive(false);

        PhotonNetwork.LeaveRoom();
        NetworkManager.Instance.SetSpawnerPos(SpawnerPos.개인서재, LibName.None);
        UTILS.LoadingSceneLoad("04_Myroom");
    }

    /// <summary>방문을 취소한다.</summary>
    private void Btn_Cancel()
    {
        UI.SetActive(false);
    }
}
