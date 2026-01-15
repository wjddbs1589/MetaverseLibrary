using Photon.Pun;
using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_Interaction_Room : UI_PlayerInteraction
{
    [SerializeField] Button btn_KickOff;

    [Header("내쫓기 UI")]
    [SerializeField] GameObject KickUIObj;
    [SerializeField] UI_Kick KickUI;

    PhotonView pv;
    UI_LibMembers uI_LibMembers;
    protected override void Awake()
    {
        uI_LibMembers = GetComponentInParent<UI_LibMembers>();
        btn_CutOff.onClick.AddListener(() => Btn_CutOff());
        btn_KickOff.onClick.AddListener(() => Btn_Kick());
    }

    /// <summary>
    /// 차단버튼을 눌렀을 때, 
    /// 1. 대상의 닉네임 정보를 가져옴, 2. 닉네임을 이용해 대상을 쫓아냄, 3. 방문인원 갱신
    /// </summary>
    void Btn_CutOff()
    {
        Kick_Check();
    }

    void Btn_Kick()
    {
        Kick_Check();
    }

    void Kick_Check()
    {
        KickUIObj.SetActive(true);
        KickUI.TextSetting(NetworkManager.Instance.user_name);
    }

    /// <summary>
    /// 선택한 플레이어를 내보냄
    /// </summary>
    public void Remove_Player()
    {
        pv = NetworkManager.Instance.Go_Player.GetPhotonView();
        pv.RPC("RPCLibKickPlayer", RpcTarget.Others, NetworkManager.Instance.user_name);

        uI_LibMembers.Reset_PlayerList();

        KickUIObj.SetActive(false);
    }

    public void Remove_Cancel()
    {
        KickUIObj.SetActive(false);
    }
}
