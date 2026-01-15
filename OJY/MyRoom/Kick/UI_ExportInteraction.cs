using Photon.Pun;
using Suncheon.UI;
using Suncheon;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Suncheon.Player;

public class UI_ExportInteraction : MonoBehaviour
{
    #region UnityButton   
    [SerializeField] private Button btn_export;     
    #endregion

    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private TMP_Text text_ChatMode;

    [SerializeField] private TMP_Text text_Name;

    [SerializeField] private float rayCastMaxDistance = 100.0f;

    [SerializeField] private PhotonView interactionPV;

    private void Awake()
    {
        btn_export.onClick.AddListener(() => Btn_export());
    }

    private void Update()
    {
        Click_interactive();
    }

    /// <summary>
    /// 클릭했을 떄 실행될 상호작용 기능
    /// </summary>
    void Click_interactive()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (playerManager == null)
            {
                playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
            }

            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayCastMaxDistance, LayerMask.GetMask("Player")))
            {
                interactionPV = hit.collider.gameObject.GetPhotonView();
                string hitPlayerName = interactionPV.Controller.NickName;
                //if (hitPlayerName == PhotonNetwork.NickName) return;
                Vector3 canvasPos = Camera.main.WorldToScreenPoint(hit.point);

                UIInteractionManager.Instance.PlayerInteractionOn(hitPlayerName, canvasPos);

                Load_UserID();
            }
            return;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = EventSystem.current.currentSelectedGameObject;
            if (obj == null)
            {
                UIInteractionManager.Instance.PlayerInteractionOff();
            }
        }
    }
    
    /// <summary>
    /// 이름을 파라미터 값으로 설정
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        text_Name.text = name;
    }

    /// <summary>
    /// 닉네임에 일치하는 유저 ID를 가져옴
    /// </summary>
    void Load_UserID()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userIdCheck}", $"nickname={interactionPV.Controller.NickName}",
           (jsonData) =>
           {
               UTILS.Log($"Load_UserID : {jsonData}");

               Set_UserID(jsonData);
           }
           ));
    }

    /// <summary>
    /// 불러온 유저 ID를 저장
    /// </summary>
    /// <param name="userID"></param>
    void Set_UserID(string userID)
    {
        NetworkManager.Instance.user_id = userID;
        NetworkManager.Instance.user_name = interactionPV.Controller.NickName;
    }

    /// <summary>
    /// name에 맞는 플레이어를 룸에서 내보냄
    /// </summary>
    private void Btn_export()
    {
        interactionPV.RPC("RPCClubKickPlayer", RpcTarget.Others, name);
    }

}
