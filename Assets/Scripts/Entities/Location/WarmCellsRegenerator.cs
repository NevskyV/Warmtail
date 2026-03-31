using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Location
{
    public class WarmCellsRegenerator : MonoBehaviour
    {
        [Inject] private WarmthSystem _warmthSystem;
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _warmthSystem.AddCell();
            }
        }
    }
}