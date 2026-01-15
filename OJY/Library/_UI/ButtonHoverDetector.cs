using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    PlayerObjInteraction playerObjInteraction;
    private void Start()
    {
        playerObjInteraction = FindAnyObjectByType<PlayerObjInteraction>();
    }

    // 마우스가 버튼 위에 올라갔을 때 호출됨
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerObjInteraction == null)
        {
            playerObjInteraction = FindAnyObjectByType<PlayerObjInteraction>();
        }
        playerObjInteraction.ChooseUI = true;
    }

    // 마우스가 버튼에서 벗어났을 때 호출됨
    public void OnPointerExit(PointerEventData eventData)
    {
        playerObjInteraction.ChooseUI = false;
    }

}
