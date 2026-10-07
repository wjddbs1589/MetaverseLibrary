using UnityEngine;
using UnityEngine.UI;

/// <summary>추천글 작성 취소 확인 팝업. 확인을 누르면 작성 내용을 버리고 목록으로 돌아간다.</summary>
public class RecmPopUp_UI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button btn_Yes;
    [SerializeField] private Button btn_No;

    [Tooltip("작성 중인 본문 화면")]
    [SerializeField] private Recommend_UI recommend;

    /// <summary>버튼 이벤트를 연결한다.</summary>
    private void Awake()
    {
        btn_Yes.onClick.AddListener(() =>
        {
            recommend.Cancel();
            gameObject.SetActive(false);
        });
        btn_No.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
