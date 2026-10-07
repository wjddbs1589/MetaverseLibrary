using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>방문자 내보내기 확인 팝업.</summary>
public class UI_Kick : MonoBehaviour
{
    [SerializeField] private Button btn_Ok;
    [SerializeField] private Button btn_Cancel;
    [SerializeField] private TextMeshProUGUI text;

    [Tooltip("내보내기를 실행할 상호작용 메뉴")]
    [SerializeField] private UI_Interaction_Room room_ui;

    /// <summary>버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        btn_Ok.onClick.AddListener(Btn_Ok);
        btn_Cancel.onClick.AddListener(Btn_Cancel);
    }

    /// <summary>내보낼 방문자의 이름으로 안내 문구를 설정한다.</summary>
    public void TextSetting(string name)
    {
        text.text = $"'{name}'님을 \r\n서재에서 내보내시겠습니까?";
    }

    /// <summary>확인을 누르면 방문자를 내보낸다.</summary>
    public void Btn_Ok()
    {
        room_ui.Remove_Player();
    }

    /// <summary>취소를 누르면 팝업을 닫는다.</summary>
    public void Btn_Cancel()
    {
        room_ui.Remove_Cancel();
    }
}
