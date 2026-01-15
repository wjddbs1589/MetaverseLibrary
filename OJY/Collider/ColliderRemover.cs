using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 컴포넌트를 가진 게임오브젝트를 포함한 모든 하위 오브젝트의 콜라이더를 제거
public class ColliderRemover : MonoBehaviour
{
    Collider[] cols;
        
    private void Awake()
    {
        cols = GetComponentsInChildren<Collider>();

        foreach (Collider col in cols) 
        {
            Destroy(col);
        }
    }
}
