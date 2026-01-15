using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using UnityEngine;

namespace Suncheon
{
    public class Get_HanjeomArt : MonoBehaviour
    {
        //Response_SS_Museum

        private SS_HanjeomArt[] hanjeomArt = null;

        void Start()
        {
            hanjeomArt = GetComponentsInChildren<SS_HanjeomArt>();
            SetEasel();
        }

        private void SetEasel()
        {
            // 잠깐 주석 처리
            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.ssMuseumLoad}", (jsonData) =>
            {
                UTILS.Log($"SetEasel : {jsonData}");
                //Debug.LogWarning(jsonData);
                JArray jArray = null;
                try
                {
                    jArray = JArray.Parse(jsonData);
                }
                catch
                {
                    jArray = null;
                }

                Response_SS_Museum response_SSMuseum = null;
                try
                {
                    response_SSMuseum = JsonUtility.FromJson<Response_SS_Museum>(jArray[0].ToString());
                }
                catch
                {
                    response_SSMuseum = null;
                }

                if (response_SSMuseum == null) return;

                for (int i = 0; i < hanjeomArt.Length; i++)
                {                   
                    hanjeomArt[i].Init(response_SSMuseum);
                }
            }));
        }
    }
}