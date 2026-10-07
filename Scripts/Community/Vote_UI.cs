using Suncheon.WebData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.UI
{
    /// <summary>
    /// 투표 UI. 투표 주제와 최대 4개 항목을 표시하고, 항목을 고른 뒤 확인 팝업을 거쳐 서버에 투표를 보낸다.
    /// </summary>
    public class Vote_UI : MonoBehaviour
    {
        // 서버 응답 성공 코드
        private const string CODE_SUCCESS = "000";

        [SerializeField] private Button btn_exit;
        [SerializeField] private Button btn_vote;

        [Tooltip("투표 항목 표시 오브젝트 (항목 순서대로)")]
        [SerializeField] private VoteObject[] voteObjects;

        [Tooltip("투표 주제 텍스트")]
        public TextMeshProUGUI vote_text;

        // 항목 선택 버튼
        private VoteSelectBtn[] _selectButtons;

        // 서버의 투표 번호와 선택한 항목 인덱스 (0부터)
        private int _voteSeq = -1;
        private int _selectedIndex = -1;

        private UI_YesNoPopUp _confirmPopup;

        /// <summary>버튼 이벤트를 연결하고 항목 선택 버튼을 가져온다.</summary>
        private void Awake()
        {
            btn_exit.onClick.AddListener(() => UIInteractionManager.Instance.ClosePopUp(gameObject));
            btn_vote.onClick.AddListener(Btn_Vote);

            _selectButtons = GetComponentsInChildren<VoteSelectBtn>();
        }

        /// <summary>서버에서 받은 투표 주제와 항목을 표시하고 선택 상태를 초기화한다.</summary>
        public void Set_VoteInfo(Response_VoteSelect result)
        {
            foreach (VoteSelectBtn button in _selectButtons)
                button.Toggle_Off();

            _voteSeq = int.Parse(result.voteSeq);
            vote_text.text = result.title;

            string[] categories = { result.category1, result.category2, result.category3, result.category4 };
            string[] categoryFiles = { result.categoryFile1, result.categoryFile2, result.categoryFile3, result.categoryFile4 };

            for (int i = 0; i < voteObjects.Length; i++)
                voteObjects[i].Init(i, categories[i], categoryFiles[i]);
        }

        /// <summary>투표 버튼. 선택한 항목이 있으면 확인 팝업을 띄우고, 없으면 먼저 선택하라고 안내한다.</summary>
        private void Btn_Vote()
        {
            _selectedIndex = System.Array.FindIndex(_selectButtons, button => button.isSelect);

            if (_selectedIndex < 0)
            {
                UIInteractionManager.Instance.ShowSystemPopUp("항목을 먼저 선택하세요.");
                return;
            }

            if (_confirmPopup == null)
                _confirmPopup = UIInteractionManager.Instance.ui_YesNoPopUp.GetComponent<UI_YesNoPopUp>();

            _confirmPopup.SetYesBtn(new YesBtnDelegate(OnConfirmYes));
            _confirmPopup.SetNoBtn(new NoBtnDelegate(OnConfirmNo));
            _confirmPopup.ShowPopUp($"{_selectedIndex + 1}번 항목에 투표 하시겠습니까?");
        }

        /// <summary>확인 팝업에서 예를 누르면 투표를 보낸다.</summary>
        private void OnConfirmYes()
        {
            Upload();
            _confirmPopup.HidePopUp();
        }

        /// <summary>확인 팝업에서 아니오를 누르면 팝업만 닫는다.</summary>
        private void OnConfirmNo()
        {
            _confirmPopup.HidePopUp();
        }

        /// <summary>선택한 항목(1부터 시작하는 번호)으로 서버에 투표를 보내고, 성공하면 안내 후 UI를 닫는다.</summary>
        private void Upload()
        {
            string url = $"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteSelect}";
            Request_UesrVote request = new Request_UesrVote(_voteSeq, _selectedIndex + 1);

            StartCoroutine(UTILS.Requset_HttpPostData(url, request, jsonData =>
            {
                Response_ReturnMsg response;
                try { response = JsonUtility.FromJson<Response_ReturnMsg>(jsonData); }
                catch { return; }

                if (response == null) return;
                UTILS.Log($"[투표] {response.rtnCode} {response.rtnMsg}");

                if (response.rtnCode != CODE_SUCCESS)
                {
                    UIInteractionManager.Instance.ShowSystemPopUp("투표에 실패했습니다. 잠시 후 다시 시도해주세요.");
                    return;
                }

                UIInteractionManager.Instance.ShowSystemPopUp($"{_selectedIndex + 1}번 항목에 투표가 되었습니다.");
                UIInteractionManager.Instance.ClosePopUp(gameObject);
            }));
        }
    }
}
