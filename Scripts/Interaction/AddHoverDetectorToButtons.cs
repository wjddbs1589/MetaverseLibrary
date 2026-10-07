using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 루트에 붙이면 하위의 모든 버튼에 ButtonHoverDetector를 자동으로 추가한다.
/// 버튼마다 일일이 컴포넌트를 붙이지 않아도 UI 뒤 오브젝트 클릭 방지가 적용된다.
/// </summary>
public class AddHoverDetectorToButtons : MonoBehaviour
{
    /// <summary>하위 버튼 중 ButtonHoverDetector가 없는 버튼에 추가한다. (비활성 버튼 포함)</summary>
    private void Awake()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            if (!button.TryGetComponent(out ButtonHoverDetector _))
                button.gameObject.AddComponent<ButtonHoverDetector>();
        }
    }
}
