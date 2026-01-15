using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWebView : MonoBehaviour
{
    [Header("OutLine_Webview")]
    [SerializeField] Outline_Webview ow; 
    private void OnDisable()
    {
        ow.Close_Web();
    }
}
