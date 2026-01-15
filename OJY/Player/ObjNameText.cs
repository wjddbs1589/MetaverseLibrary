using Suncheon.UI;
using TMPro;
using UnityEngine;

public class ObjNameText : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI uiText;
    Canvas canvas;
    float offset = -80f; // 마우스와 UI 사이의 거리 조절

    [HideInInspector]public bool Clickable;

    private void Awake()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        canvas = GetComponentInParent<Canvas>();
    }

    void FixedUpdate()
    {
        if (UIInteractionManager.Instance == null) return;

        if (UIInteractionManager.Instance.IsUIInteraction) return;

        Vector2 mousePosition = Input.mousePosition;

        // RectTransform을 사용하여 캔버스 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            mousePosition, canvas.worldCamera, out Vector2 localPoint);

        uiText.rectTransform.localPosition = localPoint + new Vector2(0f, offset);
    }
}
