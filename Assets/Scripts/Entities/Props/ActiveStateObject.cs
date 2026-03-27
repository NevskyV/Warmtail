using UnityEngine;
using System;

namespace Entities.Props
{
    public class ActiveStateObject : MonoBehaviour
    {
        public Action<bool> OnActiveChanged;

        private void OnEnable()
        {
            OnActiveChanged?.Invoke(true);
        }

        private void OnDisable()
        {
            OnActiveChanged?.Invoke(false);
        }
    }
}