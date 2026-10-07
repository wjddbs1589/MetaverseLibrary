using Suncheon.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 마우스를 올린 상호작용 오브젝트의 이름을 마우스 커서 아래에 따라다니며 표시한다.
/// 텍스트 내용은 ClickableHoverHandler에서 바꾸고, 이 컴포넌트는 위치만 갱신한다.
/// </summary>
public class ObjNameText : MonoBehaviour
{
    /// <summary>이름을 표시할 텍스트</summary>
    [HideInInspector] public TextMeshProUGUI uiText;

    // 커서와 텍스트 사이의 세로 간격 (Canvas 좌표)
    private const float CURSOR_OFFSET_Y = -80f;

    private Canvas _canvas;

    /// <summary>텍스트와 상위 Canvas를 캐싱한다.</summary>
    private void Awake()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        _canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>팝업 UI가 열려 있지 않으면 마우스 위치를 Canvas 좌표로 변환해 텍스트를 옮긴다.</summary>
    private void LateUpdate()
    {
        if (UIInteractionManager.Instance == null) return;
        if (UIInteractionManager.Instance.IsUIInteraction) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform, Input.mousePosition, _canvas.worldCamera, out Vector2 localPoint);

        uiText.rectTransform.localPosition = localPoint + new Vector2(0f, CURSOR_OFFSET_Y);
    }
}
