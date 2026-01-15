using Photon.Pun;
using Suncheon;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerCheck : MonoBehaviour
{
    public int maxPlayerCount = 0;
    public int currentPlayerCount = 0;

    [SerializeField] GameObject PopUp;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button btn;


    private void Awake()
    {
        btn.onClick.AddListener(() => Close_Btn());
        StartCoroutine(player_Check());
    }

    IEnumerator player_Check()
    {
        yield return new WaitForSeconds(.2f);
        Check_PeopleCount();
    }
    void Check_PeopleCount()
    {
        //내 서재가 아닐때
        if (!NetworkManager.Instance.Check_MyRoom())
        {
            //나를 제외한 유저의 수가 최대 수와 같거나 크면 광장으로 이동
            if (maxPlayerCount <= PhotonNetwork.PlayerListOthers.Length)
            {
                InteractionManager.Inst.Ray_Off();                
                text.text = $"입장 인원 초과로\r\n '{NetworkManager.Instance.user_name}'님의 개인 서재를 \r\n이용하실 수 없습니다.";
                PopUp.SetActive(true);
            }
            else
            {
                currentPlayerCount++;
            }
        }
    }

    void Close_Btn()
    {
        InteractionManager.Inst.Ray_On();
        NetworkManager.Instance.SetSpawnerPos(SpawnerPos.오천그린광장);
        NetworkManager.Instance.OnLeaveRoom();
        UTILS.LoadingSceneLoad("02_Garden_Scene");
    }
}
