using UnityEngine;
using UnityEngine.UI;

//방 색상 변경
public class RCC : MonoBehaviour
{
    public Material FloorMaterial; //바닥 Material
    public Material WallMaterial;  //벽지 Material
    Material SelectedMaterial;     //선택된 Material

    bool wallColor = true; //true일 때는 벽지 색상 변경, false일 때 바닥 색상 변경

    Color MaterialColor; //타일의 색상
    Color[] colors;      //타일의 색으로 지정될 색상
    Button[] buttons;    //색상 지정 버튼

    [Header("벽지선택 버튼")]
    public Image wallButtons;
    public Image floorButtons;

    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;

    [Header("시작 색상")]
    public Color wall_OriginColor;
    public Color floor_OriginColor;

    [Header("팔레트 슬라이더")]
    [SerializeField] Slider slider_R;
    [SerializeField] Slider slider_G;
    [SerializeField] Slider slider_B;

    FurnitureManager FurnitureManager;

    [Header("색상초기화 버튼")]
    [SerializeField] Button btn_ColorReset;

    private void Awake()
    {
        FurnitureManager = FindAnyObjectByType<FurnitureManager>();
        btn_ColorReset.onClick.AddListener(() => Btn_ColorReset());
        buttons = transform.GetChild(0).GetComponentsInChildren<Button>();
        Set_Click_Color();
        Set_Btn_Event();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (wallColor)
        {
            MaterialColor = WallMaterial.color;
        }
        else
        {
            MaterialColor = FloorMaterial.color;
        }

        Set_RGB();
    }

    public void Init()
    {
        Change_Color_Wall(); //기본 세팅을 벽지 색상 선택으로 변경

        //매니저에 저장된 벽지 Material정보를 가져옴
        WallMaterial = FurnitureManager.wallMaterial;
        FloorMaterial = FurnitureManager.floorMaterial;

        //색상 자료가 있으면 적용. 아니면 기본 컬러로 지정
        if (FloorMaterial != null)
        {
            Take_ColorInfo(FloorMaterial);
        }
        else
        {
            FloorMaterial.color = floor_OriginColor;
            Take_ColorInfo(FloorMaterial);
        }

        if (WallMaterial != null)
        {
            Take_ColorInfo(WallMaterial);
        }
        else
        {
            WallMaterial.color = wall_OriginColor;
            Take_ColorInfo(WallMaterial);
        }
    }

    /// <summary>
    /// 버튼을 눌렀을 때 변경해줄 색상 정보 가져옴
    /// </summary>
    void Set_Click_Color()
    {
        colors = new Color[buttons.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = buttons[i].image.color;
        }
    }

    /// <summary>
    /// 각 버튼을 눌렀을 때 실행할 함수를 연결함
    /// </summary>
    void Set_Btn_Event()
    {
        for (int i = 0;i<buttons.Length; i++)
        {
            //AddListener가 i를 참조하여 모든 클로저가 동일한 변수를 가지므로 전부 16이 저장됨
            int index = i; //따라서 값을 임시변수에 저장하고 사용함
            buttons[i].onClick.AddListener( () => Set_Tile_Color(index) );
        }
    }

    /// <summary>
    /// 색상 버튼을 눌렀을 때 버튼의 인덱스에 맞는 색상인덱스로 벽지 색 지정
    /// </summary>
    /// <param name="index"></param>
    void Set_Tile_Color(int index)
    {
        if (wallColor)
        {
            WallMaterial.color = colors[index];
            Take_ColorInfo(WallMaterial);
        }
        else if (!wallColor)
        {
            FloorMaterial.color = colors[index];
            Take_ColorInfo(FloorMaterial);
        }
    }

    /// <summary>
    /// 색상 슬라이더가 변경 될 때 Material의 색 변경
    /// </summary>
    void Set_RGB()
    {
        MaterialColor.r = slider_R.value;
        MaterialColor.g = slider_G.value;
        MaterialColor.b = slider_B.value;

        SelectedMaterial.color = MaterialColor;
    }

    /// <summary>
    /// 파라미터값을 선택된 Material로 지정, 색상의 RGB에 맞게 슬라이더 조정
    /// </summary>
    /// <param name="material"></param>
    void Take_ColorInfo(Material material)
    {
        SelectedMaterial = material;

        MaterialColor = SelectedMaterial.color;
        slider_R.value = MaterialColor.r;
        slider_G.value = MaterialColor.g;
        slider_B.value = MaterialColor.b;
    }    

    /// <summary>
    /// 색상을 변경할 대상을 바닥으로 설정
    /// </summary>
    public void Change_Color_Floor()
    {
        wallColor = false;
        Change_Imgae();
    }

    /// <summary>
    /// 색상을 변경할 대상을 벽으로 설정
    /// </summary>
    public void Change_Color_Wall()
    {
        wallColor = true;
        Change_Imgae();
    }

    /// <summary>
    /// 현재 선택된 버튼의 이미지를 On 이미지로 바꿈
    /// </summary>
    void Change_Imgae()
    {
        if (wallColor)
        {
            wallButtons.sprite = onSprite;
            floorButtons.sprite = offSprite;
            Take_ColorInfo(WallMaterial);
        }
        else
        {
            wallButtons.sprite = offSprite;
            floorButtons.sprite = onSprite;
            Take_ColorInfo(FloorMaterial);
        }
    }

    /// <summary>
    /// 벽과 바닥의 색상을 기본 제공되는 색상으로 변경
    /// </summary>
    void Btn_ColorReset()
    {
        WallMaterial.color = wall_OriginColor;
        Take_ColorInfo(WallMaterial);

        FloorMaterial.color = floor_OriginColor;
        Take_ColorInfo(FloorMaterial);
    }
}
