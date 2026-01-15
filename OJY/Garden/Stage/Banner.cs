using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//배너오브젝트를 클릭했을 때 지정된 URL 오픈
public class Banner : MonoBehaviour, Clickable
{
    public string BannerLink;
    [SerializeField] string obj_Name;
    
    public void OnClick()
    {
        Application.OpenURL(BannerLink);
    }

    public string Return_ObjName()
    {
        return obj_Name;
    }
}
