using Newtonsoft.Json.Linq;
using Suncheon;
using Suncheon.Player;
using Suncheon.WebData;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 개인서재 인테리어(가구 배치, 벽지·바닥 색상)의 생성·저장·불러오기를 관리한다.
///
/// 저장 구조:
///   가구별 위치(x, y, z), Y축 회전, 가구 인덱스와 벽지·바닥 RGB를 Room_InteriorData로 묶어 JSON 문자열로 직렬화하고,
///   서버의 서재 데이터(privateBook) 필드 하나에 저장한다.
///
/// 불러오기:
///   내 서재면 내 데이터를, 다른 사람의 서재면 방 주인의 데이터를 불러온다.
///   저장된 데이터가 없는 신규 유저는 기본 인테리어 상태를 바로 저장해 초기 데이터를 만든다.
///
/// 숫자 직렬화:
///   기기의 지역 설정에 따라 소수점이 쉼표로 바뀌지 않도록 InvariantCulture로 변환한다.
/// </summary>
public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    [Header("가구")]
    [Tooltip("가구 프리팹. 배열 순서가 가구 인덱스이며 FurnitureNumber 순서와 같아야 한다.")]
    [FormerlySerializedAs("FuniturePrefabs")]
    public GameObject[] FurniturePrefabs;

    /// <summary>인덱스별로 현재 배치된 가구 (없으면 null). 가구는 종류별로 하나씩만 배치할 수 있다.</summary>
    [HideInInspector] public GameObject[] spawnedFurniture;

    /// <summary>벽지 머티리얼</summary>
    [HideInInspector] public Material wallMaterial;

    /// <summary>바닥 머티리얼</summary>
    [HideInInspector] public Material floorMaterial;

    [Header("생성 위치")]
    [Tooltip("바닥 가구를 처음 생성할 위치")]
    public Transform FloorSpawnPos;

    [Tooltip("벽 가구(액자 등)를 처음 생성할 위치")]
    public Transform wallSpawnPos;

    [Header("인테리어 종료 버튼")]
    [Tooltip("저장하지 않고 나가기")]
    [SerializeField] private Button btn_save_N;

    [Tooltip("저장하고 나가기")]
    [SerializeField] private Button btn_save_Y;

    [Header("연결 대상")]
    [Tooltip("가구 선택·이동을 처리하는 컴포넌트")]
    [SerializeField] private FurnitureMove furnitureMove;

    [Tooltip("인테리어 모드 UI")]
    [SerializeField] private GameObject Interior;

    [Tooltip("플레이어 모드 UI")]
    [SerializeField] private GameObject Player;

    [Header("기본 색상")]
    public Color wall_OriginColor;
    public Color floor_OriginColor;

    /// <summary>인테리어 모드 사용 중 여부</summary>
    [HideInInspector] public bool usingInterior;

    private PlayerMoveManager _moveManager;
    private CameraManager _cameraManager;

    /// <summary>가구 인덱스를 설정하고 색상을 초기화한 뒤, 서재 주인에 맞는 인테리어를 불러온다.</summary>
    private void Awake()
    {
        Instance = this;

        Set_FurnitureIndex();
        spawnedFurniture = new GameObject[FurniturePrefabs.Length];

        btn_save_N.onClick.AddListener(Btn_Save_N);
        btn_save_Y.onClick.AddListener(Btn_Save_Y);

        Material_Reset();

        if (NetworkManager.Instance.Check_MyRoom())
            Load_MyFurniture();
        else
            Load_UserFurniture();
    }

    // ── 불러오기 ─────────────────────────────────────────

    /// <summary>
    /// 내 서재의 인테리어를 불러온다.
    /// 저장된 데이터가 없으면 신규 유저로 보고 기본 인테리어를 저장한다.
    /// </summary>
    public void Load_MyFurniture()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.MyFurnitureLoadUrl}";
        StartCoroutine(UTILS.Requset_HttpGetData(url, jsonData =>
        {
            if (!TryParseInteriorData(jsonData, out Room_InteriorData data))
            {
                SaveMyFurnitureList();
                return;
            }

            ApplyInterior(data);
        }));
    }

    /// <summary>방문한 서재 주인의 인테리어를 불러온다.</summary>
    private void Load_UserFurniture()
    {
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userFurnitureLoad}";
        Room_OwnerId ownerId = new Room_OwnerId(NetworkManager.Instance.user_id);

        StartCoroutine(UTILS.Requset_HttpPostData(url, ownerId, jsonData =>
        {
            if (TryParseInteriorData(jsonData, out Room_InteriorData data))
                ApplyInterior(data);
        }));
    }

    /// <summary>
    /// 서버 응답(JSON 배열)에서 서재 데이터를 꺼내고, 그 안의 privateBook 문자열을 인테리어 데이터로 변환한다.
    /// 응답이 비어 있거나 형식이 맞지 않으면 false를 반환한다.
    /// </summary>
    private static bool TryParseInteriorData(string jsonData, out Room_InteriorData data)
    {
        data = null;
        try
        {
            JArray jArray = JArray.Parse(jsonData);
            Get_RoomData roomData = JsonUtility.FromJson<Get_RoomData>(jArray.First().ToString());
            data = JsonUtility.FromJson<Room_InteriorData>(roomData.privateBook);
        }
        catch (Exception e)
        {
            UTILS.Log(e.ToString());
            return false;
        }

        return data != null;
    }

    /// <summary>불러온 인테리어 데이터로 색상과 가구를 배치한다.</summary>
    private void ApplyInterior(Room_InteriorData data)
    {
        Set_Color(data);

        foreach (Furniture_TransformData furniture in data.response_FurnitureDatas)
            SpawnSavedFurniture(furniture);
    }

    /// <summary>저장된 색상이 있으면 적용하고, 없으면 기본 색상을 적용한다.</summary>
    private void Set_Color(Room_InteriorData data)
    {
        floorMaterial.color = data.floorColor_R != null
            ? new Color(ParseFloat(data.floorColor_R), ParseFloat(data.floorColor_G), ParseFloat(data.floorColor_B))
            : floor_OriginColor;

        wallMaterial.color = data.wallColor_R != null
            ? new Color(ParseFloat(data.wallColor_R), ParseFloat(data.wallColor_G), ParseFloat(data.wallColor_B))
            : wall_OriginColor;
    }

    /// <summary>저장된 위치와 회전으로 가구를 생성한다.</summary>
    private void SpawnSavedFurniture(Furniture_TransformData data)
    {
        int index = int.Parse(data.FurnitureIndex);

        GameObject obj = Instantiate(FurniturePrefabs[index]);
        spawnedFurniture[index] = obj;

        obj.transform.position = new Vector3(ParseFloat(data.posX), ParseFloat(data.posY), ParseFloat(data.posZ));
        obj.transform.rotation = Quaternion.Euler(0, ParseFloat(data.rotY), 0);

        if (usingInterior)
            Interior_Off();
    }

    // ── 가구 생성·삭제 ───────────────────────────────────

    /// <summary>각 가구 프리팹에 배열 인덱스를 기록한다. 저장 시 어떤 가구인지 구분하는 데 사용한다.</summary>
    private void Set_FurnitureIndex()
    {
        for (int i = 0; i < FurniturePrefabs.Length; i++)
            FurniturePrefabs[i].GetComponent<Furniture_Inven_Index>().InventoryIndex = i;
    }

    /// <summary>
    /// 수납함에서 가구를 꺼낸다. 가구 레이어(바닥·벽)에 맞는 위치에 생성하고 바로 위치 조정을 시작한다.
    /// </summary>
    public void Spawn_Furniture(int index)
    {
        GameObject obj = Instantiate(FurniturePrefabs[index]);
        spawnedFurniture[index] = obj;

        if (obj.layer == LayerMask.NameToLayer("Floor"))
            obj.transform.position = FloorSpawnPos.position;
        else if (obj.layer == LayerMask.NameToLayer("Wall"))
            obj.transform.position = wallSpawnPos.position;

        furnitureMove.SelectedFurniture = obj;
        furnitureMove.ObjAdjust_Start();
    }

    /// <summary>배치 목록에서 가구를 제거한다. (오브젝트 파괴는 호출한 쪽에서 처리)</summary>
    public void Furniture_Delete(GameObject selectedObj)
    {
        spawnedFurniture[Return_FurnitureIndex(selectedObj)] = null;
    }

    /// <summary>가구 오브젝트의 인덱스를 반환한다.</summary>
    private static int Return_FurnitureIndex(GameObject furniture)
    {
        return furniture.GetComponent<Furniture_Inven_Index>().InventoryIndex;
    }

    /// <summary>배치된 모든 가구를 제거하고 수납함을 닫는다.</summary>
    public void All_Destroy()
    {
        for (int i = 0; i < spawnedFurniture.Length; i++)
        {
            if (spawnedFurniture[i] != null) Destroy(spawnedFurniture[i]);
            spawnedFurniture[i] = null;
        }

        furnitureMove.Close_Inventory();
    }

    /// <summary>배치된 가구가 하나 이상 있으면 true를 반환한다.</summary>
    public bool Is_Furniture()
    {
        return spawnedFurniture.Any(obj => obj != null);
    }

    // ── 저장 ─────────────────────────────────────────────

    /// <summary>저장하고 나가기 버튼. 현재 배치를 저장하고 인테리어 모드를 끝낸다.</summary>
    public void Btn_Save_Y()
    {
        Player_Physics_On();
        SaveMyFurnitureList();
        Interior_Off();
    }

    /// <summary>저장하지 않고 나가기 버튼. 현재 배치를 버리고 마지막으로 저장된 배치를 다시 불러온다.</summary>
    public void Btn_Save_N()
    {
        Player_Physics_On();
        All_Destroy();
        Load_MyFurniture();
        Interior_Off();
    }

    /// <summary>배치된 가구와 벽지·바닥 색상을 JSON으로 직렬화해 서버에 저장한다.</summary>
    public void SaveMyFurnitureList()
    {
        Room_InteriorData interior = new Room_InteriorData();

        foreach (GameObject obj in spawnedFurniture)
        {
            if (obj == null) continue;

            Vector3 pos = obj.transform.position;
            interior.response_FurnitureDatas.Add(new Furniture_TransformData
            {
                posX = ToInvariant(pos.x),
                posY = ToInvariant(pos.y),
                posZ = ToInvariant(pos.z),
                rotY = ToInvariant(obj.transform.eulerAngles.y),
                FurnitureIndex = Return_FurnitureIndex(obj).ToString()
            });
        }

        interior.wallColor_R = ToInvariant(wallMaterial.color.r);
        interior.wallColor_G = ToInvariant(wallMaterial.color.g);
        interior.wallColor_B = ToInvariant(wallMaterial.color.b);

        interior.floorColor_R = ToInvariant(floorMaterial.color.r);
        interior.floorColor_G = ToInvariant(floorMaterial.color.g);
        interior.floorColor_B = ToInvariant(floorMaterial.color.b);

        Request_SaveFurniture request = new Request_SaveFurniture(JsonUtility.ToJson(interior));
        string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.FurnitureSaveUrl}";

        StartCoroutine(UTILS.Requset_HttpPostData(url, request, jsonData =>
        {
            Response_SaveFurniture response = JsonUtility.FromJson<Response_SaveFurniture>(jsonData);
            UTILS.Log($"[가구 저장] {response.rtnCode} {response.rtnMsg}");
        }));
    }

    // ── 인테리어 모드 종료 ───────────────────────────────

    /// <summary>인테리어 모드를 끝내고 플레이어 조작과 서재 카메라 설정을 되돌린다.</summary>
    private void Interior_Off()
    {
        if (_moveManager == null || _cameraManager == null)
        {
            _moveManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerMoveManager>();
            _cameraManager = NetworkManager.Instance.Go_Player.GetComponentInChildren<CameraManager>();
        }

        usingInterior = false;
        Set_PlayerMove(true);
        Interior.SetActive(false);
        Player.SetActive(true);

        FindAnyObjectByType<RoomCameraSetting>().Off_PlayerInteractive();
    }

    /// <summary>벽지·바닥 머티리얼을 기본 색상으로 되돌린다.</summary>
    private void Material_Reset()
    {
        wallMaterial.color = wall_OriginColor;
        floorMaterial.color = floor_OriginColor;
    }

    /// <summary>플레이어 이동과 카메라 조작 가능 여부를 설정한다.</summary>
    private void Set_PlayerMove(bool on)
    {
        _cameraManager.enabled = on;
        _moveManager.enabled = on;
    }

    /// <summary>인테리어 모드에서 꺼 두었던 플레이어의 충돌, 중력, 오브젝트 상호작용을 다시 켠다.</summary>
    private void Player_Physics_On()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!obj.TryGetComponent(out PlayerObjInteraction interaction)) continue;

            interaction.enabled = true;
            obj.GetComponent<Collider>().enabled = true;
            obj.GetComponent<Rigidbody>().useGravity = true;
        }

        InteractionManager.Inst.Ray_On();
    }

    // ── 숫자 변환 ────────────────────────────────────────

    /// <summary>지역 설정과 무관하게 소수점(.)을 사용하는 문자열로 변환한다.</summary>
    private static string ToInvariant(float value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>소수점(.)을 사용하는 문자열을 지역 설정과 무관하게 float로 변환한다.</summary>
    private static float ParseFloat(string value) => float.Parse(value, CultureInfo.InvariantCulture);
}
