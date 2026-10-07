using Photon.Pun;
using Suncheon;
using Suncheon.Player;
using UnityEngine;

/// <summary>
/// 플레이어 머리 위 이름표가 항상 현재 시점의 카메라를 바라보도록 회전시킨다.
///
/// 바라볼 대상:
///   개인서재          → 서재 전용 카메라
///   내 이름표         → 내 플레이어 카메라 (포토존 사용 중이면 포토존 카메라)
///   다른 플레이어 이름표 → 내 플레이어의 시점 기준점 (내 화면에서 읽히도록)
/// </summary>
public class PlayerNameController : MonoBehaviour
{
    [Tooltip("이 이름표 주인의 플레이어 카메라")]
    [SerializeField] private Camera PlayerCam;

    [Tooltip("이름표 주인의 PhotonView (내 플레이어인지 판별)")]
    [SerializeField] private PhotonView pv;

    /// <summary>포토존 사용 중 바라볼 카메라</summary>
    [HideInInspector] public Camera PhotoZoneCam;

    /// <summary>개인서재 전용 카메라 (RoomCameraSetting에서 설정)</summary>
    [HideInInspector] public Camera roomCamera;

    /// <summary>개인서재 안에 있으면 true</summary>
    [HideInInspector] public bool InMyroom;

    // false면 포토존 카메라를 바라본다
    private bool _lookPlayerCam = true;

    // 다른 플레이어 이름표가 바라볼 내 플레이어의 시점 기준점
    private Transform _localViewPoint;

    /// <summary>카메라 이동이 반영된 뒤 이름표가 대상을 바라보게 한다.</summary>
    private void LateUpdate()
    {
        Transform target = GetLookTarget();
        if (target != null)
            transform.LookAt(target);
    }

    /// <summary>현재 상황에 맞는 바라볼 대상을 반환한다.</summary>
    private Transform GetLookTarget()
    {
        if (InMyroom)
            return roomCamera != null ? roomCamera.transform : null;

        if (pv.IsMine)
        {
            Camera cam = _lookPlayerCam ? PlayerCam : PhotoZoneCam;
            return cam != null ? cam.transform : null;
        }

        // 다른 플레이어의 이름표는 내 화면 기준으로 보여야 하므로 내 플레이어의 시점을 바라본다
        if (_localViewPoint == null && NetworkManager.Instance.Go_Player != null)
            _localViewPoint = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().tr_LoockAtPos;

        return _localViewPoint;
    }

    /// <summary>포토존 카메라를 설정한다. (null이면 해제)</summary>
    public void Set_PhotoZoneCam(Camera cam)
    {
        PhotoZoneCam = cam;
    }

    /// <summary>true면 플레이어 카메라를, false면 포토존 카메라를 바라본다.</summary>
    public void Look_PlayerCam(bool look)
    {
        _lookPlayerCam = look;
    }
}
