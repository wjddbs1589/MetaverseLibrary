using Suncheon;
using UnityEngine;

/// <summary>
/// 플레이어가 별자리 동굴 안에 있는지 확인하는 클래스
/// </summary>
public class ML_PlayerInCheck : MonoBehaviour
{
    Byul_Interactive[] byul_Interactive;
    CameraManager cameraManager;

    private void Awake()
    {
        byul_Interactive = GetComponentsInChildren<Byul_Interactive>();
        
    }
    private void OnTriggerEnter(Collider other)
    {
        SetBool_PlayerIn(true);        

        if (NetworkManager.Instance.Go_Player == other.gameObject)
        {
            cameraManager = other.GetComponentInChildren<CameraManager>();
            cameraManager.SetThirdPerson(false); //좁은곳에 들어가므로 3인칭 해제
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SetBool_PlayerIn(false);        

        if (NetworkManager.Instance.Go_Player == other.gameObject)
        {
            cameraManager = other.GetComponentInChildren<CameraManager>();
            cameraManager.SetThirdPerson(true);
        }
        
    }

    void SetBool_PlayerIn(bool have)
    {
        foreach (Byul_Interactive byul in byul_Interactive)
        {
            byul.PlayerIN = have;
        }
    }
}
