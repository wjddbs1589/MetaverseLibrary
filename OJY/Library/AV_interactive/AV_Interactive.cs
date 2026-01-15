using Suncheon;
using Suncheon.Admin;
using Suncheon.WebData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AV_Interactive : MonoBehaviour
{
    [SerializeField] Image image_panel;

    Outline_Webview ow;
    private void Start()
    {
        ow = GetComponent<Outline_Webview>();
        SetScreen();        
    }

    public void SetScreen()
    {
        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrEventList}", (jsonData) =>
        {
            Response_EventList response_EventList = new Response_EventList(jsonData);

            if (response_EventList == null)
            {
                UTILS.Log("Null");
                return;
            }
            else
            {
                if (response_EventList.response_EventListDatas.Count != 0)
                {
                    UTILS.Log("이벤트");
                    SetEventObject(response_EventList);
                }
                else //if (response_EventList.response_EventListDatas.Count == 0) 
                {
                    UTILS.Log("URL 설정");
                    ScreenLoad();
                }
            }
        }));
    }

    /// <summary>
    /// 스크린 URL정보를 가져오는 함수
    /// </summary>
    void ScreenLoad()
    {
        string location = string.Empty;
        if (NetworkManager.Instance.LibPos == LibName.삼산도서관) { location = $"{EventLoacation.삼산도서관시청각실}"; }
        else if (NetworkManager.Instance.LibPos == LibName.그림책도서관) { location = $"{EventLoacation.그림책도서관시청각실}"; }
        else if (NetworkManager.Instance.LibPos == LibName.기적의도서관) { location = $"{EventLoacation.기적의도서관시청각실}"; }
        else if (NetworkManager.Instance.LibPos == LibName.조례호수도서관) { location = $"{EventLoacation.조례호수도서관시청각실}"; }
        else if (NetworkManager.Instance.LibPos == LibName.연향도서관) { location = $"{EventLoacation.연향도서관시청각실}"; }
        else if (NetworkManager.Instance.LibPos == LibName.신대도서관) { location = $"{EventLoacation.신대도서관시청각실}"; }

        StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrScreenLoad}", $"screen_location={location}", (jsonData) =>
            {
                Response_ScreenLoad response_ScreenLoad = null;

                UTILS.Log(jsonData);
                try
                {
                    response_ScreenLoad = JsonUtility.FromJson<Response_ScreenLoad>(jsonData);
                }
                catch
                {
                    response_ScreenLoad = null;
                }

                if (response_ScreenLoad == null) return;

                try
                {
                    SetURL(response_ScreenLoad.screen_url);
                    string fileUrl = "";
                    if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
                    {
                        fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
                    }
                    else
                    {
                        fileUrl = GameManager.Instance.defaultData.fileUrl;
                    }

                    StartCoroutine(UTILS.Requset_HttpGetTexture($"{fileUrl}{response_ScreenLoad.screen_img}", (textureData) =>
                    {
                        UTILS.Log($"SetBanner : {textureData}");
                        SetImage(textureData);
                    }));
                }
                catch
                {
                    return;
                }

            }));
    }

    private DateTime NowDateTime;

    private void SetEventObject(Response_EventList result)
    {
        GetNowDate();
        foreach (var item in result.response_EventListDatas)
        {
            DateTime startTime = DateTime.Parse(item.eventStartTime);
            DateTime endTime = startTime.AddMinutes(item.eventTime);

            if (startTime <= NowDateTime && NowDateTime >= endTime && NetworkManager.Instance.LibPos.ToString() == item.eventLocation)
            {
                SetURL(item.eventUrl);
                return;
            }
        }

    }

    void SetURL(string url)
    {
        string modifiedUrl = url.StartsWith("http://") || url.StartsWith("https://") ? url : "http://" + url;
        ow.url = modifiedUrl;
    }

    private void SetImage(Texture2D texture)
    {
        if (texture == null) return;
        image_panel.sprite = UTILS.ConvertToSprite(texture);
    }

    private void SetImage(byte[] bytes)
    {
        Sprite sprite = UTILS.ByteArrayToSprite(bytes);
        image_panel.sprite = sprite;
    }

    private void SetImage(string base64Data)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64Data);
        SetImage(imageBytes);
    }

    private void GetNowDate()
    {
#if UNITY_EDITOR
        NowDateTime = DateTime.Now;
#elif !UNITY_EDITOR && UNITY_WEBGL
        Application.ExternalCall("GetNowTime");
#endif
    }

    private void SetNowDate(string nowDateTime)
    {
        NowDateTime = DateTime.Parse(nowDateTime);
        UTILS.Log($"{nowDateTime} : {NowDateTime}");
    }
}
