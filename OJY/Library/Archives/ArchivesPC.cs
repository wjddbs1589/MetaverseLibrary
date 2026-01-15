using Suncheon.Player;
using Suncheon.UI;
using UnityEngine;

namespace Suncheon
{
    public class ArchivesPC : MonoBehaviour, Clickable
    {
        [Header("VideoUI")]
        [SerializeField] GameObject PC;

        public void OnClick()
        {
            if (NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>().IsGuest)
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"Guest는 추천도서 기능을 사용할 수 없습니다.");
                return;
            }

            UIInteractionManager.Instance.OpenPopUp(PC);
        }

        [SerializeField] string obj_Name;
        public string Return_ObjName()
        {
            return obj_Name;
        }
    }
}