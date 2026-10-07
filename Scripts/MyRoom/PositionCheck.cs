using UnityEngine;

/// <summary>
/// 가구 프리팹에 붙는 겹침 판정 트리거.
/// 다른 물체와 겹쳐 있는 동안 Crash가 true가 되며, FurnitureMove가 이 값으로 배치 가능 여부를 판단한다.
/// </summary>
public class PositionCheck : MonoBehaviour
{
    /// <summary>다른 물체와 겹쳐 있으면 true</summary>
    [HideInInspector] public bool Crash = false;

    /// <summary>다른 물체와 겹치기 시작하면 배치 불가로 표시한다.</summary>
    private void OnTriggerEnter(Collider other)
    {
        Crash = true;
    }

    /// <summary>
    /// 겹친 상태가 유지되는 동안 배치 불가를 유지한다.
    /// 여러 물체와 겹쳐 있다가 그중 하나에서만 벗어나 Exit가 호출된 경우에도 다시 true로 바로잡는다.
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        Crash = true;
    }

    /// <summary>물체에서 벗어나면 배치 가능으로 표시한다.</summary>
    private void OnTriggerExit(Collider other)
    {
        Crash = false;
    }
}
