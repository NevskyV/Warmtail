using System;
using DG.Tweening;
using TMPro;
using TriInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Data
{
    [Serializable]
    public abstract class PopupBase
    {
        public PopupType Type { get; protected set; }
        [field:SerializeReference] public GameObject Prefab { get; private set; }
        public string Header { get; private set; }
        public string Text { get; private set; }
        [field:SerializeReference, Unit("Seconds")] public float AnimDuration { get; private set; }

        protected Transform _instanceTf;

        public PopupBase(string header, string text, GameObject prefab = null)
        {
            Header = header;
            Text = text;
            Prefab = prefab;
        }
        
        public virtual void Setup(PopupBase data, Transform parent)
        {
            Prefab ??= data.Prefab;
            AnimDuration = data.AnimDuration;
            _instanceTf = Object.Instantiate(Prefab, parent).transform;
            _instanceTf.localScale = Vector3.zero;
            _instanceTf.DOScale( 1, AnimDuration);
            _instanceTf.GetChild(0).GetComponent<TMP_Text>().text = Header;
            _instanceTf.GetChild(1).GetComponent<TMP_Text>().text = Text;
        }

        public async void ClosePopup()
        {
            _instanceTf.localScale = Vector3.one;
            await _instanceTf.DOScale( 0, AnimDuration).AsyncWaitForCompletion();
            Object.Destroy(_instanceTf?.gameObject);
        }
    }
    
    public enum PopupType {Notification, Modular, Overlapping}
}