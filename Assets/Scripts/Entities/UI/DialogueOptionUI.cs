using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class DialogueOptionUI : MonoBehaviour
    {
        [Inject] private DialogueVisuals _dialogueVisuals;
        
        public void ChooseOption()
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                {
                    _dialogueVisuals.ChooseOption(i);
                    break;
                }
            }
        }
    }
}