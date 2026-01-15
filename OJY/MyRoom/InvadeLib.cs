using Photon.Pun;
using Suncheon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvadeLib : MonoBehaviour
{
    [SerializeField] GameObject UI;
    [SerializeField] Button Ok;
    [SerializeField] Button Cancel;
    [SerializeField] TextMeshProUGUI text;

    private void Awake()
    {
        Ok.onClick.AddListener(() => Btn_Ok());
        Cancel.onClick.AddListener(() => Btn_Cancel());
    }

    /// <summary>
    /// 개인서재 이용 UI 실행
    /// </summary>
    public void On_UI()
    {
        UI.SetActive(true);
        GameManager.Instance.VisitPlayerName = NetworkManager.Instance.user_name;
        text.text = $"'{GameManager.Instance.VisitPlayerName}'님의 개인 서재를 \r\n이용하시겠습니까?";
    }

    /// <summary>
    /// 타인의 개인서재 입장
    /// </summary>
    void Btn_Ok() 
    {
        UI.SetActive(false);

        PhotonNetwork.LeaveRoom();
        NetworkManager.Instance.SetSpawnerPos(SpawnerPos.개인서재, LibName.None);
        UTILS.LoadingSceneLoad("04_Myroom");
    }

    /// <summary>
    /// 타인의 개인서재 입장 취소
    /// </summary>
    void Btn_Cancel()
    {
        UI.SetActive(false);
    }

}
