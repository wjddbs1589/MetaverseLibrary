using Suncheon;
using Suncheon.Player;
using Suncheon.UI;

/// <summary>
/// 동아리방의 회의 자료 오브젝트.
/// 동아리장이 회의를 시작했을 때만 열 수 있고, 서버에서 받은 회의 자료 주소를 연다.
/// 표시 이름 앞에는 현재 회의 이름을 붙인다.
/// </summary>
public class ClubMeetingWebView : WebViewInteractable
{
    private PlayerManager LocalPlayer => NetworkManager.Instance.Go_Player.GetComponent<PlayerManager>();

    /// <summary>회의가 시작되지 않았으면 안내 메시지를 띄우고 열지 않는다.</summary>
    protected override bool CanOpen()
    {
        if (LocalPlayer.isClub_Meeting) return true;

        UIInteractionManager.Instance.ShowSystemPopUp("동아리장이 회의를 시작하지 않았습니다.");
        return false;
    }

    /// <summary>현재 회의의 자료 주소를 반환한다.</summary>
    protected override string GetUrl() => LocalPlayer.club_FileURL;

    /// <summary>회의 이름과 오브젝트 이름을 함께 표시한다.</summary>
    public override string Return_ObjName() => $"{LocalPlayer.club_MeetingName}\n{base.Return_ObjName()}";
}
