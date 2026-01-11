using AYellowpaper.SerializedCollections;
using Data;
using UnityEngine;

namespace Entities.UI
{
    public class PopupSystem : MonoBehaviour
    {
        [SerializeField] private NotificationPopup _normalNotificationPopup;
        [SerializeField, SerializedDictionary] private SerializedDictionary<PopupType, Transform> _popupHolders;

        private PopupBase _currentPopup;
        
        public void ShowPopup(PopupBase data)
        {
            PopupBase normalData = null;
            switch (data.Type)
            {
                case PopupType.Notification:
                    normalData = _normalNotificationPopup;
                    break;
            }
            data.Setup(normalData, _popupHolders[data.Type]);
            _currentPopup = data;
        }
        
        public void ClosePopup(PopupBase data)
        {
            _currentPopup.ClosePopup();
        }
    }
}