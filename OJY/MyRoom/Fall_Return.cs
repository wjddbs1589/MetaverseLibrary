using Suncheon;
using UnityEngine;

public class Fall_Return : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Instance.Go_Player == other.gameObject)
        {
            other.transform.position = transform.parent.position;
        }
    }
}
