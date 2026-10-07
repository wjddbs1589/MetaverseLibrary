using Suncheon;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 마우스가 UI 버튼 위에 있는 동안 플레이어의 오브젝트 상호작용을 막는다.
/// UI 버튼을 누를 때 버튼 뒤에 있는 3D 오브젝트까지 함께 클릭되는 문제를 방지한다.
/// AddHoverDetectorToButtons가 UI의 모든 버튼에 자동으로 붙인다.
/// </summary>
public class ButtonHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 내 플레이어의 상호작용 컴포넌트
    private PlayerObjInteraction _localInteraction;

    /// <summary>버튼 위에 마우스가 올라가면 오브젝트 상호작용을 막는다.</summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerObjInteraction interaction = GetLocalInteraction();
        if (interaction != null) interaction.ChooseUI = true;
    }

    /// <summary>버튼에서 마우스가 벗어나면 오브젝트 상호작용을 다시 허용한다.</summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerObjInteraction interaction = GetLocalInteraction();
        if (interaction != null) interaction.ChooseUI = false;
    }

    /// <summary>
    /// 내 플레이어의 상호작용 컴포넌트를 찾는다.
    /// 멀티플레이 씬에는 다른 플레이어의 컴포넌트도 있으므로 NetworkManager의 내 플레이어에서 가져온다.
    /// </summary>
    private PlayerObjInteraction GetLocalInteraction()
    {
        if (_localInteraction == null && NetworkManager.Instance.Go_Player != null)
            _localInteraction = NetworkManager.Instance.Go_Player.GetComponent<PlayerObjInteraction>();

        return _localInteraction;
    }
}
