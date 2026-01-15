using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWebView_Club : MonoBehaviour
{
    [HideInInspector] public Outline_Webview_Club OW;
    private void OnDisable()
    {
        OW.Close_Web();
    }
}
