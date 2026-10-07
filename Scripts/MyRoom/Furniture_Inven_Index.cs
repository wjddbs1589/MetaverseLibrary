using UnityEngine;

/// <summary>
/// 가구 프리팹에 붙는 식별 정보.
/// InventoryIndex는 FurnitureManager가 프리팹 배열 순서로 설정하며, 저장·불러오기 때 어떤 가구인지 구분하는 데 쓴다.
/// </summary>
public class Furniture_Inven_Index : MonoBehaviour
{
    /// <summary>가구 인덱스 (FurnitureNumber와 같은 값)</summary>
    [HideInInspector] public int InventoryIndex = 0;

    [Tooltip("위치 조정 중 표시할 가구 이름")]
    public string Name;
}
