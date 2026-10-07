using Suncheon;
using System.Collections;
using UnityEngine;

/// <summary>
/// 개인서재 입장 시 카메라를 서재 전용 고정 카메라로 전환한다.
/// 플레이어 이름표가 서재 카메라를 바라보도록 하고, 서재 안의 오브젝트 상호작용은 FurnitureMove가 맡도록
/// 플레이어의 PlayerObjInteraction을 끈다.
/// </summary>
public class RoomCameraSetting : MonoBehaviour
{
    [Tooltip("서재 전용 카메라 오브젝트")]
    [SerializeField] private GameObject RoomCamera;

    /// <summary>서재 입장 시 카메라를 설정한다.</summary>
    private void Awake()
    {
        Off_PlayerInteractive();
    }

    /// <summary>
    /// 서재 카메라 설정을 적용한다. 인테리어 모드를 끝낼 때도 FurnitureManager에서 다시 호출한다.
    /// 내 플레이어가 아직 생성되지 않았으면 생성될 때까지 기다린다.
    /// </summary>
    public void Off_PlayerInteractive()
    {
        StartCoroutine(ApplyWhenPlayerReady());
    }

    /// <summary>내 플레이어가 생성되면 서재 카메라를 켜고 이름표와 상호작용 설정을 바꾼다.</summary>
    private IEnumerator ApplyWhenPlayerReady()
    {
        while (NetworkManager.Instance.Go_Player == null)
            yield return null;

        GameObject player = NetworkManager.Instance.Go_Player;
        RoomCamera.SetActive(true);

        PlayerNameController nameController = player.GetComponentInChildren<PlayerNameController>();
        nameController.roomCamera = RoomCamera.GetComponent<Camera>();
        nameController.InMyroom = true;

        player.GetComponent<PlayerObjInteraction>().enabled = false;
    }
}
