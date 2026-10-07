using Suncheon;
using TMPro;
using UnityEngine;

/// <summary>
/// 개인서재에서 가구 선택·이동·회전·배치 확정과, 인테리어 모드가 아닐 때의 오브젝트 상호작용을 처리한다.
///
/// 가구 배치:
///   1. 가구를 누르면 선택하고, 누른 채로 드래그하면 마우스가 가리키는 바닥(또는 벽) 위치로 이동한다.
///      가구 레이어가 Floor면 바닥에, Wall이면 벽에만 붙는다.
///   2. 다른 물체와 겹치면 아웃라인이 빨간색, 배치 가능하면 노란색으로 표시된다.
///   3. 겹친 상태에서 손을 떼면 잡기 전 위치로 되돌린다.
///   4. 겹치지 않을 때만 배치를 확정할 수 있다.
///
/// 오브젝트 상호작용:
///   서재 전용 카메라를 사용하므로 PlayerObjInteraction 대신 이 컴포넌트가 처리하며,
///   강조와 클릭 판정은 ClickableHoverHandler를 함께 사용한다.
/// </summary>
public class FurnitureMove : MonoBehaviour
{
    [Header("선택된 가구")]
    public GameObject SelectedFurniture;

    [Tooltip("선택된 가구 이름을 표시할 텍스트")]
    [SerializeField] private TextMeshProUGUI FurnitureName;

    [Header("UI")]
    [Tooltip("가구 위치 조정 중 표시할 UI (회전·확정·취소 버튼)")]
    public GameObject ObjAdjustUI;

    [Tooltip("수납함 열기 버튼")]
    public GameObject inventoryBtn;

    [Tooltip("가구 수납함")]
    public GameObject inventory;

    [Header("레이")]
    [Tooltip("서재 전용 카메라")]
    [SerializeField] private Camera rayCam;

    /// <summary>가구 위치 조정 중이면 true</summary>
    [HideInInspector] public bool positioning = false;

    /// <summary>팝업 UI를 사용 중이면 true (오브젝트 상호작용 중지)</summary>
    [HideInInspector] public bool UsingUI = false;

    // 가구를 잡기 전 위치 (배치 불가 위치에서 놓으면 이 위치로 복귀)
    private Vector3 _startPos;

    // 마우스가 가리키는 바닥·벽 위치
    private Vector3 _floorPos;
    private Vector3 _wallPos;

    // 가구를 잡고 드래그 중이면 true
    private bool _isGrabbing = false;

    // 수납함이 열려 있으면 true (가구 선택 중지)
    private bool _useInven = false;

    // 선택된 가구의 아웃라인과 겹침 판정
    private Outline _outline;
    private PositionCheck _check;

    private FurnitureManager _furnitureManager;
    private ClickableHoverHandler _hoverHandler;

    /// <summary>가구 매니저와 오브젝트 강조·클릭 처리기를 준비한다.</summary>
    private void Awake()
    {
        _furnitureManager = FindAnyObjectByType<FurnitureManager>();

        ObjNameText nameText = NetworkManager.Instance.Go_Player.GetComponentInChildren<ObjNameText>();
        _hoverHandler = new ClickableHoverHandler(nameText);
    }

    /// <summary>인테리어 모드면 가구 배치를, 아니면 오브젝트 상호작용을 처리한다.</summary>
    private void Update()
    {
        if (_furnitureManager.usingInterior)
        {
            UpdatePointedSurface();

            if (!_useInven)
                Furniture_Click();

            CrashCheck();
        }
        else
        {
            Obj_Interactive();
        }
    }

    // ── 가구 배치 ────────────────────────────────────────

    /// <summary>마우스가 가리키는 방향에서 가장 먼저 맞는 바닥 또는 벽의 위치를 저장한다.</summary>
    private void UpdatePointedSurface()
    {
        Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);

        // 가구 자신에 가려지지 않도록 레이에 맞은 모든 물체 중 바닥·벽을 찾는다
        foreach (RaycastHit hit in Physics.RaycastAll(transform.position, ray.direction))
        {
            if (hit.collider.CompareTag("Floor"))
            {
                _floorPos = hit.point;
                break;
            }
            if (hit.collider.CompareTag("Wall"))
            {
                _wallPos = hit.point;
                break;
            }
        }
    }

    /// <summary>누르기(선택) → 누른 채 드래그(이동) → 떼기(겹침이면 복귀) 순서로 가구를 처리한다.</summary>
    private void Furniture_Click()
    {
        Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, ray.direction, 1000);

        // 누르기: 위치 조정 중이 아니면 가구를 선택한다
        if (Input.GetMouseButtonDown(0) && !positioning)
        {
            foreach (RaycastHit hit in hits)
            {
                if (!hit.collider.CompareTag("Furniture")) continue;

                SelectedFurniture = hit.collider.gameObject;
                _check = SelectedFurniture.GetComponent<PositionCheck>();
                positioning = true;
                return;
            }
        }

        // 누른 채 드래그: 선택한 가구를 잡고, 잡은 뒤에는 바닥·벽 위치를 따라 이동한다
        if (Input.GetMouseButton(0))
        {
            if (!_isGrabbing)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.CompareTag("Furniture") && hit.collider.gameObject == SelectedFurniture)
                        Obj_Grab();
                }
            }
            else if (SelectedFurniture.layer == LayerMask.NameToLayer("Floor"))
            {
                Set_FurniturePosition(_floorPos);
            }
            else if (SelectedFurniture.layer == LayerMask.NameToLayer("Wall"))
            {
                Set_FurniturePosition(_wallPos);
            }
        }

        // 떼기: 다른 물체와 겹친 위치면 잡기 전 위치로 되돌린다
        if (Input.GetMouseButtonUp(0))
        {
            if (_isGrabbing && _check.Crash)
            {
                Set_FurniturePosition(_startPos);
                _outline.OutlineColor = Color.yellow;
            }

            _isGrabbing = false;
        }
    }

    /// <summary>가구를 잡고 위치 조정을 시작한다.</summary>
    private void Obj_Grab()
    {
        _isGrabbing = true;
        ObjAdjust_Start();
    }

    /// <summary>선택한 가구의 위치를 옮긴다.</summary>
    private void Set_FurniturePosition(Vector3 position)
    {
        SelectedFurniture.transform.position = position;
    }

    /// <summary>
    /// 위치 조정을 시작한다. 잡기 전 위치를 저장하고, 아웃라인을 켠 뒤 조정 UI를 표시한다.
    /// 수납함에서 가구를 꺼낼 때도 FurnitureManager에서 호출한다.
    /// </summary>
    public void ObjAdjust_Start()
    {
        positioning = true;
        _startPos = SelectedFurniture.transform.position;

        _check = SelectedFurniture.GetComponent<PositionCheck>();
        _outline = SelectedFurniture.GetComponent<Outline>();
        _outline.enabled = true;

        Close_Inventory();

        ObjAdjustUI.SetActive(true);
        inventoryBtn.SetActive(false);

        FurnitureName.text = SelectedFurniture.GetComponent<Furniture_Inven_Index>().Name;
    }

    /// <summary>위치 조정을 끝내고 조정 UI를 닫는다.</summary>
    public void ObjAdjust_End()
    {
        positioning = false;
        ObjAdjustUI.SetActive(false);
        inventoryBtn.SetActive(true);
    }

    /// <summary>조정 UI의 취소 버튼. 선택한 가구를 수납함으로 되돌린다(삭제).</summary>
    public void Furniture_cancle()
    {
        ObjAdjust_End();
        _furnitureManager.Furniture_Delete(SelectedFurniture);
        Destroy(SelectedFurniture);
        _isGrabbing = false;
    }

    /// <summary>조정 UI의 회전 버튼. 선택한 가구를 Y축으로 90도 회전한다.</summary>
    public void Furniture_rotate()
    {
        SelectedFurniture.transform.Rotate(0f, 90f, 0f, Space.World);
    }

    /// <summary>조정 UI의 확정 버튼. 다른 물체와 겹치지 않을 때만 배치를 확정한다.</summary>
    public void Furniture_complete()
    {
        if (_check.Crash) return;

        ObjAdjust_End();
        _outline.enabled = false;

        SelectedFurniture = null;
        positioning = false;
    }

    /// <summary>위치 조정 중 겹침 여부에 따라 아웃라인 색을 바꾼다. (배치 가능 = 노란색, 배치 불가 = 빨간색)</summary>
    public void CrashCheck()
    {
        if (SelectedFurniture == null || _outline == null) return;

        _outline.OutlineColor = _check.Crash ? Color.red : Color.yellow;
    }

    /// <summary>가구 수납함을 연다. 열려 있는 동안 가구 선택을 막는다.</summary>
    public void Open_Inventory()
    {
        inventory.SetActive(true);
        _useInven = true;
    }

    /// <summary>가구 수납함을 닫는다.</summary>
    public void Close_Inventory()
    {
        inventory.SetActive(false);
        _useInven = false;
    }

    // ── 오브젝트 상호작용 (인테리어 모드가 아닐 때) ──────

    /// <summary>서재 카메라 기준으로 IClickable 오브젝트를 강조하고, 클릭이 확정되면 기능을 실행한다.</summary>
    private void Obj_Interactive()
    {
        if (UsingUI) return;

        Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            _hoverHandler.Clear();
            return;
        }

        GameObject hitObj = hit.collider.gameObject;
        _hoverHandler.UpdateHover(hitObj);

        if (_hoverHandler.TryGetClickTarget(hitObj, out IClickable target))
        {
            // 열리는 팝업 UI를 쓰는 동안 오브젝트 상호작용을 막는다 (팝업을 닫을 때 해제)
            UsingUI = true;
            target.OnClick();
            _hoverHandler.Clear();
        }
    }

    /// <summary>강조와 이름 표시를 지운다.</summary>
    public void Clear()
    {
        _hoverHandler.Clear();
    }
}
