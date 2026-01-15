using UnityEngine.UI;
using UnityEngine;
using Suncheon.UI;
using Suncheon.WebData;

namespace Suncheon
{
    public class SS_HanjeomArt : MonoBehaviour, Clickable
    {
        [SerializeField] string loaction = string.Empty;
        [SerializeField] string ObjName = "크게보기";

        Image Hanjeom_Image; //한점 미술관에 들어갈 이미지
        Material image_Material;

        [SerializeField] GameObject Image_Popup; //이미지 팝업    
        Response_SS_Museum SS_Museum;
        Easel_UI easel_UI; //이미지 넣는 함수 불러오기 용

        void Awake()
        {
            easel_UI = Image_Popup.GetComponent<Easel_UI>();
            Hanjeom_Image = GetComponentInChildren<Image>();
        }

        public void OnClick()
        {
            UIInteractionManager.Instance.OpenPopUp(Image_Popup);
            easel_UI.Set_Sprite(Hanjeom_Image.sprite);
        }

        public string Return_ObjName()
        {
            return ObjName;
        }

        public void Init(Response_SS_Museum _response_ChildrenEaselLoad)
        {
            SS_Museum = _response_ChildrenEaselLoad;

            try
            {
                string imgName = string.Empty;
                string fileUrl = "";
                if (string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
                {
                    fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
                }
                else
                {
                    fileUrl = GameManager.Instance.defaultData.fileUrl;
                }

                if (loaction == "n") { imgName = SS_Museum.n; }
                else if (loaction == "w") { imgName = SS_Museum.w; }
                else if (loaction == "e") { imgName = SS_Museum.e; }
                else if (loaction == "s") { imgName = SS_Museum.s; }

                StartCoroutine(UTILS.Requset_HttpGetTexture($"{fileUrl}{imgName}", (textureData) =>
                {
                    UTILS.Log($"SetBanner : {textureData}");
                    if (textureData == null) return;
                    
                    Hanjeom_Image.sprite = UTILS.ConvertToSprite(textureData);
                }));
            }
            catch
            {

            }
        }
    }
}


