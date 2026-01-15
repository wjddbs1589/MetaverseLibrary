using System.Collections;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    static InteractionManager instance = null;
    public static InteractionManager Inst => instance;

    PlayerObjInteraction playerObjInteraction;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        //Instantiate();
    }

    void Instantiate()
    {
        StartCoroutine(FindPlayerCo());
    }
    IEnumerator FindPlayerCo()
    {
        while (true)
        {
            playerObjInteraction = FindAnyObjectByType<PlayerObjInteraction>();
            if (playerObjInteraction != null)
            {
                break;
            }
            yield return null;
        }
    }

    IEnumerator Off()
    {
        while (true)
        {
            playerObjInteraction = FindAnyObjectByType<PlayerObjInteraction>();
            if (playerObjInteraction != null)
            {
                playerObjInteraction.UsingUI = true;
                break;
            }
            yield return null;
        }
    }
    IEnumerator On()
    {
        while (true)
        {
            playerObjInteraction = FindAnyObjectByType<PlayerObjInteraction>();
            if (playerObjInteraction != null)
            {
                playerObjInteraction.UsingUI = false;
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 플레이어 Ray 사용 안함
    /// </summary>
    public void Ray_Off()
    {
        if (playerObjInteraction == null)
        {
            StartCoroutine(Off());
        }
        else
        {
            playerObjInteraction.UsingUI = true;
        }        
    }

    /// <summary>
    /// 플레이어 Ray 사용 
    /// </summary>
    public void Ray_On()
    {
        if (playerObjInteraction == null)
        {
            StartCoroutine(On());
        }
        else
        {
            playerObjInteraction.UsingUI = false;
        }
    }
}
