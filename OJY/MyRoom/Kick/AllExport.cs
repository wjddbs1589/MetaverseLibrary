using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class AllExport : MonoBehaviour
{
    Button thisBtn;

    [SerializeField] GameObject UI;
    [SerializeField] Button btn_Ok;
    [SerializeField] Button btn_Cancel;

    UI_LibMembers libMembers;

    private void Awake()
    {
        libMembers = GetComponentInParent<UI_LibMembers>();
        thisBtn=GetComponent<Button>();
        thisBtn.onClick.AddListener(()=> UI_On());
        btn_Ok.onClick.AddListener(() => Btn_Ok());
        btn_Cancel.onClick.AddListener(() => Btn_Cancel());
    }

    /// <summary>
    /// 전부 내쫓기 UI 활성화
    /// </summary>
    void UI_On()
    {
        UI.SetActive(true);
    }

    /// <summary>
    /// 모든 플레이어를 내보냄
    /// </summary>
    void Btn_Ok()
    {
        //전부 내쫓는 코드 추가
        libMembers.ExportPlayers();        
        UI.SetActive(false);
        UIInteractionManager.Instance.ClosePopUp(transform.parent.gameObject);
    }

    /// <summary>
    /// UI닫기
    /// </summary>
    void Btn_Cancel()
    {
        UI.SetActive(false);
    }
}
