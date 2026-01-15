using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suncheon.Player;
using Photon.Pun;

namespace Suncheon.UI
{
    public class UI_LibMembers : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Transform content_libMembers;
        [SerializeField] private GameObject libMemeberObject;

        [SerializeField] List<string> playerNickNameList = new List<string>();

        /// <summary>
        /// 팝업 활성화
        /// </summary>
        public void ShowPopUp()
        {
            UIInteractionManager.Instance.OpenPopUp(gameObject);

            playerNickNameList.Clear(); //현재 목록 초기화 

            if (playerManager == null)
            {
                playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            }

            StartCoroutine(SetlibMembers());
        }


        IEnumerator SetlibMembers()
        {
            playerNickNameList.Clear();

            yield return new WaitForSeconds(0.5f);

            PlayerListUpdate();

            foreach (string nick in playerNickNameList)
            {
                GameObject go = Instantiate(libMemeberObject, content_libMembers);

                go.GetComponent<LibMemberObject>().SetObject(nick);
            }
        }

        List<PlayerPhotonManager> photonManagers;

        public void ExportPlayers()
        {
            PhotonView pv = NetworkManager.Instance.Go_Player.GetPhotonView();
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
            {
                pv.RPC("RPCLibKickPlayer", RpcTarget.Others, player.NickName);
            }
            StartCoroutine(SetlibMembers());
        }

        public void PlayerListUpdate()
        {
            foreach (LibMemberObject child in content_libMembers.GetComponentsInChildren<LibMemberObject>())
            {
                Destroy(child.gameObject);
            }

            foreach (var player in PhotonNetwork.PlayerListOthers)
            {
                playerNickNameList.Add(player.NickName);
            }
        }

        public void Reset_PlayerList()
        {
            StartCoroutine(SetlibMembers());
        }
    }
}