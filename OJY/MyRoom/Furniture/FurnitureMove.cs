using Suncheon;
using System.Collections;
using TMPro;
using UnityEngine;

public class FurnitureMove : MonoBehaviour
{
    [Header("선택된 가구")]
    public GameObject SelectedFurniture;

    [Header("선택된 가구의 이름")]
    [SerializeField] TextMeshProUGUI FurnitureName;

    Vector3 startPos;
    Vector3 FloorPos;
    Vector3 WallPos;

    [SerializeField] bool ObjGrab = false;

    [Header("가구배치 캔버스")]
    public GameObject ObjAdjustUI;
    [Header("수납함 버튼")]
    public GameObject inventoryBtn;

    Outline outline;

    PositionCheck check;

    [Header("아이템 인벤토리")]
    public GameObject inventory;

    bool UseInven = false;
    public bool positioning = false;

    [SerializeField] Camera rayCam;
    
    FurnitureManager furnitureManager;

    #region 개인서재 오브젝트 상호작용
    GameObject hitObj;  //ray에 닿은 오브젝트
    GameObject clickObj;//마지막으로 선택된 오브젝트
    Outline objOutline;
    Clickable clickableObj;
    ObjNameText nameText;
    public bool UsingUI = false;
    #endregion

    private void Awake()
    {        
        furnitureManager = FindAnyObjectByType<FurnitureManager>();
        nameText = NetworkManager.Instance.Go_Player.GetComponentInChildren<ObjNameText>();
    }
    private void Update()
    {
        //인테리어 사용중 일 때
        if (furnitureManager.usingInterior)
        {
            FloorCheck();

            if (!UseInven)
            {
                Furniture_Click();
            }

            CrashCheck();
        }
        else
        {
            Obj_Interactive();
        }
    }

    #region 인테리어 기능
    /// <summary>
    /// 마우스가 가리키고 있는 곳이 바닥인지 벽인지 확인
    /// </summary>
    void FloorCheck()
    {
        Ray PosRay = rayCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, PosRay.direction);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Floor"))
            {
                FloorPos = hits[i].point;
                break;
            }
            else if (hits[i].collider.gameObject.CompareTag("Wall"))
            {
                WallPos = hits[i].point;
                break;
            }
        }
    }

    /// <summary>
    /// 가구를 클릭한 것인지 확인
    /// </summary>
    void Furniture_Click()
    {
        Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits2 = Physics.RaycastAll(transform.position, ray.direction, 1000);

        //눌러서 가구 선택
        if (Input.GetMouseButtonDown(0))
        {
            if (!positioning)
            {
                foreach (RaycastHit hit in hits2)
                {
                    GameObject hitObj = hit.collider.gameObject;

                    if (hitObj.CompareTag("Furniture"))
                    {
                        SelectedFurniture = hitObj;
                        check = SelectedFurniture.GetComponent<PositionCheck>();
                        positioning = !positioning;
                        return;
                    }
                }
            }
        }

        //누르고 있을 때
        if (Input.GetMouseButton(0))
        {
            if (!ObjGrab)
            {
                foreach (RaycastHit hit in hits2)
                {
                    GameObject hitObj = hit.collider.gameObject;

                    if (hitObj.CompareTag("Furniture") && (hitObj == SelectedFurniture))
                    {
                        Obj_Grab();
                    }
                }

            }
            else if (ObjGrab)
            {
                if (SelectedFurniture.layer == LayerMask.NameToLayer("Floor"))
                {
                    Set_FurniturePosition(FloorPos);
                }
                else if (SelectedFurniture.layer == LayerMask.NameToLayer("Wall"))
                {
                    Set_FurniturePosition(WallPos);
                }
            }
        }

        //뗐을 때 
        if (Input.GetMouseButtonUp(0))
        {
            //물건을 이동중일 때
            if (ObjGrab)
            {
                //유효한 위치가 아니면 원래의 위치로 복귀
                if (check.Crash)
                {
                    Set_FurniturePosition(startPos);     //처음 위치로 이동
                    outline.OutlineColor = Color.yellow; //아웃라인 색상 변경
                }
            }

            ObjGrab = false;
        }
    }

    /// <summary>
    /// 가구를 선택 했을 때 실행될 기본 세팅
    /// </summary>
    void Furniture_ChooseSetting()
    {
        check = SelectedFurniture.GetComponent<PositionCheck>();
        outline = SelectedFurniture.GetComponent<Outline>();
        outline.enabled = true;

        StartCoroutine(CrashCheckCo());
    }

    IEnumerator CrashCheckCo()
    {
        yield return new WaitForSeconds(0.01f);
        //CrashCheck();
    }

    
    /// <summary>
    /// 물건 잡을때 실행
    /// </summary>
    void Obj_Grab()
    {
        ObjGrab = true;
        ObjAdjust_Start();
    }

    /// <summary>
    /// 가구 위치 변경
    /// </summary>
    /// <param name="position"></param>
    void Set_FurniturePosition(Vector3 position)
    {
        SelectedFurniture.transform.position = position;
    }

    /// <summary>
    /// 선택된 가구 삭제
    /// </summary>
    public void Furniture_cancle()
    {
        ObjAdjust_End();
        furnitureManager.Furniture_Delete(SelectedFurniture);
        Destroy(SelectedFurniture);
        ObjGrab = false;
    }

    /// <summary>
    /// 선택된 가구 회전
    /// </summary>
    public void Furniture_rotate()
    {
        // 현재 회전 각도를 가져오기
        Vector3 currentRotation = SelectedFurniture.transform.rotation.eulerAngles;

        // Y축을 기준으로 90도씩 회전
        currentRotation.y += 90f;

        // 새로운 회전값 적용
        SelectedFurniture.transform.rotation = Quaternion.Euler(currentRotation);
    }

    /// <summary>
    /// 가구 배치 확정
    /// </summary>
    public void Furniture_complete()
    {
        if (!check.Crash)
        {
            ObjAdjust_End();
            outline = SelectedFurniture.GetComponent<Outline>();
            outline.enabled = false;
            furnitureManager.Furniture_Set(SelectedFurniture);

            SelectedFurniture = null;
            positioning = false;
        }
    }

    /// <summary>
    /// 배치 시작. 배치 UI 생성
    /// </summary>
    public void ObjAdjust_Start()
    {
        positioning = true;
        startPos = SelectedFurniture.transform.position; //가구가 이동하기 전의 위치 저장

        //새로 잡은 물건에 OutLine 생성
        Furniture_ChooseSetting();        
        Close_Inventory();  //인벤 UI 제거

        ObjAdjustUI.SetActive(true);
        inventoryBtn.SetActive(false);

        FurnitureName.text = SelectedFurniture.GetComponent<Furniture_Inven_Index>().Name;
    }

    /// <summary>
    /// 배치 끝냄. 배치 UI제거 및 수납함 버튼 생성
    /// </summary>
    public void ObjAdjust_End()
    {
        positioning = false;
        ObjAdjustUI.SetActive(false);
        inventoryBtn.SetActive(true);
    }

    /// <summary>
    /// 위치 조정중에 위치에 따라 Outline색 변경. 배치가능 = 노란색, 배치불가 = 빨간색
    /// </summary>
    public void CrashCheck()
    {
        if (SelectedFurniture)
        {
            if (outline == null)
            {
                return;
            }

            //충돌중이면 아웃라인 적색 아니면 노란색으로 표기
            if (check.Crash)
            {
                outline.OutlineColor = Color.red;
            }
            else
            {
                outline.OutlineColor = Color.yellow;
            }
        }
    }

    /// <summary>
    /// 인벤토리 열기 버튼
    /// </summary>
    public void Open_Inventory()
    {
        inventory.SetActive(true);
        UseInven = true;
    }

    /// <summary>
    /// 인벤토리 닫기 버튼
    /// </summary>
    public void Close_Inventory()
    {
        inventory.SetActive(false);
        UseInven = false;
    }
    #endregion

    #region 맵 오브젝트 상호작용
    /// <summary>
    /// 클릭 가능한 오브젝트 탐지
    /// </summary>
    void Obj_Interactive()
    {
        if (UsingUI)
        {
            return;
        }

        Ray clickRay = rayCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit clickHit;

        if (Physics.Raycast(clickRay, out clickHit))
        {
            hitObj = clickHit.collider.gameObject;

            Clickable_Check(hitObj);

            if (Input.GetMouseButtonDown(0))
            {
                clickObj = hitObj;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Click_Obj();
            }
        }
        else
        {
            Clear();
            return;
        }
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트의 아웃라인 생성
    /// </summary>
    void Clickable_Check(GameObject hit)
    {
        clickableObj = hit.GetComponent<Clickable>();
        if (clickableObj != null)
        {
            //사용중인 아웃라인이 없을 때
            if (objOutline == null)
            {
                objOutline = hit.GetComponent<Outline>();
            }
            //사용중인 아웃라인이 이미 있을 때
            else
            {
                Outline newOutline = hit.GetComponent<Outline>();

                // 아웃라인 대상이 같지 않을 때
                if (objOutline != newOutline)
                {
                    objOutline.enabled = false; //기존 아웃라인 끄기
                    objOutline = newOutline;    //아웃라인 대상 변경
                }
            }

            objOutline.enabled = true;
            Set_ObjName();
        }
        else
        {
            if (objOutline != null)
            {
                objOutline.enabled = false;
            }
            Set_ObjName();
        }
    }


    /// <summary>
    /// 상호작용 가능한 오브젝트이면 기능 실행
    /// </summary>
    void Click_Obj()
    {
        //눌러서 가구 선택        
        Ray clickRay = rayCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit clickHit;

        if (Physics.Raycast(clickRay, out clickHit))
        {
            Clickable clickable = clickHit.collider.gameObject.GetComponent<Clickable>();
            if (clickable != null && (clickHit.collider.gameObject == clickObj))
            {
                UsingUI = true;
                clickable.OnClick();
                Clear();
            }
        }
    }

    /// <summary>
    /// 기존에 적용된 사항들 초기화
    /// </summary>
    public void Clear()
    {
        if (objOutline != null)
        {
            objOutline.enabled = false;
        }
        hitObj = null;
        clickableObj = null;
        Set_ObjName();
    }

    /// <summary>
    /// 상호작용 오브젝트의 이름표시 
    /// </summary>
    /// <param name="isOn"></param>
    void Set_ObjName()
    {
        if (clickableObj == null)
        {
            nameText.uiText.text = "";
            return;
        }

        string name = clickableObj.Return_ObjName();

        if (name != null)
        {
            nameText.uiText.text = name;
        }
        else
        {
            nameText.uiText.text = "";
        }
    }
    #endregion
}
