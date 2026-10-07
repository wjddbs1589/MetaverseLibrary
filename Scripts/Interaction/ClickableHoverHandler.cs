using UnityEngine;

/// <summary>
/// IClickable 오브젝트의 마우스 오버 강조와 클릭 판정을 처리하는 공통 로직.
/// PlayerObjInteraction(광장·도서관)과 FurnitureMove(개인서재)가 같은 방식으로 상호작용하도록 함께 사용한다.
///
/// 오클릭 방지:
///   누른 시점의 대상과 뗀 시점의 대상이 같을 때만 클릭으로 인정한다.
///   화면을 드래그해 시점을 돌리다 우연히 오브젝트 위에서 손을 떼도 클릭되지 않는다.
/// </summary>
public class ClickableHoverHandler
{
    // 마우스를 따라다니는 오브젝트 이름 텍스트
    private readonly ObjNameText _nameText;

    // 현재 켜져 있는 아웃라인
    private Outline _outline;

    // 현재 마우스가 가리키는 클릭 가능 대상
    private IClickable _hovered;

    // 마우스를 누른 시점의 대상
    private GameObject _pressedObj;

    /// <summary>이름을 표시할 텍스트를 받아 생성한다.</summary>
    public ClickableHoverHandler(ObjNameText nameText)
    {
        _nameText = nameText;
    }

    /// <summary>레이에 맞은 오브젝트가 클릭 가능하면 아웃라인과 이름을 표시하고, 아니면 숨긴다.</summary>
    public void UpdateHover(GameObject hitObj)
    {
        _hovered = hitObj.GetComponent<IClickable>();

        if (_hovered != null)
        {
            Outline newOutline = hitObj.GetComponent<Outline>();

            // 다른 대상으로 옮겨 가면 이전 아웃라인을 끈다
            if (_outline != null && _outline != newOutline)
                _outline.enabled = false;

            _outline = newOutline;
            if (_outline != null) _outline.enabled = true;
        }
        else if (_outline != null)
        {
            _outline.enabled = false;
        }

        UpdateName();
    }

    /// <summary>
    /// 누름·뗌 입력을 처리한다.
    /// 뗀 시점의 대상이 누른 시점의 대상과 같고 클릭 가능하면 true와 함께 대상을 반환한다.
    /// </summary>
    public bool TryGetClickTarget(GameObject hitObj, out IClickable target)
    {
        target = null;

        if (Input.GetMouseButtonDown(0))
            _pressedObj = hitObj;

        if (!Input.GetMouseButtonUp(0)) return false;
        if (_hovered == null || hitObj != _pressedObj) return false;

        target = _hovered;
        return true;
    }

    /// <summary>아웃라인과 이름 표시를 모두 지운다.</summary>
    public void Clear()
    {
        if (_outline != null) _outline.enabled = false;
        _hovered = null;
        UpdateName();
    }

    /// <summary>현재 대상의 이름을 표시한다. 대상이 없거나 이름이 없으면 비운다.</summary>
    private void UpdateName()
    {
        string objName = _hovered != null ? _hovered.Return_ObjName() : null;
        _nameText.uiText.text = objName ?? "";
    }
}
