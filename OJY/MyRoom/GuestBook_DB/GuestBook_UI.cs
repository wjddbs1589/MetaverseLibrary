using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon
{
    public class GuestBook_UI : MonoBehaviour
    {
        [SerializeField] Button btn_Exit;

        FurnitureMove furnitureMove;

        private void OnDisable()
        {
            furnitureMove.UsingUI = false;
        }

        private void Awake()
        {
            btn_Exit.onClick.AddListener(() => Btn_Exit());
            furnitureMove = FindAnyObjectByType<FurnitureMove>();
        }


        void Btn_Exit()
        {
            btn_Exit.GetComponent<ButtonControl>().Btnoff();
            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }
    }
}