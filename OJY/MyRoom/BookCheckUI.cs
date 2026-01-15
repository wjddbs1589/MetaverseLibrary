using Suncheon.UI;
using UnityEngine;
using UnityEngine.UI;

public class BookCheckUI : MonoBehaviour
{
    [SerializeField] Button btn_Exit;
    FurnitureMove furnitureMove;
    private void Awake()
    {
        btn_Exit.onClick.AddListener(() => Btn_Exit());
        furnitureMove = FindAnyObjectByType<FurnitureMove>();
    }

    void Btn_Exit()
    {
        furnitureMove.UsingUI = false;
        InteractionManager.Inst.Ray_On();        
        UIInteractionManager.Instance.ClosePopUp(gameObject);
    }
}
