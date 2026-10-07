using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개인서재의 벽지·바닥 색상 변경 UI.
/// 벽지와 바닥 중 편집 대상을 고르고, 프리셋 색상 버튼이나 RGB 슬라이더로 색을 바꾼다.
/// 바뀐 색은 FurnitureManager의 머티리얼에 바로 반영되고, 인테리어 저장 시 함께 저장된다.
/// </summary>
public class RoomColorChanger : MonoBehaviour
{
    [Header("편집 대상 선택 버튼")]
    [Tooltip("벽지 편집 버튼 이미지")]
    public Image wallButtons;

    [Tooltip("바닥 편집 버튼 이미지")]
    public Image floorButtons;

    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [Header("기본 색상")]
    public Color wall_OriginColor;
    public Color floor_OriginColor;

    [Header("RGB 슬라이더")]
    [SerializeField] private Slider slider_R;
    [SerializeField] private Slider slider_G;
    [SerializeField] private Slider slider_B;

    [Header("색상 초기화 버튼")]
    [SerializeField] private Button btn_ColorReset;

    // 벽지·바닥 머티리얼 (FurnitureManager에서 가져옴)
    private Material _wallMaterial;
    private Material _floorMaterial;

    // 현재 편집 중인 머티리얼
    private Material _selectedMaterial;

    // true = 벽지 편집, false = 바닥 편집
    private bool _isWallMode = true;

    // 프리셋 색상 버튼과 각 버튼의 색
    private Button[] _presetButtons;
    private Color[] _presetColors;

    private FurnitureManager _furnitureManager;

    /// <summary>프리셋 버튼의 색을 읽어 두고 버튼·슬라이더 이벤트를 연결한다.</summary>
    private void Awake()
    {
        _furnitureManager = FindAnyObjectByType<FurnitureManager>();
        btn_ColorReset.onClick.AddListener(Btn_ColorReset);

        // 첫 번째 자식 아래의 버튼들이 프리셋 색상 버튼이며, 버튼 이미지 색이 곧 적용할 색이다
        _presetButtons = transform.GetChild(0).GetComponentsInChildren<Button>();
        _presetColors = new Color[_presetButtons.Length];

        for (int i = 0; i < _presetButtons.Length; i++)
        {
            _presetColors[i] = _presetButtons[i].image.color;

            // 람다가 반복 변수 i를 직접 참조하면 모든 버튼이 마지막 값을 쓰게 되므로 지역 변수로 복사한다
            int index = i;
            _presetButtons[i].onClick.AddListener(() => Set_Tile_Color(index));
        }

        slider_R.onValueChanged.AddListener(OnSliderChanged);
        slider_G.onValueChanged.AddListener(OnSliderChanged);
        slider_B.onValueChanged.AddListener(OnSliderChanged);
    }

    /// <summary>UI를 열 때마다 초기화한다.</summary>
    private void OnEnable()
    {
        Init();
    }

    /// <summary>FurnitureManager의 머티리얼을 가져오고 벽지 편집 상태로 시작한다.</summary>
    public void Init()
    {
        _wallMaterial = _furnitureManager.wallMaterial;
        _floorMaterial = _furnitureManager.floorMaterial;

        if (_wallMaterial == null || _floorMaterial == null)
        {
            Debug.LogWarning("[RoomColorChanger] 벽지 또는 바닥 머티리얼이 없습니다.", this);
            return;
        }

        Change_Color_Wall();
    }

    /// <summary>편집 대상을 바닥으로 바꾼다.</summary>
    public void Change_Color_Floor()
    {
        _isWallMode = false;
        RefreshModeUI();
    }

    /// <summary>편집 대상을 벽지로 바꾼다.</summary>
    public void Change_Color_Wall()
    {
        _isWallMode = true;
        RefreshModeUI();
    }

    /// <summary>선택된 편집 대상 버튼을 강조하고, 슬라이더를 그 머티리얼의 색에 맞춘다.</summary>
    private void RefreshModeUI()
    {
        wallButtons.sprite = _isWallMode ? onSprite : offSprite;
        floorButtons.sprite = _isWallMode ? offSprite : onSprite;
        SelectMaterial(_isWallMode ? _wallMaterial : _floorMaterial);
    }

    /// <summary>프리셋 색상 버튼을 누르면 편집 중인 머티리얼에 해당 색을 적용한다.</summary>
    private void Set_Tile_Color(int index)
    {
        _selectedMaterial.color = _presetColors[index];
        SelectMaterial(_selectedMaterial);
    }

    /// <summary>슬라이더를 움직이면 편집 중인 머티리얼의 RGB를 바꾼다.</summary>
    private void OnSliderChanged(float _)
    {
        if (_selectedMaterial == null) return;

        Color color = _selectedMaterial.color;
        color.r = slider_R.value;
        color.g = slider_G.value;
        color.b = slider_B.value;
        _selectedMaterial.color = color;
    }

    /// <summary>
    /// 편집 대상 머티리얼을 바꾸고 슬라이더를 그 색에 맞춘다.
    /// 슬라이더 값을 코드로 바꿀 때는 이벤트가 다시 호출되지 않도록 SetValueWithoutNotify를 사용한다.
    /// </summary>
    private void SelectMaterial(Material material)
    {
        _selectedMaterial = material;

        Color color = material.color;
        slider_R.SetValueWithoutNotify(color.r);
        slider_G.SetValueWithoutNotify(color.g);
        slider_B.SetValueWithoutNotify(color.b);
    }

    /// <summary>벽지와 바닥을 기본 색상으로 되돌리고, 현재 편집 대상 기준으로 슬라이더를 갱신한다.</summary>
    private void Btn_ColorReset()
    {
        _wallMaterial.color = wall_OriginColor;
        _floorMaterial.color = floor_OriginColor;
        RefreshModeUI();
    }
}
