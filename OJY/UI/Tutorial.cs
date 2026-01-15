using UnityEngine.UI;
using UnityEngine;
using Suncheon.UI;
using Suncheon;

public class Tutorial : MonoBehaviour
{
    [Header("튜토리얼 이미지들")]
    [SerializeField] Sprite[] Images_PC;
    [SerializeField] Sprite[] Images_MB;


    [Header("변경될 이미지")]
    [SerializeField] Image TargetImage;

    [Header("페이지 버튼")]
    [SerializeField] Button btn_prev;
    [SerializeField] Button btn_next;
    [SerializeField] Button btn_close;


    int pageCount = 0;
    UIInteractionManager uIInteractionManager;
    [SerializeField] GameObject child_Obj;

    private void Start()
    {
        UTILS.Log($"튜토리얼 참여 여부 : {GameManager.Instance.TutorialCheck}");

        if(UIInteractionManager.Instance == null)
        {
            UTILS.Log("인스턴스 없음");
        }
        if (child_Obj == null)
        {
            UTILS.Log("오브젝트 없음");
        }

        UIInteractionManager.Instance.OpenPopUp(child_Obj);

        //튜토리얼 완료시
        if (GameManager.Instance.TutorialCheck)
        {
            UIInteractionManager.Instance.ClosePopUp(child_Obj); //씬 입장시 닫기
        }

        Set_Page(0);

        uIInteractionManager = FindAnyObjectByType<UIInteractionManager>();
        btn_prev.onClick.AddListener(() => Btn_Prev());
        btn_next.onClick.AddListener(() => Btn_Next());
        btn_close.onClick.AddListener(() => Btn_Close());
    }

    void Set_Page(int index)
    {
        GameManager.Instance.TutorialCheck = true; 
        pageCount = index;

#if !UNITY_EDITOR && UNITY_WEBGL
        TargetImage.sprite = Images_PC[pageCount]; //에디터 또는 WebGL일 때
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        TargetImage.sprite = Images_MB[pageCount]; //모바일 일 때
#else
        TargetImage.sprite = Images_PC[pageCount];
#endif

        if (pageCount == 0)
        {
            btn_prev.gameObject.SetActive(false);
        }
        else
        {
            btn_prev.gameObject.SetActive(true);
        }
    }

    void Btn_Prev()
    {
        pageCount--;
        Set_Page(pageCount);
    }

    void Btn_Next()
    {
        pageCount++;
        if (pageCount >= Images_PC.Length)
        {            
            Exit_Tuto();
        }
        else
        {
            Set_Page(pageCount);
        }        
    }
    void Btn_Close()
    {
        btn_close.GetComponent<ButtonControl>().Btnoff();
        Exit_Tuto();
    }
    void Exit_Tuto()
    {
        if (!GameManager.Instance.TutorialCheck)
        {
            uIInteractionManager.Btn_MapClick();
        }
        Set_Page(0);
        UIInteractionManager.Instance.ClosePopUp(child_Obj);
    }
}
