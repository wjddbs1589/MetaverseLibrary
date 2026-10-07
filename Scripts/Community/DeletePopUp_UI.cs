using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 추천도서 삭제 확인 팝업의 버튼 참조.
/// 팝업은 모든 글 항목이 함께 쓰며, 삭제를 누른 글(BookRecommend)이 그때그때 버튼에 리스너를 연결한다.
/// </summary>
public class DeletePopUp_UI : MonoBehaviour
{
    [Header("버튼")]
    public Button btn_Yes;
    public Button btn_No;
}
