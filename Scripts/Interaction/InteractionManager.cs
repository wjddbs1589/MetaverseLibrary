using Suncheon;
using System.Collections;
using UnityEngine;

/// <summary>
/// 팝업 UI를 여닫을 때 내 플레이어의 오브젝트 상호작용(레이캐스트)을 끄고 켜는 싱글톤.
/// 씬 입장 직후처럼 내 플레이어가 아직 생성되지 않았으면, 생성될 때까지 기다렸다가 적용한다.
///
/// 내 플레이어 기준:
///   멀티플레이 씬에는 다른 플레이어의 PlayerObjInteraction도 있으므로,
///   NetworkManager.Go_Player(내 플레이어)의 컴포넌트에만 적용한다.
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Inst { get; private set; }

    // 내 플레이어의 상호작용 컴포넌트
    private PlayerObjInteraction _localInteraction;

    /// <summary>싱글톤을 등록한다. 다른 스크립트의 Start에서 호출해도 사용할 수 있도록 Awake에서 처리한다.</summary>
    private void Awake()
    {
        if (Inst == null) Inst = this;
    }

    /// <summary>플레이어의 오브젝트 상호작용을 끈다. (팝업 UI를 열 때)</summary>
    public void Ray_Off() => SetRayEnabled(false);

    /// <summary>플레이어의 오브젝트 상호작용을 켠다. (팝업 UI를 닫을 때)</summary>
    public void Ray_On() => SetRayEnabled(true);

    /// <summary>내 플레이어가 준비되어 있으면 바로 적용하고, 아니면 준비될 때까지 기다린다.</summary>
    private void SetRayEnabled(bool enable)
    {
        if (_localInteraction != null)
        {
            _localInteraction.UsingUI = !enable;
            return;
        }

        StartCoroutine(ApplyWhenPlayerReady(enable));
    }

    /// <summary>내 플레이어가 생성될 때까지 기다린 뒤 상호작용 여부를 적용한다.</summary>
    private IEnumerator ApplyWhenPlayerReady(bool enable)
    {
        while (NetworkManager.Instance.Go_Player == null)
            yield return null;

        _localInteraction = NetworkManager.Instance.Go_Player.GetComponent<PlayerObjInteraction>();
        if (_localInteraction != null)
            _localInteraction.UsingUI = !enable;
    }
}
