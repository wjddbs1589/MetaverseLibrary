using Suncheon;
using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 첫 입장 시 표시하는 튜토리얼. 이전·다음 버튼으로 이미지를 넘긴다.
/// PC(웹)와 모바일은 조작 방법이 달라 플랫폼에 맞는 이미지 세트를 사용한다.
///
/// 처음 보는 유저가 튜토리얼을 닫으면 지도를 열어 이동할 곳을 안내한다.
/// 이미 본 유저는 씬에 들어와도 튜토리얼이 열리지 않는다.
/// </summary>
public class Tutorial : MonoBehaviour
{
    [Header("튜토리얼 이미지")]
    [Tooltip("PC·웹용 이미지")]
    [SerializeField] private Sprite[] Images_PC;

    [Tooltip("모바일용 이미지")]
    [SerializeField] private Sprite[] Images_MB;

    [Tooltip("이미지를 표시할 Image")]
    [SerializeField] private Image TargetImage;

    [Header("버튼")]
    [SerializeField] private Button btn_prev;
    [SerializeField] private Button btn_next;
    [SerializeField] private Button btn_close;

    [Tooltip("튜토리얼 팝업 오브젝트")]
    [SerializeField] private GameObject child_Obj;

    // 현재 플랫폼에 맞는 이미지 세트
    private Sprite[] _images;

    // 현재 페이지
    private int _page = 0;

    // 이번 입장에서 튜토리얼을 처음 보는지 여부 (닫을 때 지도 안내에 사용)
    private bool _isFirstTime;

    /// <summary>
    /// 처음 보는 유저인지 기록한 뒤 튜토리얼을 연다. 이미 본 유저면 바로 닫는다.
    /// 다음 입장부터는 열리지 않도록 본 것으로 표시한다.
    /// </summary>
    private void Start()
    {
        _images = GetPlatformImages();
        _isFirstTime = !GameManager.Instance.TutorialCheck;

        UIInteractionManager.Instance.OpenPopUp(child_Obj);
        if (!_isFirstTime)
            UIInteractionManager.Instance.ClosePopUp(child_Obj);

        GameManager.Instance.TutorialCheck = true;
        Set_Page(0);

        btn_prev.onClick.AddListener(() => Set_Page(_page - 1));
        btn_next.onClick.AddListener(Btn_Next);
        btn_close.onClick.AddListener(Btn_Close);
    }

    /// <summary>현재 플랫폼에 맞는 이미지 세트를 반환한다. (모바일 빌드만 모바일 이미지, 그 외는 PC 이미지)</summary>
    private Sprite[] GetPlatformImages()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        return Images_MB;
#else
        return Images_PC;
#endif
    }

    /// <summary>지정한 페이지를 표시한다. 첫 페이지에서는 이전 버튼을 숨긴다.</summary>
    private void Set_Page(int page)
    {
        _page = page;
        TargetImage.sprite = _images[_page];
        btn_prev.gameObject.SetActive(_page > 0);
    }

    /// <summary>다음 페이지로 넘긴다. 마지막 페이지였으면 튜토리얼을 닫는다.</summary>
    private void Btn_Next()
    {
        if (_page + 1 >= _images.Length)
            Exit_Tuto();
        else
            Set_Page(_page + 1);
    }

    /// <summary>닫기 버튼.</summary>
    private void Btn_Close()
    {
        btn_close.GetComponent<ButtonControl>().Btnoff();
        Exit_Tuto();
    }

    /// <summary>튜토리얼을 닫고 첫 페이지로 되돌린다. 처음 본 유저면 지도를 열어 안내한다.</summary>
    private void Exit_Tuto()
    {
        if (_isFirstTime)
        {
            _isFirstTime = false;
            UIInteractionManager.Instance.Btn_MapClick();
        }

        Set_Page(0);
        UIInteractionManager.Instance.ClosePopUp(child_Obj);
    }
}
