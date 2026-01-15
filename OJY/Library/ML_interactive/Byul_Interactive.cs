using UnityEngine;
using Suncheon;

public class Byul_Interactive : MonoBehaviour, Clickable
{
    [HideInInspector]public bool PlayerIN = false;
    [SerializeField] string obj_Name = null;
    public void OnClick()
    {
        if (PlayerIN)
        {
            Application.OpenURL("https://astro.kasi.re.kr/learning/pageView/5272");
            UTILS.Log("별자리전설 페이지 연결");
        }
    }

    public string Return_ObjName()
    {
        if(PlayerIN)
        {
            return obj_Name;
        }
        else
        {
            return "";
        }
    }
}
