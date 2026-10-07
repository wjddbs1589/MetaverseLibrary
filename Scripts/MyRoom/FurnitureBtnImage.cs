using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 수납함 가구 버튼의 아이콘. 배치 가능하면 On 이미지, 이미 배치된 가구면 Off 이미지를 표시한다.
/// </summary>
[RequireComponent(typeof(Image))]
public class FurnitureBtnImage : MonoBehaviour
{
    [Tooltip("배치 가능할 때 아이콘")]
    [SerializeField] private Sprite On_Image;

    [Tooltip("이미 배치되었을 때 아이콘")]
    [SerializeField] private Sprite Off_Image;

    private Image _image;

    /// <summary>배치 가능 상태 아이콘으로 바꾼다.</summary>
    public void Set_OnImage() => Set_Image(true);

    /// <summary>이미 배치된 상태 아이콘으로 바꾼다.</summary>
    public void Set_OffImage() => Set_Image(false);

    /// <summary>
    /// 아이콘을 바꾼다.
    /// 카테고리 필터로 숨겨진 버튼은 Awake가 아직 실행되지 않았을 수 있으므로, 처음 호출될 때 Image를 가져온다.
    /// </summary>
    private void Set_Image(bool isOn)
    {
        if (_image == null) _image = GetComponent<Image>();
        _image.sprite = isOn ? On_Image : Off_Image;
    }
}
