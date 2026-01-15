using Suncheon;
using System.Collections;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    //플레이어의 카메라 주시
    Camera cam;

    private void Start()
    {
        Find_Player();
    }

    private void Update()
    {
        if (cam != null)
        {
            transform.LookAt(cam.transform);
        }
    }

    void Find_Player()
    {
        StartCoroutine(Find_Co());
    }

    IEnumerator Find_Co()
    {
        while (true)
        {
            if (NetworkManager.Instance.Go_Player != null)
            {
                cam = NetworkManager.Instance.Go_Player.GetComponentInChildren<Camera>();

                if (cam != null)
                {
                    break;
                }
            }

            yield return null;
        }
        
    }
}
