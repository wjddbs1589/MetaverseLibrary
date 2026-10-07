using Suncheon;
using Suncheon.UI;
using UnityEngine;

/// <summary>
/// 광장·도서관의 마스코트 NPC.
/// 플레이어가 가까이 오면 말풍선을 띄우고 플레이어 쪽으로 몸을 돌린다.
/// 클릭하면 NPC 대화창을 열거나, 웹 연결 NPC면 지정한 페이지를 연다(WebViewInteractable 기능 사용).
/// </summary>
public class Mascot : WebViewInteractable
{
    private static readonly int IdleHash = Animator.StringToHash("Idle");

    // 말풍선을 표시하고 플레이어를 바라보는 거리 (m)
    private const float TALK_DISTANCE = 15.0f;

    [Header("NPC")]
    [Tooltip("가까이 오면 표시할 말풍선")]
    [SerializeField] private GameObject ui_TalkBox;

    [SerializeField] private Animator anim;

    [Tooltip("체크하면 클릭 시 NPC 대화창을, 해제하면 웹 페이지를 연다")]
    [SerializeField] private bool isNpcChat;

    [Tooltip("NPC 대화창")]
    [SerializeField] private GameObject ui_NpcChat;

    // 내 플레이어
    private GameObject _player;

    /// <summary>기본 대기 애니메이션을 재생한다.</summary>
    private void Start()
    {
        anim.SetTrigger(IdleHash);
    }

    /// <summary>내 플레이어와의 거리에 따라 말풍선을 표시하고 플레이어를 바라본다.</summary>
    private void Update()
    {
        if (_player == null)
        {
            if (NetworkManager.Instance == null) return;
            _player = NetworkManager.Instance.Go_Player;
            if (_player == null) return;
        }

        Vector3 toPlayer = _player.transform.position - transform.position;
        bool isNear = toPlayer.magnitude <= TALK_DISTANCE;

        // 상태가 바뀔 때만 말풍선 표시를 바꾼다
        if (ui_TalkBox.activeSelf != isNear)
            ui_TalkBox.SetActive(isNear);

        if (isNear)
        {
            toPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(toPlayer);
        }
    }

    /// <summary>대화 NPC면 대화창을 열고, 웹 연결 NPC면 페이지를 연다.</summary>
    public override void OnClick()
    {
        if (isNpcChat)
        {
            UIInteractionManager.Instance.OpenPopUp(ui_NpcChat);
            return;
        }

        base.OnClick();
    }

    /// <summary>NPC는 마우스를 올려도 이름을 표시하지 않는다.</summary>
    public override string Return_ObjName() => "";
}
