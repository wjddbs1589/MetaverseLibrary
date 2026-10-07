using AlmostEngine.Screenshot;
using Photon.Pun;
using Suncheon;
using Suncheon.Player;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 포토존. 클릭하면 촬영 모드로 전환하고, 배경·포즈를 골라 사진을 찍어 플랫폼에 맞게 저장한다.
///
/// 촬영 모드:
///   - 다른 플레이어를 숨겨 내 캐릭터만 찍히도록 한다
///   - 캐릭터를 촬영 위치로 옮기고 포토존 전용 카메라를 바라보게 한다
///   - 이동·카메라 조작과 오브젝트 상호작용을 막고, 이름표도 포토존 카메라를 향하게 한다
///   - UI 숨기기 버튼으로 화면을 가린 UI를 치울 수 있으며, 화면을 누르면 다시 표시된다
///
/// 저장 방식:
///   에디터         → 프로젝트 폴더에 파일로 저장
///   웹 (WebGL)     → 페이지의 JavaScript 함수(DownLoadImage)로 브라우저 다운로드
///   모바일         → 스크린샷 에셋으로 기기 갤러리에 저장
/// </summary>
public class PhotoZone : MonoBehaviour, IClickable
{
    // 촬영 이미지 크기
    private const int CAPTURE_WIDTH = 720;
    private const int CAPTURE_HEIGHT = 540;

    [Tooltip("마우스를 올렸을 때 표시할 이름")]
    [SerializeField] private string obj_name = "포토존";

    [Header("촬영")]
    [Tooltip("포토존 전용 카메라")]
    [SerializeField] private Camera photoZoneCam;

    [Tooltip("촬영 시 캐릭터를 세울 위치")]
    [SerializeField] private Transform PhotoPos;

    [Tooltip("포토존 UI")]
    [SerializeField] private GameObject UI_PhotoZone;

    [Tooltip("모바일 조이스틱 (촬영 중 숨김)")]
    [SerializeField] private GameObject JoyStick;

    [Header("배경")]
    [Tooltip("배경 이미지")]
    [SerializeField] private Image BG;

    [Tooltip("배경 선택 버튼. 0번은 배경 없음")]
    [SerializeField] private Button[] backgroundButtons;

    [Tooltip("배경 이미지 (backgroundButtons 순서)")]
    [SerializeField] private Sprite[] backgroundSprites;

    [Header("포즈")]
    [Tooltip("포즈 선택 버튼. i번 버튼은 Photo_Pose{i} 트리거를 실행한다.")]
    [SerializeField] private Button[] poseButtons;

    [Header("기능 버튼")]
    [SerializeField] private Button Btn_hide;
    [SerializeField] private Button Btn_capture;
    [SerializeField] private Button Btn_return;

    // 촬영 모드 사용 중 여부
    private bool _useNow = false;

    // 촬영 모드 진입 전 캐릭터 위치
    private Vector3 _prevPosition;

    // 촬영 모드 동안 숨긴 다른 플레이어
    private readonly List<GameObject> _hiddenPlayers = new List<GameObject>();

    // 내 플레이어 구성 요소
    private PlayerManager _playerManager;
    private PlayerMoveManager _moveManager;
    private PlayerObjInteraction _playerObjInteraction;
    private PlayerAnimManager _animManager;
    private PlayerNameController _nameController;
    private Animator _anim;

    private ScreenshotManager _screenshotManager;

    /// <summary>마우스를 올렸을 때 표시할 이름을 반환한다.</summary>
    public string Return_ObjName() => obj_name;

    /// <summary>버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        Btn_hide.onClick.AddListener(() => UI_PhotoZone.SetActive(false));
        Btn_capture.onClick.AddListener(CaptureScreenshot);
        Btn_return.onClick.AddListener(End_PhotoZone);

        for (int i = 0; i < backgroundButtons.Length; i++)
        {
            int index = i;
            backgroundButtons[i].onClick.AddListener(() => SetBackground(index));
        }

        for (int i = 0; i < poseButtons.Length; i++)
        {
            string trigger = $"Photo_Pose{i}";
            poseButtons[i].onClick.AddListener(() => _anim.SetTrigger(trigger));
        }

        _screenshotManager = GetComponent<ScreenshotManager>();
    }

    /// <summary>촬영 중 UI를 숨긴 상태에서 화면을 누르면 UI를 다시 표시한다.</summary>
    private void Update()
    {
        if (_useNow && Input.GetMouseButtonDown(0))
            ShowPhotoUI();
    }

    /// <summary>포토존을 클릭하면 내 플레이어 정보를 가져와 촬영 모드를 시작한다.</summary>
    public void OnClick()
    {
        if (_useNow) return;

        _playerManager = NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();
        _nameController = _playerManager.Canvas_PlayerUI;
        _moveManager = _playerManager.GetComponent<PlayerMoveManager>();
        _playerObjInteraction = _playerManager.GetComponent<PlayerObjInteraction>();
        _animManager = _playerManager.GetComponent<PlayerAnimManager>();
        _anim = _playerManager.GetComponentInChildren<Animator>();

        _prevPosition = _playerManager.transform.position;

        Use_PhotoZone();
    }

    // ── 촬영 모드 시작·종료 ──────────────────────────────

    /// <summary>다른 플레이어를 숨기고 캐릭터와 카메라를 촬영 상태로 전환한다.</summary>
    private void Use_PhotoZone()
    {
        HideOtherPlayers();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        JoyStick.SetActive(false);
#endif

        _useNow = true;

        // 탈것에서 내리고 앉아 있던 상태를 해제한다
        _playerManager.VehicleOnOff(false);
        _animManager.SetBookSit(false);
        _animManager.SetIdle();

        SetNameTagTarget(true);

        // 촬영 중에는 이동·카메라 조작과 오브젝트 상호작용을 막는다
        InteractionManager.Inst.Ray_Off();
        _playerObjInteraction.enabled = false;
        _moveManager.Cam_Off();

        // 캐릭터를 촬영 위치로 옮기고 포토존 카메라를 바라보게 한다
        _playerManager.transform.position = PhotoPos.position;
        photoZoneCam.gameObject.SetActive(true);

        Vector3 toCamera = photoZoneCam.transform.position - _moveManager.go_Char.transform.position;
        toCamera.y = 0;
        _moveManager.go_Char.transform.rotation = Quaternion.LookRotation(toCamera);

        // 카메라를 마주 보는 캐릭터의 이름표가 뒤집혀 보이지 않도록 180도 돌린다
        _playerManager.ui_NickName.transform.localRotation = Quaternion.Euler(0, 180, 0);

        UI_PhotoZone.SetActive(true);
    }

    /// <summary>촬영 모드를 끝내고 캐릭터 위치, 카메라, 조작, 다른 플레이어 표시를 되돌린다.</summary>
    private void End_PhotoZone()
    {
        ShowOtherPlayers();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        JoyStick.SetActive(true);
#endif

        SetNameTagTarget(false);

        _anim.SetBool("UsePhotoZone", false);
        _animManager.SetIdle();

        InteractionManager.Inst.Ray_On();
        _playerManager.ui_NickName.transform.localRotation = Quaternion.identity;
        _moveManager.Cam_On();
        _playerManager.transform.position = _prevPosition;

        photoZoneCam.gameObject.SetActive(false);
        UI_PhotoZone.SetActive(false);
        _playerObjInteraction.enabled = true;
        _useNow = false;
    }

    /// <summary>이름표가 바라볼 카메라를 포토존 카메라 또는 플레이어 카메라로 전환한다.</summary>
    private void SetNameTagTarget(bool usePhotoZone)
    {
        _nameController.Set_PhotoZoneCam(usePhotoZone ? photoZoneCam : null);
        _nameController.Look_PlayerCam(!usePhotoZone);
    }

    /// <summary>포토존 UI를 표시하고 촬영 대기 애니메이션을 켠다.</summary>
    private void ShowPhotoUI()
    {
        _anim.SetBool("UsePhotoZone", true);
        UI_PhotoZone.SetActive(true);
    }

    // ── 다른 플레이어 숨기기 ─────────────────────────────

    /// <summary>내 캐릭터만 찍히도록 다른 플레이어를 모두 숨긴다.</summary>
    private void HideOtherPlayers()
    {
        _hiddenPlayers.Clear();

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetPhotonView().IsMine) continue;

            player.SetActive(false);
            _hiddenPlayers.Add(player);
        }
    }

    /// <summary>숨겼던 다른 플레이어를 다시 표시한다.</summary>
    private void ShowOtherPlayers()
    {
        foreach (GameObject player in _hiddenPlayers)
        {
            if (player != null) player.SetActive(true);
        }
        _hiddenPlayers.Clear();
    }

    // ── 배경 ─────────────────────────────────────────────

    /// <summary>배경을 바꾼다. 0번은 배경 없음(투명)이다.</summary>
    private void SetBackground(int index)
    {
        Color color = BG.color;
        color.a = index == 0 ? 0f : 1f;
        BG.color = color;

        BG.sprite = backgroundSprites[index];
    }

    // ── 촬영·저장 ────────────────────────────────────────

    /// <summary>포토존 카메라 화면을 RenderTexture로 렌더링해 JPG로 만들고, 플랫폼에 맞게 저장한다.</summary>
    private void CaptureScreenshot()
    {
        RenderTexture rt = new RenderTexture(CAPTURE_WIDTH, CAPTURE_HEIGHT, 16);
        RenderTexture prevActive = RenderTexture.active;

        // 포토존 카메라를 RenderTexture에 한 번 렌더링하고 픽셀을 읽는다
        photoZoneCam.targetTexture = rt;
        photoZoneCam.Render();
        RenderTexture.active = rt;

        Texture2D screenshot = new Texture2D(CAPTURE_WIDTH, CAPTURE_HEIGHT, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, CAPTURE_WIDTH, CAPTURE_HEIGHT), 0, 0);
        screenshot.Apply();

        // 렌더링 대상을 원래대로 되돌리고 임시 리소스를 해제한다
        RenderTexture.active = prevActive;
        photoZoneCam.targetTexture = null;
        Destroy(rt);

        byte[] jpgData = screenshot.EncodeToJPG();
        Destroy(screenshot);

        string fileName = $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg";
        SaveScreenshot(jpgData, fileName);
    }

    /// <summary>플랫폼에 맞는 방식으로 사진을 저장한다.</summary>
    private void SaveScreenshot(byte[] jpgData, string fileName)
    {
#if UNITY_EDITOR
        File.WriteAllBytes(Path.Combine(Application.dataPath, fileName), jpgData);
#elif UNITY_WEBGL
        // 웹 페이지에 정의된 DownLoadImage(base64, fileName) 함수로 브라우저 다운로드를 실행한다
        Application.ExternalCall("DownLoadImage", Convert.ToBase64String(jpgData), fileName);
#elif UNITY_ANDROID || UNITY_IOS
        // 기기 갤러리 저장은 스크린샷 에셋에 맡긴다
        if (_screenshotManager != null)
            _screenshotManager.Capture();
#endif
    }
}
