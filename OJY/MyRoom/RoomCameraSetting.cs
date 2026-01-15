using Suncheon;
using System.Collections;
using UnityEngine;

//개인서재 입장시 카메라 세팅
public class RoomCameraSetting : MonoBehaviour
{
    [SerializeField] GameObject RoomCamera;
    PlayerNameController nameController;
    PlayerObjInteraction playerObjInteraction;
    private void Awake()
    {
        Off_PlayerInteractive();
    }

    public void Off_PlayerInteractive()
    {
        StartCoroutine(CameraCo());
    }
    IEnumerator CameraCo()
    {
        while (true)
        {
            if (NetworkManager.Instance.Go_Player != null)
            {   
                RoomCamera.SetActive(true);

                nameController = NetworkManager.Instance.Go_Player.GetComponentInChildren<PlayerNameController>();
                nameController.roomCamera = RoomCamera.GetComponent<Camera>();
                nameController.InMyroom = true;

                playerObjInteraction = NetworkManager.Instance.Go_Player.GetComponent<PlayerObjInteraction>();
                playerObjInteraction.enabled = false;

                break;
            }
            yield return null;
        }
    }

}
