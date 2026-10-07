using Suncheon;
using Suncheon.UI;
using UnityEngine;

/// <summary>
/// 광장·도서관에서 내 플레이어의 오브젝트 상호작용을 처리한다.
/// 매 프레임 마우스(터치) 위치로 레이를 쏴서 IClickable 대상을 찾고, 강조와 클릭 판정은 ClickableHoverHandler에 맡긴다.
///
/// 상호작용을 막는 경우:
///   - 미니게임 진행 중
///   - 팝업 UI 사용 중 (UsingUI) 또는 UI 버튼 위에 마우스가 있을 때 (ChooseUI)
///   - 대상이 MAX_DISTANCE보다 멀 때
/// </summary>
public class PlayerObjInteraction : MonoBehaviour
{
    [Tooltip("레이를 쏠 플레이어 카메라")]
    [SerializeField] private Camera PlayerCamera;

    [Tooltip("마우스를 따라다니는 오브젝트 이름 텍스트")]
    [SerializeField] private ObjNameText nameText;

    /// <summary>팝업 UI를 사용 중이면 true (InteractionManager에서 변경)</summary>
    public bool UsingUI = false;

    /// <summary>마우스가 UI 버튼 위에 있으면 true (ButtonHoverDetector에서 변경)</summary>
    public bool ChooseUI = false;

    // 상호작용 가능한 최대 거리 (m)
    private const float MAX_DISTANCE = 15f;

    private UIInteractionManager _uiManager;
    private ClickableHoverHandler _hoverHandler;

    /// <summary>UI 매니저와 강조·클릭 처리기를 준비한다.</summary>
    private void Awake()
    {
        _uiManager = FindAnyObjectByType<UIInteractionManager>();
        _hoverHandler = new ClickableHoverHandler(nameText);
    }

    /// <summary>미니게임 중이 아니고 내 플레이어일 때만 상호작용 대상을 탐색한다.</summary>
    private void Update()
    {
        if (_uiManager.Play_Game) return;
        if (NetworkManager.Instance.Go_Player != gameObject) return;

        DetectClickable();
    }

    /// <summary>마우스 위치의 오브젝트를 찾아 강조하고, 클릭이 확정되면 기능을 실행한다.</summary>
    private void DetectClickable()
    {
        // UI를 쓰는 중이거나 UI 버튼 위에 있으면 뒤에 있는 오브젝트가 클릭되지 않도록 한다
        if (UsingUI || ChooseUI)
        {
            _hoverHandler.Clear();
            return;
        }

        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit) || IsTooFar(hit.collider.transform))
        {
            _hoverHandler.Clear();
            return;
        }

        GameObject hitObj = hit.collider.gameObject;
        _hoverHandler.UpdateHover(hitObj);

        if (_hoverHandler.TryGetClickTarget(hitObj, out IClickable target))
        {
            target.OnClick();
            _hoverHandler.Clear();
        }
    }

    /// <summary>대상이 상호작용 가능 거리보다 멀면 true를 반환한다.</summary>
    private bool IsTooFar(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) > MAX_DISTANCE;
    }

    /// <summary>강조와 이름 표시를 지운다.</summary>
    public void Clear()
    {
        _hoverHandler.Clear();
    }
}
