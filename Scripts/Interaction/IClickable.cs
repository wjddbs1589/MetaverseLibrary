/// <summary>
/// 마우스·터치로 상호작용할 수 있는 오브젝트의 공통 인터페이스.
/// PlayerObjInteraction(광장·도서관)과 FurnitureMove(개인서재)가 레이에 맞은 오브젝트에서 이 인터페이스를 찾아 처리한다.
///
/// 새 상호작용 오브젝트는 이 인터페이스만 구현하면 아웃라인 강조, 이름 표시, 클릭 처리가 자동으로 연동된다.
/// </summary>
public interface IClickable
{
    /// <summary>오브젝트를 클릭했을 때 실행할 기능</summary>
    void OnClick();

    /// <summary>마우스를 올렸을 때 표시할 오브젝트 이름. 빈 문자열이면 이름을 표시하지 않는다.</summary>
    string Return_ObjName();
}
