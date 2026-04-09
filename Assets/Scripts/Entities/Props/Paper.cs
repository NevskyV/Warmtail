using System;
using Entities.UI;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.Props
{
    public class Paper : MonoBehaviour
    {
        [SerializeField] private Chapter _chapter;
        [SerializeField] private int _page;
        [Inject] private ManualSystem _manualSystem;
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _manualSystem.AddPage(_chapter, _page);
            }
        }
    }
}