using Photon.Pun;
using Suncheon;
using Suncheon.Player;
using UnityEngine;

public class PlayerNameController : MonoBehaviour
{
    //플레이어의 카메라 주시
    [SerializeField] Camera PlayerCam;
    public Camera PhotoZoneCam;
    bool LookPlayer = true;

    [HideInInspector] public Camera roomCamera;
    public bool InMyroom;

    [SerializeField] PhotonView pv;

    private void Update()
    {
        //개인서재의 전용 카메라 응시
        if (InMyroom)
        {
            if(roomCamera != null)
            {
                transform.LookAt(roomCamera.transform);
            }
        }
        else
        {
            if (LookPlayer)
            {
                if (pv.IsMine)
                {
                    transform.LookAt(PlayerCam.transform);
                }
                else
                {
                    transform.LookAt(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().tr_LoockAtPos);
                }
            }
            else
            {
                if (pv.IsMine)
                {
                    transform.LookAt(PhotoZoneCam.transform);
                }
                else
                {
                    transform.LookAt(NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().tr_LoockAtPos);
                }
            }
        }
    }

    public void Set_PhotoZoneCam(Camera cam)
    {
        PhotoZoneCam = cam;
    }

    /// <summary>
    /// 플레이어의 캠을 보는지 설정
    /// </summary>
    /// <param name="look">true이면 포토존을 사용하지 않을 때</param>
    public void Look_PlayerCam(bool look)
    {
        LookPlayer = look;
    }
}
