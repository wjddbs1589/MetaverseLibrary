using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 수납함의 가구 버튼. 형제 순서를 가구 인덱스로 사용해, 누르면 해당 가구를 꺼낸다.
/// 버튼 순서를 FurnitureNumber와 같게 배치해 두면 버튼마다 인덱스를 따로 입력하지 않아도 된다.
/// </summary>
[RequireComponent(typeof(Button))]
public class FurnitureBtnNumber : MonoBehaviour
{
    // 이 버튼이 꺼낼 가구 인덱스
    private int _furnitureIndex;

    /// <summary>형제 순서로 가구 인덱스를 정하고 클릭 이벤트를 연결한다.</summary>
    private void Awake()
    {
        _furnitureIndex = transform.GetSiblingIndex();
        GetComponent<Button>().onClick.AddListener(Btn_Click);
    }

    /// <summary>해당 가구를 생성하고 위치 조정을 시작한다.</summary>
    private void Btn_Click()
    {
        FurnitureManager.Instance.Spawn_Furniture(_furnitureIndex);
    }
}
