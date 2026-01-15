using Suncheon.UI;
using Suncheon;
using UnityEngine;

public class Mascot_rumi : MonoBehaviour, Clickable
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ui_TalkBox;    

    [SerializeField] private GameObject ui_NpcChat;

    //[Range(0.0f, 300.0f)]
    //[SerializeField] private float maxDistance;
    float maxDistance = 15.0f;

    Animator anim;
    bool useAnim = false;
    float times = 10;
    int num;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerCheck();
    }

    void PlayerCheck()
    {
        if (NetworkManager.Instance == null) return;

        if (player == null)
        {
            player = NetworkManager.Instance.Go_Player;
        }

        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > maxDistance)
            {
                ui_TalkBox.SetActive(false);
            }
            else
            {
                ui_TalkBox.SetActive(true);
                Vector3 direction = player.transform.position - transform.position;
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;

                if (!useAnim)
                {
                    times -= Time.deltaTime;
                    if (times <= 0)
                    {
                        useAnim = true;
                        switch (RtnN())
                        {
                            case 0:
                                anim.SetTrigger("Talk");
                                break;
                            case 1:
                                anim.SetTrigger("Hand_Raising");
                                break;
                            case 2:
                                anim.SetTrigger("Samba");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    int RtnN()
    {
        int rn = Random.Range(0, 3);
        while (true)
        {
            if (rn != num)
            {
                num = rn;
                break;                
            }
            rn = Random.Range(0, 3);
        }
        return num;
    }

    public void ENdAnim()
    {
        useAnim = false;
        times = 10;
    }

    public void OnClick()
    {
        UIInteractionManager.Instance.OpenPopUp(ui_NpcChat);
    }

    public string Return_ObjName()
    {
        return "";
    }
}
