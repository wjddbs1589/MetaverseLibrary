using Suncheon.UI;
using UnityEngine;

namespace Suncheon
{
    public class ArchivesBook : MonoBehaviour, Clickable
    {
        [SerializeField] GameObject Archives_Book_UI;
        [HideInInspector]public bool waiting = false;
        public void OnClick()
        {
            if (!waiting)
            {
                UIInteractionManager.Instance.OpenPopUp(Archives_Book_UI);
            }
        }

        [SerializeField] string obj_Name;
        public string Return_ObjName()
        {
            return obj_Name;
        }
    }
}
