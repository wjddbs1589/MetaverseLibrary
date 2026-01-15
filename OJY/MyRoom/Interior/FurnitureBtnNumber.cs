using UnityEngine;
using UnityEngine.UI;

public class FurnitureBtnNumber : MonoBehaviour
{
    [HideInInspector] int BtnIndex;

    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => Btn_Click());
        BtnIndex = Find_Index();
    }

    void Btn_Click()
    {
        FurnitureManager.Instance.Spawn_Furniture(BtnIndex);
    }

    int Find_Index()
    {
        Transform parent = transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.transform.GetChild(i) == transform)
            {
                return i;
            }
        }

        return -1; 
    }
}
