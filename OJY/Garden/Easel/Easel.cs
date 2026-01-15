using Suncheon.UI;
using Suncheon.WebData;
using UnityEngine;

namespace Suncheon
{
    public class Easel : MonoBehaviour, Clickable
    {
        [SerializeField] GameObject UI_Easel;
        Easel_UI easel_UI;

        Response_ChildrenEaselLoad response_ChildrenEaselLoad;

        [SerializeField] private Material mat_Picture;

        private void Awake()
        {
            easel_UI = UI_Easel.GetComponent<Easel_UI>();
            mat_Picture = transform.GetChild(1).GetComponent<MeshRenderer>().materials[1];
        }

        public void Init(Response_ChildrenEaselLoad _response_ChildrenEaselLoad)
        {
            response_ChildrenEaselLoad = _response_ChildrenEaselLoad;

            try
            {
                string fileUrl = "";
                if (GameManager.Instance == null || string.IsNullOrEmpty(GameManager.Instance.defaultData.fileUrl.Trim()))
                {
                    fileUrl = $"https://metalibrary.suncheon.go.kr/upload/";
                }
                else
                {
                    fileUrl = GameManager.Instance.defaultData.fileUrl;
                }

                StartCoroutine(UTILS.Requset_HttpGetTexture($"{fileUrl}{response_ChildrenEaselLoad.fileName}", (textureData) =>
                {
                    UTILS.Log($"SetBanner : {textureData}");
                    if (textureData == null) return;

                    mat_Picture.mainTexture = textureData;
                }));
            }
            catch
            {

            }
        }

        public void OnClick()
        {
            UIInteractionManager.Instance.OpenPopUp(UI_Easel);
            easel_UI.Set_Material(mat_Picture);
        }

        public string Return_ObjName()
        {
            return "크게보기";
        }
    }
}