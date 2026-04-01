using UnityEngine;

namespace Entities.Props
{
    public class AutoDestroy : MonoBehaviour
    {
        public void Destroy(float t = 0)
        {
            Destroy(gameObject, t);
        }
    }
}