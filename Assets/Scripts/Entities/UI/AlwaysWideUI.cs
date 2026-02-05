using UnityEngine;

namespace Entities.UI
{
    [ExecuteAlways]
    public class AlwaysWideUI : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public void OnEnable()
        {
            _rectTransform =  GetComponent<RectTransform>();
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _rectTransform.rect.size.x);
        }
    }
}