using Suncheon.Player;
using Suncheon.UI;
using UnityEngine;

namespace Suncheon
{
    /// <summary>자료실의 추천도서 PC 오브젝트. 클릭하면 추천도서 게시판을 연다. 게스트 계정은 사용할 수 없다.</summary>
    public class ArchivesPC : MonoBehaviour, IClickable
    {
        [Tooltip("추천도서 게시판 UI")]
        [SerializeField] private GameObject PC;

        [Tooltip("마우스를 올렸을 때 표시할 이름")]
        [SerializeField] private string obj_Name;

        /// <summary>게스트가 아니면 추천도서 게시판을 연다.</summary>
        public void OnClick()
        {
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().IsGuest)
            {
                UIInteractionManager.Instance.ShowSystemPopUp("Guest는 추천도서 기능을 사용할 수 없습니다.");
                return;
            }

            UIInteractionManager.Instance.OpenPopUp(PC);
        }

        /// <summary>마우스를 올렸을 때 표시할 이름을 반환한다.</summary>
        public string Return_ObjName() => obj_Name;
    }
}
