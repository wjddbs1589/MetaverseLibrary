using UnityEngine;
using Suncheon;

public class TurnTableInteractive : MonoBehaviour, Clickable
{
    Outline outline;
    [SerializeField] string obj_Name;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }


    public void OnClick()
    {
        Application.OpenURL("https://youtube.com/");
        UTILS.Log("턴테이블 사용");
    }

    public string Return_ObjName()
    {
        return obj_Name;
    }
}
