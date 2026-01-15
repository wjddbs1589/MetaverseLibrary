using Suncheon;
using Suncheon.UI;
using UnityEngine;

public class PlayerObjInteraction : MonoBehaviour
{
    [SerializeField]Camera PlayerCamera;
    Outline objOutline;    
    GameObject hitObj;  //ray에 닿은 오브젝트
    GameObject clickObj;//마지막으로 선택된 오브젝트

    public bool UsingUI = false;

    public bool ChooseUI = false;

    Clickable clickableObj;
    [SerializeField] ObjNameText nameText;
    UIInteractionManager interactionManager;
    private void Awake()
    {
        interactionManager = FindAnyObjectByType<UIInteractionManager>();
    }

    void Update()
    {
        if (!interactionManager.Play_Game && NetworkManager.Instance.Go_Player == this.gameObject)
        {
            Distance_Check();
        }
    }

    /// <summary>
    /// 상호작용 오브젝트 탐색
    /// </summary>
    void Distance_Check()
    {
        if (!UsingUI && !ChooseUI)
        {
            Ray mouseRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (!Too_far_Check(mouseHit.collider.gameObject.transform))
                {                    
                    hitObj = mouseHit.collider.gameObject;
                }
                else
                {
                    Clear();
                    return;
                }
            }
            else
            {
                Clear();
                return;
            }

            Mouse_Action();
        }
        else
        {
            Clear();
        }
    }

    /// <summary>
    /// 물체가 너무 멀리있는지 확인
    /// </summary>
    /// <param name="target">ray에 맞은놈</param>
    /// <returns>물체가 멀리있을 때 true</returns>
    bool Too_far_Check(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= 15f)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 대상 감지 및 클릭 상호작용
    /// </summary>
    void Mouse_Action()
    {
        if (hitObj != null)
        {
            Clickable_Check(hitObj);

            if (Input.GetMouseButtonDown(0))
            {
                clickObj = hitObj;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Click_Obj();
            }
        }
    }
    
    /// <summary>
    /// 상호작용 가능한 오브젝트의 아웃라인 생성
    /// </summary>
    void Clickable_Check(GameObject hit)
    {        
        clickableObj = hit.GetComponent<Clickable>();
        if (clickableObj != null)
        {            
            //사용중인 아웃라인이 없을 때
            if(objOutline == null)
            {
                objOutline = hit.GetComponent<Outline>();
            }
            //사용중인 아웃라인이 이미 있을 때
            else
            {
                Outline newOutline = hit.GetComponent<Outline>();

                // 아웃라인 대상이 같지 않을 때
                if (objOutline != newOutline)
                {
                    objOutline.enabled = false; //기존 아웃라인 끄기
                    objOutline = newOutline;    //아웃라인 대상 변경
                }                
            }

            objOutline.enabled = true;
            Set_ObjName();
        }
        else
        {
            if (objOutline != null)
            {
                objOutline.enabled = false;
            }
            Set_ObjName();
        }
    }

    /// <summary>
    /// 상호작용 가능한 오브젝트이면 기능 실행
    /// </summary>
    void Click_Obj()
    {
        Ray clickRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit clickHit;

        if (Physics.Raycast(clickRay, out clickHit))
        {
            Clickable clickable = clickHit.collider.gameObject.GetComponent<Clickable>();
            if (clickable != null && (clickHit.collider.gameObject == clickObj))
            {
                clickable.OnClick();
                Clear();
            }
        }
    }


    /// <summary>
    /// 기존에 적용된 사항들 초기화
    /// </summary>
    public void Clear()
    {
        if (objOutline != null)
        {
            objOutline.enabled = false;
        }
        hitObj = null;
        clickableObj = null;
        Set_ObjName();
    }

    /// <summary>
    /// 상호작용 오브젝트의 이름표시 
    /// </summary>
    /// <param name="isOn"></param>
    void Set_ObjName()
    {
        if (clickableObj == null)
        {
            nameText.uiText.text = "";
            return;
        }

        string name = clickableObj.Return_ObjName();

        if (name != null)
        {
            nameText.uiText.text = name;
        }
        else
        {
            nameText.uiText.text = "";
        }
    }
}

