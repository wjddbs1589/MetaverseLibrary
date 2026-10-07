using Photon.Pun;
using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 내 서재에서 방문자를 선택했을 때 열리는 서재 주인용 상호작용 메뉴.
/// 차단·내보내기를 누르면 확인 팝업(UI_Kick)을 거쳐 대상에게 내보내기 RPC를 보낸다.
/// </summary>
public class UI_Interaction_Room : UI_PlayerInteraction
{
    [Tooltip("내보내기 버튼")]
    [SerializeField] private Button btn_KickOff;

    [Header("내보내기 확인 팝업")]
    [SerializeField] private GameObject KickUIObj;
    [SerializeField] private UI_Kick KickUI;

    private UI_LibMembers _libMembers;

    /// <summary>방문자 목록 참조를 가져오고 버튼 이벤트를 연결한다.</summary>
    protected override void Awake()
    {
        _libMembers = GetComponentInParent<UI_LibMembers>();

        // 서재에서는 차단도 내보내기로 처리한다
        btn_CutOff.onClick.AddListener(Kick_Check);
        btn_KickOff.onClick.AddListener(Kick_Check);
    }

    /// <summary>선택한 방문자의 이름으로 내보내기 확인 팝업을 연다.</summary>
    private void Kick_Check()
    {
        KickUIObj.SetActive(true);
        KickUI.TextSetting(NetworkManager.Instance.user_name);
    }

    /// <summary>선택한 방문자에게 내보내기 RPC를 보내고 방문자 목록을 갱신한다.</summary>
    public void Remove_Player()
    {
        PhotonView pv = NetworkManager.Instance.Go_Player.GetPhotonView();
        pv.RPC("RPCLibKickPlayer", RpcTarget.Others, NetworkManager.Instance.user_name);

        _libMembers.Reset_PlayerList();
        KickUIObj.SetActive(false);
    }

    /// <summary>내보내기를 취소하고 팝업을 닫는다.</summary>
    public void Remove_Cancel()
    {
        KickUIObj.SetActive(false);
    }
}
