using Data;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class DeleteDataButton : MonoBehaviour
    {
        [Inject] private GlobalData _globalData;
        public void Delete() => _globalData.DeleteSaveData();
    }
}