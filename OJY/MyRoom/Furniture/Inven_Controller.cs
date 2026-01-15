using Suncheon;
using Suncheon.UI;
using Suncheon.Player;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

public class Inven_Controller : MonoBehaviour
{
    [Header("버튼 UI")]
    [SerializeField] GameObject btn_saveN;
    [SerializeField] GameObject btn_saveY;
    [SerializeField] GameObject btn_palette;

    [Header("가구 선택 버튼")]
    [SerializeField] GameObject content;

    [Header("가구 선택 UI 이미지")]
    [SerializeField] Sprite sprite_furnitureBox_on;
    [SerializeField] Sprite sprite_furnitureBox_off;

    [Header("가구 전체 수납 버튼")]
    [SerializeField] Button btn_allDestroy;

    [Header("가구수납 UI")]
    [SerializeField] GameObject DeleteUI;
    [SerializeField] Button btn_Y;
    [SerializeField] Button btn_N;

    Button[] btn_furnitures;
    FurnitureManager furnitureManager;

    [Header("가구수납함 버튼")]
    [SerializeField] Button btn_all;
    [SerializeField] Button btn_table;
    [SerializeField] Button btn_chair;
    [SerializeField] Button btn_etc;

    [Header("가구수납함 버튼 스프라이트")]
    [SerializeField] Sprite Sprite_Category_On;
    [SerializeField] Sprite Sprite_Category_Off;

    [Header("가구수납함 텍스트 색상")]
    [SerializeField] Color Text_On;
    [SerializeField] Color Text_Off;

    [Header("인테리어 저장 Anim")]
    [SerializeField] Furniture_Alert Alert_anim;

    [Header("가구 언락여부")]
    List<int> treasureNumber = new List<int>(8);
    [HideInInspector]public bool unLock_M;
    [HideInInspector]public bool unLock_T;

    private void Awake()
    {
        Unlock_M();

        btn_furnitures = content.GetComponentsInChildren<Button>();
        furnitureManager = FindAnyObjectByType<FurnitureManager>();

        btn_allDestroy.onClick.AddListener(() => Btn_AllDestroy());
        btn_N.onClick.AddListener(() => Delete_Cancle());
        btn_Y.onClick.AddListener(() => Delete_Furniture());

        btn_all.onClick.AddListener(() => Btn_All());
        btn_table.onClick.AddListener(() => Btn_Table());
        btn_chair.onClick.AddListener(() => Btn_Chair());
        btn_etc.onClick.AddListener(() => Btn_Etc());
    }

    private void OnEnable()
    {
        Change_UI(false);
        Set_FurnitureBox_UI();
        Set_DestroyBtn();
    }

    private void OnDisable()
    {
        Change_UI(true);
    }

    /// <summary>
    /// 인벤토리를 열 때 가구가 하나 이상 있는지 확인하여 버튼 활성화여부 결정
    /// </summary>
    void Set_DestroyBtn()
    {
        btn_allDestroy.interactable = furnitureManager.Is_Furniture() ? true : false;
    }

    /// <summary>
    /// 인테리어 버튼을 눌렀을때 플레이어UI와 인테리어UI를 전환
    /// </summary>
    /// <param name="isOn"></param>
    void Change_UI(bool isOn)
    {
        btn_saveN.SetActive(isOn);
        btn_saveY.SetActive(isOn);
        btn_palette.SetActive(isOn);
    }

    /// <summary>
    /// 모던 가구 잠금해제 확인
    /// </summary>
    void Unlock_M()
    {
        PlayerManager playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

        unLock_M = playerManager.achievementCompInfo.isAllComp;
    }

    /// <summary>
    /// 현재 배치된 오브젝트의 상황에 따라 UI 변경
    /// </summary>
    void Set_FurnitureBox_UI()
    {
        for (int i = 0; i< btn_furnitures.Length; i++)
        {
            FurnitureBtnImage btn_Image = btn_furnitures[i].GetComponentInChildren<FurnitureBtnImage>();

            //현재 소환된 오브젝트가 있으면 버튼 비 활성화
            if (furnitureManager.spawnedFuniture[i] != null)
            {
                btn_furnitures[i].enabled = false;
                btn_furnitures[i].image.sprite = sprite_furnitureBox_off;
                btn_Image.Set_OffImage();

            }
            else
            {
                btn_furnitures[i].image.sprite = sprite_furnitureBox_on;
                btn_furnitures[i].enabled = true;
                btn_Image.Set_OnImage();
            }
            
        }
    }

    #region 가구 수납 기능
    /// <summary>
    /// 전체 수납 버튼
    /// </summary>
    public void Btn_AllDestroy()
    {
        DeleteUI.SetActive(true);
    }

    /// <summary>
    /// 전체수납 실행 기능. 가구가 한개이상 있을 때 실행 가능
    /// </summary>
    void Delete_Furniture()
    {
        btn_Y.GetComponent<ButtonControl>().Btnoff();
        furnitureManager.All_Destory();
        Alert_anim.Play_Anim();
        DeleteUI.SetActive(false);
    }

    /// <summary>
    /// 전체수납 취소 기능
    /// </summary>
    void Delete_Cancle()
    {
        btn_N.GetComponent<ButtonControl>().Btnoff();
        DeleteUI.SetActive(false);
    }
    #endregion

    #region 가구 카테고리 버튼
    /// <summary>
    /// 수납함에서 가구 카테고리 '전체'를 눌렀을 때 실행될 함수
    /// </summary>
    void Btn_All()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).gameObject.SetActive(true);
        }

        On_Btn(btn_all);
    }

    /// <summary>
    /// 수납함에서 가구 카테고리 '서랍/책상'를 눌렀을 때 실행될 함수
    /// </summary>
    void Btn_Table()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            FurnitureNumber furnitureNumber = (FurnitureNumber)i;
            bool isActive = 
                furnitureNumber == FurnitureNumber.Bookcase_C ||  //책장
                furnitureNumber == FurnitureNumber.BookCase_M ||  //책장
                furnitureNumber == FurnitureNumber.Drawers_C ||   //서랍
                furnitureNumber == FurnitureNumber.Drawers_M ||   //서랍
                furnitureNumber == FurnitureNumber.Table_C ||     //테이블
                furnitureNumber == FurnitureNumber.RoundTable_C ||//테이블
                furnitureNumber == FurnitureNumber.Table1_M ||    //테이블
                furnitureNumber == FurnitureNumber.Table2_M;      //테이블

            content.transform.GetChild(i).gameObject.SetActive(isActive);
        }
        On_Btn(btn_table);
    }

    /// <summary>
    /// 수납함에서 가구 카테고리 "카펫 / 의자'를 눌렀을 때 실행될 함수
    /// </summary>
    void Btn_Chair()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            FurnitureNumber furnitureNumber = (FurnitureNumber)i;
            bool isActive =
                furnitureNumber == FurnitureNumber.Carpet_C || //카펫
                furnitureNumber == FurnitureNumber.Carpet_M || //카펫
                furnitureNumber == FurnitureNumber.Sofa1_C ||  //소파
                furnitureNumber == FurnitureNumber.Sofa2_C ||  //소파
                furnitureNumber == FurnitureNumber.Sofa1_M ||  //소파
                furnitureNumber == FurnitureNumber.Sofa2_M;    //소파


            content.transform.GetChild(i).gameObject.SetActive(isActive);
        }
        On_Btn(btn_chair);
    }

    /// <summary>
    /// 수납함에서 가구 카테고리 '기타'를 눌렀을 때 실행될 함수
    /// </summary>
    void Btn_Etc()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            FurnitureNumber furnitureNumber = (FurnitureNumber)i;
            bool isActive = furnitureNumber == FurnitureNumber.Lamp_C ||
                            furnitureNumber == FurnitureNumber.Lamp_M;

            content.transform.GetChild(i).gameObject.SetActive(isActive);
        }

        On_Btn(btn_etc);
    }
    
    /// <summary>
    /// 파라미터로 들어간 버튼과 기존의 버튼들을 비교하여 스프라이트 및 텍스트 컬러 변경
    /// </summary>
    void On_Btn(Button btn)
    {
        if (btn_all == btn)
        {
            Set_BtnUI(btn, true);
        }
        else
        {
            Set_BtnUI(btn_all, false);
        }

        if (btn_table == btn)
        {
            Set_BtnUI(btn, true);
        }
        else
        {
            Set_BtnUI(btn_table, false);
        }

        if (btn_chair == btn)
        {
            Set_BtnUI(btn, true);
        }
        else
        {
            Set_BtnUI(btn_chair, false);
        }

        if (btn_etc == btn)
        {
            Set_BtnUI(btn, true);
        }
        else
        {
            Set_BtnUI(btn_etc, false);            
        }

        Lock_M();
    }

    /// <summary>
    /// 버튼상태에 따라 UI 설정 변경
    /// </summary>
    void Set_BtnUI(Button btn, bool isOn)
    {
        if (isOn)
        {
            btn.image.sprite = Sprite_Category_On;
            btn.GetComponentInChildren<TextMeshProUGUI>().color = Text_On;
        }
        else
        {
            btn.image.sprite = Sprite_Category_Off;
            btn.GetComponentInChildren<TextMeshProUGUI>().color = Text_Off;
        }
        
    }
    #endregion

    /// <summary>
    /// 수납함 모던가구 on/off
    /// </summary>
    void Lock_M()
    {
        if (!unLock_M)
        {
            for (int i = 8; i < 16; i++)
            {
                content.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
