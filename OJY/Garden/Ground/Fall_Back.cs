using Suncheon;
using Suncheon.Player;
using UnityEngine;

public class Fall : MonoBehaviour
{
    [SerializeField] Transform[] pos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMoveManager>() != null)
        {
            SetClosestPosition(other.transform.position);
        }
    }

    private void SetClosestPosition(Vector3 playerPosition)
    {
        if (pos.Length == 0)
        {
            //Debug.LogError("pos array is empty!");
            return;
        }

        Transform closestTransform = pos[0];
        float closestDistance = Vector3.Distance(playerPosition, pos[0].position);

        for (int i = 1; i < pos.Length; i++)
        {
            float currentDistance = Vector3.Distance(playerPosition, pos[i].position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestTransform = pos[i];
            }
        }

        NetworkManager.Instance.Go_Player.transform.position = closestTransform.position;
    }
}
