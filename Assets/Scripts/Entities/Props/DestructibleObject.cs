using UnityEngine;
using Interfaces;

namespace Systems.Destructible
{
    public class DestructibleObject : MonoBehaviour, IDestroyable
    {
        [SerializeField] private float _destroyDelay = 0f;

        public void DestroyObject()
        {
            Debug.Log($"Объект '{gameObject.name}' уничтожается!");
           
            if (_destroyDelay > 0)
            {
                Destroy(gameObject, _destroyDelay);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}