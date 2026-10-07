using Suncheon;
using Suncheon.Player;
using Suncheon.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 가구 종류. 값은 FurnitureManager의 가구 프리팹 배열과 수납함 버튼의 순서와 같다.
/// _C = 클래식 가구 (기본 제공), _M = 모던 가구 (모든 업적 달성 시 잠금 해제)
/// </summary>
public enum FurnitureNumber
{
    Bookcase_C,
    Carpet_C,
    Drawers_C,
    Table_C,
    Lamp_C,
    RoundTable_C,
    Sofa1_C,
    Sofa2_C,
    BookCase_M,
    Carpet_M,
    Drawers_M,
    Lamp_M,
    Sofa1_M,
    Sofa2_M,
    Table1_M,
    Table2_M
}

/// <summary>
/// 가구 수납함 UI.
/// 이미 배치된 가구는 버튼을 비활성화하고, 카테고리별로 가구를 걸러 보여주며, 전체 수납 기능을 제공한다.
/// 모던 가구는 모든 업적을 달성한 유저에게만 표시한다.
/// </summary>
public class Inven_Controller : MonoBehaviour
{
    // 카테고리별 가구 목록
    private static readonly FurnitureNumber[] TableCategory =
    {
        FurnitureNumber.Bookcase_C, FurnitureNumber.BookCase_M,
        FurnitureNumber.Drawers_C, FurnitureNumber.Drawers_M,
        FurnitureNumber.Table_C, FurnitureNumber.RoundTable_C,
        FurnitureNumber.Table1_M, FurnitureNumber.Table2_M
    };

    private static readonly FurnitureNumber[] ChairCategory =
    {
        FurnitureNumber.Carpet_C, FurnitureNumber.Carpet_M,
        FurnitureNumber.Sofa1_C, FurnitureNumber.Sofa2_C,
        FurnitureNumber.Sofa1_M, FurnitureNumber.Sofa2_M
    };

    private static readonly FurnitureNumber[] EtcCategory =
    {
        FurnitureNumber.Lamp_C, FurnitureNumber.Lamp_M
    };

    // 모던 가구가 시작되는 인덱스
    private const int MODERN_START_INDEX = (int)FurnitureNumber.BookCase_M;

    [Header("인테리어 모드에서 숨길 버튼")]
    [SerializeField] private GameObject btn_saveN;
    [SerializeField] private GameObject btn_saveY;
    [SerializeField] private GameObject btn_palette;

    [Header("가구 선택 버튼")]
    [Tooltip("가구 버튼들의 부모 (자식 순서 = 가구 인덱스)")]
    [SerializeField] private GameObject content;

    [Header("가구 버튼 배경 이미지")]
    [SerializeField] private Sprite sprite_furnitureBox_on;
    [SerializeField] private Sprite sprite_furnitureBox_off;

    [Header("전체 수납")]
    [SerializeField] private Button btn_allDestroy;

    [Tooltip("전체 수납 확인 팝업")]
    [SerializeField] private GameObject DeleteUI;
    [SerializeField] private Button btn_Y;
    [SerializeField] private Button btn_N;

    [Header("카테고리 버튼")]
    [SerializeField] private Button btn_all;
    [SerializeField] private Button btn_table;
    [SerializeField] private Button btn_chair;
    [SerializeField] private Button btn_etc;

    [Header("카테고리 버튼 스타일")]
    [SerializeField] private Sprite Sprite_Category_On;
    [SerializeField] private Sprite Sprite_Category_Off;
    [SerializeField] private Color Text_On;
    [SerializeField] private Color Text_Off;

    [Header("수납 완료 알림")]
    [SerializeField] private Furniture_Alert Alert_anim;

    /// <summary>모던 가구 잠금 해제 여부</summary>
    [HideInInspector] public bool unLock_M;

    private Button[] _furnitureButtons;
    private Button[] _categoryButtons;
    private FurnitureManager _furnitureManager;

    /// <summary>모던 가구 잠금 여부를 확인하고 버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        Unlock_M();

        _furnitureButtons = content.GetComponentsInChildren<Button>();
        _categoryButtons = new[] { btn_all, btn_table, btn_chair, btn_etc };
        _furnitureManager = FindAnyObjectByType<FurnitureManager>();

        btn_allDestroy.onClick.AddListener(Btn_AllDestroy);
        btn_N.onClick.AddListener(Delete_Cancle);
        btn_Y.onClick.AddListener(Delete_Furniture);

        btn_all.onClick.AddListener(() => ShowCategory(null, btn_all));
        btn_table.onClick.AddListener(() => ShowCategory(TableCategory, btn_table));
        btn_chair.onClick.AddListener(() => ShowCategory(ChairCategory, btn_chair));
        btn_etc.onClick.AddListener(() => ShowCategory(EtcCategory, btn_etc));
    }

    /// <summary>수납함을 열면 저장 버튼을 숨기고, 배치 상태에 맞춰 버튼을 갱신한다.</summary>
    private void OnEnable()
    {
        SetSaveButtonsVisible(false);
        Set_FurnitureBox_UI();
        btn_allDestroy.interactable = _furnitureManager.Is_Furniture();
    }

    /// <summary>수납함을 닫으면 저장 버튼을 다시 표시한다.</summary>
    private void OnDisable()
    {
        SetSaveButtonsVisible(true);
    }

    /// <summary>인테리어 저장·팔레트 버튼 표시 여부를 바꾼다.</summary>
    private void SetSaveButtonsVisible(bool visible)
    {
        btn_saveN.SetActive(visible);
        btn_saveY.SetActive(visible);
        btn_palette.SetActive(visible);
    }

    /// <summary>모든 업적을 달성했으면 모던 가구를 잠금 해제한다.</summary>
    private void Unlock_M()
    {
        PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
        unLock_M = playerManager.achievementCompInfo.isAllComp;
    }

    /// <summary>이미 배치된 가구의 버튼은 비활성화하고, 배치 가능한 가구의 버튼은 활성화한다.</summary>
    private void Set_FurnitureBox_UI()
    {
        for (int i = 0; i < _furnitureButtons.Length; i++)
        {
            bool isPlaced = _furnitureManager.spawnedFurniture[i] != null;
            FurnitureBtnImage icon = _furnitureButtons[i].GetComponentInChildren<FurnitureBtnImage>();

            _furnitureButtons[i].enabled = !isPlaced;
            _furnitureButtons[i].image.sprite = isPlaced ? sprite_furnitureBox_off : sprite_furnitureBox_on;

            if (isPlaced) icon.Set_OffImage();
            else icon.Set_OnImage();
        }
    }

    // ── 전체 수납 ────────────────────────────────────────

    /// <summary>전체 수납 버튼. 확인 팝업을 연다.</summary>
    public void Btn_AllDestroy()
    {
        DeleteUI.SetActive(true);
    }

    /// <summary>전체 수납 확인. 배치된 가구를 모두 제거하고 알림을 표시한다.</summary>
    private void Delete_Furniture()
    {
        btn_Y.GetComponent<ButtonControl>().Btnoff();
        _furnitureManager.All_Destroy();
        Alert_anim.Play_Anim();
        DeleteUI.SetActive(false);
    }

    /// <summary>전체 수납 취소.</summary>
    private void Delete_Cancle()
    {
        btn_N.GetComponent<ButtonControl>().Btnoff();
        DeleteUI.SetActive(false);
    }

    // ── 카테고리 ─────────────────────────────────────────

    /// <summary>
    /// 카테고리에 속한 가구 버튼만 표시한다. category가 null이면 전체를 표시한다.
    /// 선택한 카테고리 버튼을 강조하고, 잠긴 모던 가구는 다시 숨긴다.
    /// </summary>
    private void ShowCategory(FurnitureNumber[] category, Button selected)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            bool visible = category == null || Array.IndexOf(category, (FurnitureNumber)i) >= 0;
            content.transform.GetChild(i).gameObject.SetActive(visible);
        }

        foreach (Button button in _categoryButtons)
            Set_BtnUI(button, button == selected);

        Lock_M();
    }

    /// <summary>카테고리 버튼의 선택 상태에 맞춰 배경과 글자 색을 바꾼다.</summary>
    private void Set_BtnUI(Button btn, bool isOn)
    {
        btn.image.sprite = isOn ? Sprite_Category_On : Sprite_Category_Off;
        btn.GetComponentInChildren<TextMeshProUGUI>().color = isOn ? Text_On : Text_Off;
    }

    /// <summary>모던 가구가 잠겨 있으면 수납함에서 숨긴다.</summary>
    private void Lock_M()
    {
        if (unLock_M) return;

        for (int i = MODERN_START_INDEX; i < content.transform.childCount; i++)
            content.transform.GetChild(i).gameObject.SetActive(false);
    }
}
