using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Kick : MonoBehaviour
{
    [SerializeField] Button btn_Ok;
    [SerializeField] Button btn_Cancel;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] UI_Interaction_Room room_ui;
    private void Awake()
    {
        btn_Ok.onClick.AddListener(() => Btn_Ok());
        btn_Cancel.onClick.AddListener(() => Btn_Cancel());
    }

    /// <summary>
    /// 내보내기 기능 실행시 text문구 설정
    /// </summary>
    /// <param name="name"></param>
    public void TextSetting(string name)
    {
        text.text = $"'{name}'님을 \r\n서재에서 내보내시겠습니까?";
    }

    /// <summary>
    /// 플레이어를 내보냈을 때 실행
    /// </summary>
    public void Btn_Ok()
    {
        room_ui.Remove_Player();
    }

    /// <summary>
    /// 플레이어 내보내기를 취소 했을 때 실행
    /// </summary>
    public void Btn_Cancel()
    {
        room_ui.Remove_Cancel();
    }

}
