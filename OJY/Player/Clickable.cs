// 클릭 가능한 객체에 적용할 인터페이스
using UnityEngine;

public interface Clickable
{
    /// <summary>
    /// 마우스로 상호작용 했을 떄 실행 될 함수
    /// </summary>
    void OnClick();

    /// <summary>
    /// 클릭가능한 오브젝트의 이름을 반환
    /// </summary>
    /// <returns></returns>
    string Return_ObjName();
    
}