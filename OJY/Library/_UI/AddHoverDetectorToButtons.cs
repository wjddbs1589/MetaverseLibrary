using UnityEngine;
using UnityEngine.UI;

public class AddHoverDetectorToButtons : MonoBehaviour
{
    void Awake()
    {
        // UI의 모든 버튼 찾기
        Button[] buttons = GetComponentsInChildren<Button>();

        // 각 버튼에 ButtonHoverDetector 스크립트 추가
        foreach (Button button in buttons)
        {
            ButtonHoverDetector hoverDetector = button.gameObject.GetComponent<ButtonHoverDetector>();
            if (hoverDetector == null)
            {
                hoverDetector = button.gameObject.AddComponent<ButtonHoverDetector>();
            }
        }
    }
}