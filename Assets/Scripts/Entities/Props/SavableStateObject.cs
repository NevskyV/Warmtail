using System;
using System.Linq;
using Data;
using TriInspector;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class SavableStateObject : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private UniqueID _id;
        [Inject] protected GlobalData _globalData;
        public Action<bool> OnStateChanged;
        
        public string Id => _id.Value;

        [Button("Copy")]
        private void Copy()
        {
            GUIUtility.systemCopyBuffer = Id;
        }
        
        private void ResetId()
        {
            _id.Value = Guid.NewGuid().ToString();
        }
        
        public static bool IsUnique(string id)
        {
            return Resources.FindObjectsOfTypeAll<SavableStateObject>().Count(x => x.Id == id) == 1;
        }

        protected void OnValidate()
        {
            if (!gameObject.scene.IsValid())
            {
                _id.Value = string.Empty;
                return;
            }

            if (string.IsNullOrEmpty(Id) || !IsUnique(Id))
            {
                ResetId();
            }
        }

        [Serializable]
        private struct UniqueID
        {
            public string Value;
        }
        
        public void ChangeState(bool active)
        {
            _globalData.Edit<WorldData>(worldData => {
                worldData.SavableObjects[_id.Value] = active; });
            gameObject.SetActive(active);
            OnStateChanged?.Invoke(active);
        }

        public void ChangeStateOnLoad(bool active)
        {
            _globalData.Edit<WorldData>(worldData => 
                { worldData.SavableObjects[_id.Value] = active; });
        }
        
        public void SaveNpcState(int state)
        {
            _globalData.Edit<WorldData>(worldData =>
                worldData.SavableNpcState[_id.Value] = state );
        }
    }
}