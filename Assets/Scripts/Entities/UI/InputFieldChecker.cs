using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI
{
    public class InputFieldChecker : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextAsset _textAsset;
        
        public void OnValueChanged(string value)
        {
            if (value == "" || _textAsset.ToString().Contains(value.ToLower()))
            {
                _button.interactable = false;
            }
            else
            {
                _button.interactable = true;
            }
        }
    }
}