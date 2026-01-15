using UnityEngine;

public class PositionCheck : MonoBehaviour
{
    public bool Crash = false;

    private void OnTriggerEnter(Collider other)
    {
        Crash = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Crash = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Crash = false;
    }
}
