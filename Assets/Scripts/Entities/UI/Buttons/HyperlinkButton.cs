using UnityEngine;

namespace Entities.UI
{
    public class HyperlinkButton : MonoBehaviour
    {
        public void OpenLink(string link)
        {
            Application.OpenURL(link);
        }
    }
}