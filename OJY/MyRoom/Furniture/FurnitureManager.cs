using Suncheon.Player;
using Suncheon;
using Suncheon.WebData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class FurnitureManager : MonoBehaviour
{
    private static FurnitureManager instance = null;
    public static FurnitureManager Instance => instance;

    [Header("가구 프리펩")]
    public GameObject[] FuniturePrefabs;

    [Header("생성된 가구 목록")]
    public GameObject[] spawnedFuniture;

    [Header("벽지의 Material")]
    [HideInInspector] public Material wallMaterial;
    [HideInInspector] public Material floorMaterial;

    //벽지색상의 각 RGB값
    float WR;
    float WG;
    float WB;
    float FR;
    float FG;
    float FB;

    [Header("생성될 위치 목록")]
    public Transform FloorSpawnPos;
    public Transform wallSpawnPos;

    [Header("나가기 버튼")]
    [SerializeField] Button btn_save_N;
    [SerializeField] Button btn_save_Y;

    [Header("가구 위치 변경")]
    [SerializeField] FurnitureMove furnitureMove;

    [Header("가구 선택 버튼")]
    [SerializeField] Transform content;

    [Header("인테리어 전환")]
    [SerializeField] GameObject Interior;
    [SerializeField] GameObject Player;

    [Header("시작 색상")]
    public Color wall_OriginColor;
    public Color floor_OriginColor;

    [Header("인테리어 사용 여부")]
    public bool usingInterior;

    PlayerMoveManager moveManager;
    CameraManager cameraManager;
    private void Awake()
    {
        instance = this;

        Set_FurnitureIndex();
        spawnedFuniture = new GameObject[FuniturePrefabs.Length];

        btn_save_N.onClick.AddListener(() => Btn_Save_N());
        btn_save_Y.onClick.AddListener(() => Btn_Save_Y());

        Material_Reset();

        //id가 다르면 타유저의 가구 불러오기
        if (NetworkManager.Instance.Check_MyRoom())
        {
            Load_MyFurniture();
        }
        else
        {
            Load_UserFurniture();
        }
    }

    #region 나의 가구 불러오기
    /// <summary>
    /// 나의 가구정보 가져오기
    /// </summary>
    public void Load_MyFurniture()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.MyFurnitureLoadUrl}", (jsonData) =>
        {
            //json데이터 가져옴
            JArray jArray = null; 
            try
            {
                jArray = JArray.Parse(jsonData);
            }
            catch(Exception e)
            {
                UTILS.Log(e.ToString());
                SaveMyFurnitureList();
                return;
            }

            Get_RoomData respons_RoomLoadData = null;
            try
            {
                respons_RoomLoadData = JsonUtility.FromJson<Get_RoomData>(jArray.First().ToString()); //인테리어에 대한 정보 파일 가져옴
            }
            catch (Exception e)
            {
                UTILS.Log(e.ToString());
                SaveMyFurnitureList();
                return;
            }

            Room_InteriorData response_FurnitureLoad = null; //가구정보를 가져옴
            try
            {
                response_FurnitureLoad = JsonUtility.FromJson<Room_InteriorData>(respons_RoomLoadData.privateBook);
            }
            catch (Exception e)
            {
                UTILS.Log(e.ToString());
                SaveMyFurnitureList();
                return;
            }

            Set_Color(response_FurnitureLoad);
            Set_Furniture(response_FurnitureLoad.response_FurnitureDatas);
        }));
    }
    #endregion    

    #region 타 유저의 가구 불러오기
    /// <summary>
    /// 타 유저의 가구정보 가져오기
    /// </summary>
    void Load_UserFurniture()
    {
        Room_OwnerId id = new Room_OwnerId(NetworkManager.Instance.user_id);
        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.userFurnitureLoad}"
            ,id, (jsonData) =>
            {
                //json데이터 가져옴
                JArray jArray = null;
                try
                {
                    jArray = JArray.Parse(jsonData);
                }
                catch (Exception e)
                {
                    UTILS.Log(e.ToString());
                    return;
                }

                Get_RoomData respons_RoomLoadData = null;
                try
                {
                    respons_RoomLoadData = JsonUtility.FromJson<Get_RoomData>(jArray.First().ToString()); //인테리어에 대한 정보 파일 가져옴
                }
                catch (Exception e)
                {
                    UTILS.Log(e.ToString());
                    return;
                }

                Room_InteriorData response_FurnitureLoad = null; //가구정보를 가져옴
                try
                {
                    response_FurnitureLoad = JsonUtility.FromJson<Room_InteriorData>(respons_RoomLoadData.privateBook);
                }
                catch (Exception e)
                {
                    UTILS.Log(e.ToString());
                    return;
                }

                Set_Color(response_FurnitureLoad);
                Set_Furniture(response_FurnitureLoad.response_FurnitureDatas);
            }));       
    }
    #endregion

    #region 벽지 색상 관련
    /// <summary>
    /// 저장된 색상이 있는지 확인하고 색상을 적용
    /// </summary>
    /// <param name="data">색상 정보</param>
    void Set_Color(Room_InteriorData data)
    {
        //벽지 저장정보가 있으면 적용
        if (data.floorColor_R != null)
        {
            FR = float.Parse(data.floorColor_R);
            FG = float.Parse(data.floorColor_G);
            FB = float.Parse(data.floorColor_B);
            Color C_Floor = new Color(FR, FG, FB);
            floorMaterial.color = C_Floor;
        }
        //없으면 기본색상 적용
        else
        {
            floorMaterial.color = floor_OriginColor;
        }

        if (data.wallColor_R != null)
        {
            WR = float.Parse(data.wallColor_R);
            WG = float.Parse(data.wallColor_G);
            WB = float.Parse(data.wallColor_B);
            Color C_Wall = new Color(WR, WG, WB);
            wallMaterial.color = C_Wall;
        }
        else
        {
            wallMaterial.color = wall_OriginColor;
        }
    }
    #endregion

    #region 가구정보 확인
    /// <summary>
    /// 가구정보 읽기
    /// </summary>
    /// <param name="response_FurnitureLoad"></param>
    void Set_Furniture(List<Furniture_TransformData> datas)
    {
        //가구정보를 가져옴
        foreach (Furniture_TransformData obj in datas)
        {
            SpawnSavedFurniture(obj);
        }
    }

    /// <summary>
    /// 가구 생성
    /// </summary>
    /// <param name="data"></param>
    void SpawnSavedFurniture(Furniture_TransformData data)
    {
        UTILS.Log($"가구 인덱스 {data.FurnitureIndex}");

        float posX = float.Parse(data.posX);
        float posY = float.Parse(data.posY);
        float posZ = float.Parse(data.posZ);
        float rotY = float.Parse(data.rotY);
        int furnitureIndexnt = int.Parse(data.FurnitureIndex);

        GameObject obj = Instantiate(FuniturePrefabs[furnitureIndexnt]);
        spawnedFuniture[furnitureIndexnt] = obj;

        obj.transform.position = new Vector3(posX, posY, posZ);
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));

        if (usingInterior)
        {
            Interior_Off();
        }
    }
    #endregion


    /// <summary>
    /// 가구에 인덱스 번호 저장
    /// </summary>
    void Set_FurnitureIndex()
    {
        for (int i = 0; i < FuniturePrefabs.Length; i++)
        {
            Furniture_Inven_Index index = FuniturePrefabs[i].GetComponent<Furniture_Inven_Index>();
            index.InventoryIndex = i;
        }
    }

    /// <summary>
    /// 가구를 생성하고 배열에 추가
    /// </summary>
    /// <param name="index"></param>
    public void Spawn_Furniture(int index)
    {
        spawnedFuniture[index] = Instantiate(FuniturePrefabs[index]);  //인덱스에 맞는 가구 프리펩

        if (spawnedFuniture[index].layer == LayerMask.NameToLayer("Floor"))
        {
            spawnedFuniture[index].transform.position = FloorSpawnPos.position;
        }
        else if (spawnedFuniture[index].layer == LayerMask.NameToLayer("Wall"))
        {
            spawnedFuniture[index].transform.position = wallSpawnPos.position;
        }

        furnitureMove.SelectedFurniture = spawnedFuniture[index]; //오브젝트 생성 
        furnitureMove.ObjAdjust_Start(); //위치조정 시작
    }

    /// <summary>
    /// 배치를 완료하고 저장
    /// </summary>
    /// <param name="selectedObj"></param>
    public void Furniture_Set(GameObject selectedObj)
    {
        int furniture_Index = Return_FurnitureIndex(selectedObj);

        // 이미 리스트에 존재하는지 확인
        if (spawnedFuniture[furniture_Index] != selectedObj)
        {
            return;
        }
    }

    /// <summary>
    /// 가구 삭제
    /// </summary>
    /// <param name="selectedObj"></param>
    public void Furniture_Delete(GameObject selectedObj)
    {
        int index = Return_FurnitureIndex(selectedObj);

        spawnedFuniture[index] = null;
    }

    /// <summary>
    /// 가구의 인덱스를 반환
    /// </summary>
    /// <param name="furniture"></param>
    /// <returns></returns>
    private int Return_FurnitureIndex(GameObject furniture)
    {
        return furniture.GetComponent<Furniture_Inven_Index>().InventoryIndex;
    }

    /// <summary>
    /// 가구배치를 저장하고 나가는 버튼
    /// </summary>
    public void Btn_Save_Y()
    {
        Player_Physics_On();   //플레이어의 이동 제한 해제
        SaveMyFurnitureList(); //가구및 타일정보 저장
        Interior_Off();        
    }

    /// <summary>
    /// 배치한 가구를 저장
    /// </summary>
    public void SaveMyFurnitureList()
    {
        Room_InteriorData response_FurnitureList = new Room_InteriorData();
        for (int i = 0; i < spawnedFuniture.Length; i++)
        {
            if (spawnedFuniture[i] == null)
            {
                continue;
            }

            Furniture_TransformData response_FurnitureData = new Furniture_TransformData();

            response_FurnitureData.posX = spawnedFuniture[i].transform.position.x.ToString();
            response_FurnitureData.posY = spawnedFuniture[i].transform.position.y.ToString();
            response_FurnitureData.posZ = spawnedFuniture[i].transform.position.z.ToString();
            response_FurnitureData.rotY = spawnedFuniture[i].transform.eulerAngles.y.ToString();
            response_FurnitureData.FurnitureIndex = Return_FurnitureIndex(spawnedFuniture[i]).ToString();

            response_FurnitureList.response_FurnitureDatas.Add(response_FurnitureData);
        }

        response_FurnitureList.wallColor_R = wallMaterial.color.r.ToString();
        response_FurnitureList.wallColor_G = wallMaterial.color.g.ToString();
        response_FurnitureList.wallColor_B = wallMaterial.color.b.ToString();

        response_FurnitureList.floorColor_R = floorMaterial.color.r.ToString();
        response_FurnitureList.floorColor_G = floorMaterial.color.g.ToString();
        response_FurnitureList.floorColor_B = floorMaterial.color.b.ToString();

        string jsonData = $"{JsonUtility.ToJson(response_FurnitureList)}";

        Request_SaveFurniture request_SaveFurniture = new Request_SaveFurniture(jsonData);  

        StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.FurnitureSaveUrl}", request_SaveFurniture, (jsonData) =>
        {
            Response_SaveFurniture response_SaveFurniture = JsonUtility.FromJson<Response_SaveFurniture>(jsonData);
            SaveFurniture(response_SaveFurniture);
        }
        ));
    }
    public void SaveFurniture(Response_SaveFurniture _SaveFurniture)
    {
        UTILS.Log(_SaveFurniture.rtnMsg);
        UTILS.Log(_SaveFurniture.rtnCode);
    }

    /// <summary>
    /// 가구 배치 저장하지 않고 나가기 버튼
    /// </summary>
    public void Btn_Save_N()
    {
        Player_Physics_On(); //플레이어의 이동 제한 해제
        All_Destory();       //모든 가구 없애기
        Load_MyFurniture();  //이전 가구배치 불러오기
        Interior_Off();
    }

    /// <summary>
    /// 인테리어 창을 나갈때 실행
    /// </summary>
    void Interior_Off()
    {
        if (moveManager == null || cameraManager == null)
        {
            moveManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerMoveManager>();
            cameraManager = NetworkManager.Instance.Go_Player.GetComponentInChildren<CameraManager>();
        }

        usingInterior = false;
        Set_PlayerMove(true);
        Interior.SetActive(false);
        Player.SetActive(true);

        FindAnyObjectByType<RoomCameraSetting>().Off_PlayerInteractive();
    }


    /// <summary>
    /// 타일색상 초기화
    /// </summary>
    void Material_Reset()
    {
        wallMaterial.color = wall_OriginColor;
        floorMaterial.color = floor_OriginColor;
    }

    /// <summary>
    /// 모든 가구 오브젝트를 삭제하고 UI를 닫음
    /// </summary>
    public void All_Destory()
    {
        foreach (GameObject obj in spawnedFuniture)
        {
            Destroy(obj);
        }

        furnitureMove.Close_Inventory();
    }

    /// <summary>
    /// 플레이어 조작가능여부 설정
    /// </summary>
    /// <param name="on"></param>
    void Set_PlayerMove(bool on)
    {
        cameraManager.enabled = on;
        moveManager.enabled = on;
    }

    /// <summary>
    /// 플레이어의 충돌판정 및 물리효과 활성화
    /// </summary>
    void Player_Physics_On()
    {
        GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in playerObject)
        {
            PlayerObjInteraction poi = obj.GetComponent<PlayerObjInteraction>();
            if (poi != null)
            {
                poi.enabled = true;

                Collider c = obj.GetComponent<Collider>();
                c.enabled = true;

                Rigidbody r = obj.GetComponent<Rigidbody>();
                r.useGravity = true;
            }
            else
            {
                continue;
            }
        }

        InteractionManager.Inst.Ray_On();
    }

    /// <summary>
    /// 현재 배치된 가구가 있는지 확인
    /// </summary>
    /// <returns>true일때 가구가 1개이상 있다</returns>
    public bool Is_Furniture()
    {
        foreach (GameObject obj in spawnedFuniture)
        {
            if (obj != null)
            {
                return true;
            }
        }

        return false;
    }
}
