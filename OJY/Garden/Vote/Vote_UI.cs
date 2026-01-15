using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Suncheon.UI
{
    public class Vote_UI : MonoBehaviour
    {
        [SerializeField] Button btn_exit;
        [SerializeField] Button btn_vote;

        [SerializeField] private VoteObject[] voteObjects;
        VoteSelectBtn[] voteSelectBtns;
        public TextMeshProUGUI vote_text;

        private int voteSeq = -1;
        private int selectNum = -1;

        YesBtnDelegate yesBtnDelegate;
        NoBtnDelegate noBtnDelegate;

        private void Awake()
        {
            btn_exit.onClick.AddListener(() => Btn_Exit());
            btn_vote.onClick.AddListener(() => Btn_Vote());

            yesBtnDelegate = new YesBtnDelegate(YesBtnClick);
            noBtnDelegate = new NoBtnDelegate(NoBtnClick);

            voteSelectBtns = GetComponentsInChildren<VoteSelectBtn>();
        }

        private void OnEnable()
        {
            //Get_VoteInfo();
        }

        /// <summary>
        /// 투표 내용과 항목 설정
        /// </summary>
        public void Get_VoteInfo()
        {
           

            


        }

        public void Set_VoteInfo(Response_VoteSelect result)
        {
            //모든 버튼 상태 초기화
            for (int i = 0; i < voteSelectBtns.Length; i++)
            {
                voteSelectBtns[i].Toggle_Off();
            }

            voteSeq = int.Parse(result.voteSeq);
            vote_text.text = result.title;

            List<string> categorys = new List<string>();
            categorys.Add(result.category1);
            categorys.Add(result.category2);
            categorys.Add(result.category3);
            categorys.Add(result.category4);

            List<string> categoryFiles = new List<string>();
            categoryFiles.Add(result.categoryFile1);
            categoryFiles.Add(result.categoryFile2);
            categoryFiles.Add(result.categoryFile3);
            categoryFiles.Add(result.categoryFile4);

            for (int i = 0; i < voteObjects.Length; i++)
            {
                voteObjects[i].Init(i, categorys[i], categoryFiles[i]);
            }
        }

        void Btn_Exit()
        {
            UIInteractionManager.Instance.ClosePopUp(gameObject);
        }

        private void Upload()
        {
            Request_UesrVote request_UesrVote = new Request_UesrVote(voteSeq, selectNum + 1);
            StartCoroutine(UTILS.Requset_HttpPostData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteSelect}", request_UesrVote, 
            (jsonData) =>
            {
                Response_ReturnMsg response_ReturnMsg = null;
                try
                {
                    response_ReturnMsg = JsonUtility.FromJson<Response_ReturnMsg>(jsonData);
                }
                catch
                {
                    response_ReturnMsg = null;
                }
                if (response_ReturnMsg == null) { return; }

                UIInteractionManager.Instance.ShowSystemPopUp($"{selectNum + 1}번 항목에 투표가 되었습니다.");
                UIInteractionManager.Instance.ClosePopUp(gameObject);

                UTILS.Log(response_ReturnMsg.rtnMsg);
                UTILS.Log(response_ReturnMsg.rtnCode);
            }));
        }

        void Btn_Vote()
        {
            bool slectOn = false;

            //선택한 버튼이 있는지 확인
            foreach (VoteSelectBtn btn in voteSelectBtns)
            {
                if (btn.isSelect)
                {
                    slectOn = btn.isSelect;
                    break;
                }                
            }

            if (slectOn)
            {
                UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetYesBtn(yesBtnDelegate);
                UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().SetNoBtn(noBtnDelegate);

                for (int i = 0; i < voteSelectBtns.Length; i++)
                {
                    if (voteSelectBtns[i].isSelect) { selectNum = i; }
                }

                string strInfo = $"{selectNum + 1}번 항목에 투표 하시겠습니까?";
                UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().ShowPopUp(strInfo);
            }
            else
            {
                UIInteractionManager.Instance.ShowSystemPopUp($"항목을 먼저 선택하세요.");
            }
            
        }

        private void YesBtnClick()
        {
            Upload();
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();
        }

        private void NoBtnClick()
        {
            UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>().HidePopUp();
        }
    }
}
