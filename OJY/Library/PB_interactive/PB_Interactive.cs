using UnityEngine;
using UnityEngine.UI;
using Suncheon.WebData;
using Suncheon.UI;
using System.Collections;

namespace Suncheon
{
    public class PB_Interactive : MonoBehaviour, Clickable
    {
        [SerializeField] private Image image_panel;
        [SerializeField] private string URL;
        [SerializeField] GameObject UI_YN;
        [SerializeField] Button btn_y;
        [SerializeField] Button btn_n;

        private void Awake()
        {
            btn_y.onClick.AddListener(() => Btn_Y());
            btn_n.onClick.AddListener(() => Btn_N());
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrExhibitionLoad}", (jsonData) =>
            {
                UTILS.Log($"PB_Interactive : {jsonData}");
                Response_ExhibitionLoad response_ExhibitionLoad = null;
                try
                {
                    response_ExhibitionLoad = JsonUtility.FromJson<Response_ExhibitionLoad>(jsonData);
                }
                catch
                {
                    response_ExhibitionLoad = null;
                }
                if (response_ExhibitionLoad == null) return;

                try
                {
                    SetURL(response_ExhibitionLoad.exhibition_url);
                    string fileUrl = "";
                    if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
                    {
                        fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
                    }
                    else
                    {
                        fileUrl = GameManager.Instance.defaultData.fileUrl;
                    }
                    StartCoroutine(UTILS.Requset_HttpGetTexture($"{fileUrl}{response_ExhibitionLoad.exhibition_img}", (textureData) =>
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

        void SetURL(string url)
        {
            string modifiedUrl = url.StartsWith("http://") || url.StartsWith("https://") ? url : "http://" + url;
            URL = modifiedUrl;
        }

        private void SetImage(Texture2D texture)
        {
            if(texture == null) return;
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

        public void OnClick()
        {
            if (!waiting)
            {
                UTILS.Log("가벽 상호작용");
                UIInteractionManager.Instance.OpenPopUp(UI_YN);
                //Application.OpenURL(URL);
            }
        }

        void Btn_Y()
        {
            Application.OpenURL(URL);
            UIInteractionManager.Instance.ClosePopUp(UI_YN);
            Click_Delay();
        }
        void Btn_N()
        {
            UIInteractionManager.Instance.ClosePopUp(UI_YN);
            Click_Delay();
        }

        [SerializeField] string obj_Name;
        public string Return_ObjName()
        {
            return obj_Name;
        }

        #region 클릭 딜레이 설정
        bool waiting = false;
        void Click_Delay()
        {
            StartCoroutine(Delay_Co());
        }

        IEnumerator Delay_Co()
        {
            waiting = true;
            yield return new WaitForSeconds(.5f);
            waiting = false;
        }
        #endregion
    }
}