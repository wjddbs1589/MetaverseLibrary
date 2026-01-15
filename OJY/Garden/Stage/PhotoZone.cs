using Suncheon;
using Photon.Pun;
using Suncheon.Player;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using AlmostEngine.Screenshot;

public class PhotoZone : MonoBehaviour, Clickable
{
    [Header("Clickable ??????? ???")]
    [SerializeField] string obj_name = "??????";
    bool useNow = false;  //???????? ?????? ?????? true

    [Header("?????? ????")]
    [SerializeField] Camera photoZoneCam;
    Vector3 ExPosition; //?÷?????? ??? ???

    [Header("?÷???? ???")]
    [SerializeField] Transform PhotoPos;

    [Header("?????? ?????")]
    [SerializeField] GameObject UI_PhotoZone;

    [Header("???? ?????")]
    [SerializeField] Image BG;

    [Header("???? ?????")]
    [SerializeField] Sprite BG0;
    [SerializeField] Sprite BG1;
    [SerializeField] Sprite BG2;
    [SerializeField] Sprite BG3;

    PlayerManager playerManager;
    PlayerMoveManager moveManager;
    PlayerObjInteraction playerObjInteraction;
    PlayerAnimManager animManager;

    [Header("?????? UI")]
    [SerializeField] Button Btn_hide;
    [SerializeField] Button Btn_capture;
    [SerializeField] Button Btn_return;

    [Header("?????? ???? ???")]
    [SerializeField] Button Btn_background_0;
    [SerializeField] Button Btn_background_1;
    [SerializeField] Button Btn_background_2;
    [SerializeField] Button Btn_background_3;

    [Header("?????? ???? ???")]
    [SerializeField] Button Btn_pose_0;
    [SerializeField] Button Btn_pose_1;
    [SerializeField] Button Btn_pose_2;
    [SerializeField] Button Btn_pose_3;

    Animator anim;

    private List<GameObject> otherPlayers = new List<GameObject>();

    PlayerNameController nameController;

    public byte[] screenShotData;

    [SerializeField] GameObject JoyStick;

    ScreenshotManager screenshotManager;

    public string Return_ObjName()
    {
        return obj_name;
    }

    void Awake()
    {
        Btn_hide.onClick.AddListener(() => Btn_Hide());
        Btn_capture.onClick.AddListener(() => Btn_Capture());
        Btn_return.onClick.AddListener(() => Btn_Return());

        Btn_background_0.onClick.AddListener(() => Btn_Background_0());
        Btn_background_1.onClick.AddListener(() => Btn_Background_1());
        Btn_background_2.onClick.AddListener(() => Btn_Background_2());
        Btn_background_3.onClick.AddListener(() => Btn_Background_3());

        Btn_pose_0.onClick.AddListener(() => Btn_Pose_0());
        Btn_pose_1.onClick.AddListener(() => Btn_Pose_1()); 
        Btn_pose_2.onClick.AddListener(() => Btn_Pose_2()); 
        Btn_pose_3.onClick.AddListener(() => Btn_Pose_3());

        screenshotManager = GetComponent<ScreenshotManager>();
    }

    private void Update()
    {
        if (useNow)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim = NetworkManager.Instance.Go_Player.GetComponentInChildren<Animator>();
                On_UI();
            }
        }
    }

    public void OnClick()
    {
        if (!useNow)
        {
            playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>(); //?÷???? ???? ????
            nameController = playerManager.Canvas_PlayerUI;
            ExPosition = playerManager.transform.position;   //?÷???? ???? ??? ????    
            moveManager = playerManager.GetComponent<PlayerMoveManager>(); 
            playerObjInteraction = playerManager.GetComponent<PlayerObjInteraction>();
            animManager = playerManager.GetComponent<PlayerAnimManager>();

            Use_PhotoZone();
        }
    }    

    /// <summary>
    /// ?????? ??????
    /// </summary>
    void Use_PhotoZone()
    {
        Off_otherPlayer();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        JoyStick.SetActive(false);
#endif

        useNow = true;
        playerManager.VehicleOnOff(false);   //??? ??? ????
        animManager.SetBookSit(false);       //??????? ???? ????

        Settting_PhotoZone(true);

        InteractionManager.Inst.Ray_Off();
        animManager.SetIdle();
        playerObjInteraction.enabled = false;
        moveManager.Cam_Off();                          //???? ???? off
        playerManager.transform.position = PhotoPos.position;  //?÷???? ??? ????
        photoZoneCam.gameObject.SetActive(true);        //???? ????
                
        Vector3 direction = photoZoneCam.transform.position - moveManager.go_Char.transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        moveManager.go_Char.transform.rotation = rotation;

        playerManager.ui_NickName.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

        UI_PhotoZone.SetActive(true);
    }

    void End_PhotoZone()
    {
        On_otherPlayer();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        JoyStick.SetActive(true);
#endif

        Settting_PhotoZone(false);

        anim.SetBool("UsePhotoZone", false);
        InteractionManager.Inst.Ray_On();
        animManager.SetIdle();
        playerManager.ui_NickName.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        moveManager.Cam_On();
        playerManager.transform.position = ExPosition;
        photoZoneCam.gameObject.SetActive(false);
        UI_PhotoZone.SetActive(false);
        playerObjInteraction.enabled = true;
        useNow = false;
    }

    /// <summary>
    /// ?????? ???????? ???? ????
    /// </summary>
    /// <param name="usePhotoZone">true??? ?????? ????</param>
    void Settting_PhotoZone(bool usePhotoZone)
    {
        if (usePhotoZone)
        {
            nameController.Set_PhotoZoneCam(photoZoneCam);
            nameController.Look_PlayerCam(false);
        }
        else
        {
            nameController.Look_PlayerCam(true);
            nameController.Set_PhotoZoneCam(null);

        }
    }

    #region ??????? ??????/??????   
    /// <summary>
    /// ? ???? ?????
    /// </summary>
    void Off_otherPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        otherPlayers.Clear();

        foreach (GameObject obj in players)
        {
            if (!obj.GetPhotonView().IsMine)
            {
                obj.SetActive(false);
                otherPlayers.Add(obj);
            }
        }
    }

    /// <summary>
    /// ? ???? ???
    /// </summary>
    void On_otherPlayer()
    {
        foreach (GameObject obj in otherPlayers)
        {
            obj.SetActive(true);
        }
    }
    #endregion

    void Btn_Capture()
    {
        CaptureScreenshot();
    }

    void Btn_Hide()
    {
        UI_PhotoZone.SetActive(false);
    }

    void On_UI()
    {
        anim.SetBool("UsePhotoZone", true);
        UI_PhotoZone.SetActive(true);
    }

    void Btn_Return()
    {
        End_PhotoZone();
    }

    void Btn_Background_0()
    {
        Set_Invisible(0);
        BG.sprite = BG0;
    }

    void Btn_Background_1() 
    {
        Set_Invisible(1);
        BG.sprite = BG1;
    }

    void Btn_Background_2()
    {
        Set_Invisible(1);
        BG.sprite = BG2;
    }

    void Btn_Background_3()
    {
        Set_Invisible(1);
        BG.sprite = BG3;
    }
    void Set_Invisible(float amount)
    {
        Color color = BG.color;
        color.a = amount;
        BG.color = color;
    }

    void Btn_Pose_0()
    {
        anim.SetTrigger("Photo_Pose0");
    }
    void Btn_Pose_1() 
    {   
        anim.SetTrigger("Photo_Pose1");
    }
    void Btn_Pose_2() 
    {
        anim.SetTrigger("Photo_Pose2");
    }
    void Btn_Pose_3() 
    {
        anim.SetTrigger("Photo_Pose3");
    }

    void Btn_Reset()
    {
        anim.SetTrigger("Idle");
    }

    void CaptureScreenshot()
    {
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16);
        rt.width = 720;
        rt.height = 540;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        // ĸo?? ????? RenderTexture ????
        photoZoneCam.targetTexture = rt;

        // ???? ?????? ????
        photoZoneCam.Render();

        Texture2D screenshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        screenshot.Apply();

        // RenderTexture ????
        RenderTexture.active = null;
        photoZoneCam.targetTexture = null;
        Destroy(rt);

        screenShotData = screenshot.EncodeToJPG(); ;

        // ????????? ????? ????
        string screenshotName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";

#if UNITY_EDITOR
        //byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{Application.dataPath}\\{screenshotName}", screenShotData);
#elif !UNITY_EDITOR && UNITY_WEBGL
        string imageDataString = Convert.ToBase64String(screenShotData);
        //string FilePos = Application.persistentDataPath + "/저장할폴더이름";   
        Application.ExternalCall("DownLoadImage", imageDataString, screenshotName);
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        //string filePath = System.IO.Path.Combine(Application.persistentDataPath, "/ScreenShots/", screenshotName);
        //System.IO.File.WriteAllBytes(filePath, screenShotData);
        if (screenshotManager) 
        {
			screenshotManager.Capture ();
		}
#endif        
    }

}
