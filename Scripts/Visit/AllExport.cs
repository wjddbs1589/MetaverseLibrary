using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 방문자 목록의 "전체 내보내기" 버튼.
/// 확인 팝업을 거쳐 서재의 모든 방문자를 내보내고 목록 팝업을 닫는다.
/// </summary>
[RequireComponent(typeof(Button))]
public class AllExport : MonoBehaviour
{
    [Tooltip("전체 내보내기 확인 팝업")]
    [SerializeField] private GameObject UI;
    [SerializeField] private Button btn_Ok;
    [SerializeField] private Button btn_Cancel;

    private UI_LibMembers _libMembers;

    /// <summary>방문자 목록 참조를 가져오고 버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        _libMembers = GetComponentInParent<UI_LibMembers>();

        GetComponent<Button>().onClick.AddListener(() => UI.SetActive(true));
        btn_Ok.onClick.AddListener(Btn_Ok);
        btn_Cancel.onClick.AddListener(() => UI.SetActive(false));
    }

    /// <summary>모든 방문자를 내보내고 확인 팝업과 목록 팝업을 닫는다.</summary>
    private void Btn_Ok()
    {
        _libMembers.ExportPlayers();
        UI.SetActive(false);
        UIInteractionManager.Instance.ClosePopUp(transform.parent.gameObject);
    }
}
