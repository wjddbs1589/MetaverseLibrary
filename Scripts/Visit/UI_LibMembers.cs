using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon.UI
{
    /// <summary>
    /// 내 서재에 들어와 있는 방문자 목록 팝업.
    /// Photon 방의 다른 플레이어 닉네임으로 목록을 만들고, 서재 주인은 방문자를 모두 내보낼 수 있다.
    /// </summary>
    public class UI_LibMembers : MonoBehaviour
    {
        [Tooltip("방문자 항목을 생성할 부모")]
        [SerializeField] private Transform content_libMembers;

        [Tooltip("방문자 항목 프리팹 (LibMemberObject)")]
        [SerializeField] private GameObject libMemeberObject;

        // Photon 방의 플레이어 목록이 갱신될 때까지 기다리는 시간 (초)
        private const float REFRESH_DELAY = 0.5f;

        // 현재 방문자 닉네임
        private readonly List<string> _playerNickNames = new List<string>();

        /// <summary>팝업을 열고 방문자 목록을 갱신한다.</summary>
        public void ShowPopUp()
        {
            UIInteractionManager.Instance.OpenPopUp(gameObject);
            Reset_PlayerList();
        }

        /// <summary>방문자 목록을 다시 만든다. 내보내기 후에도 호출한다.</summary>
        public void Reset_PlayerList()
        {
            StartCoroutine(RefreshMembers());
        }

        /// <summary>모든 방문자에게 내보내기 RPC를 보내고 목록을 갱신한다.</summary>
        public void ExportPlayers()
        {
            PhotonView pv = NetworkManager.Instance.Go_Player.GetPhotonView();

            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                pv.RPC("RPCLibKickPlayer", RpcTarget.Others, player.NickName);

            Reset_PlayerList();
        }

        /// <summary>Photon 방 정보가 갱신될 때까지 잠시 기다린 뒤 방문자 항목을 다시 생성한다.</summary>
        private IEnumerator RefreshMembers()
        {
            yield return new WaitForSeconds(REFRESH_DELAY);

            PlayerListUpdate();

            foreach (string nickName in _playerNickNames)
            {
                GameObject item = Instantiate(libMemeberObject, content_libMembers);
                item.GetComponent<LibMemberObject>().SetObject(nickName);
            }
        }

        /// <summary>기존 항목을 지우고 현재 방의 다른 플레이어 닉네임을 다시 모은다.</summary>
        public void PlayerListUpdate()
        {
            foreach (LibMemberObject child in content_libMembers.GetComponentsInChildren<LibMemberObject>())
                Destroy(child.gameObject);

            _playerNickNames.Clear();
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                _playerNickNames.Add(player.NickName);
        }
    }
}
