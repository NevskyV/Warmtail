using System;
using Interfaces;
using Systems.Abilities;
using UnityEngine;

namespace Systems.Fears
{
    [Serializable]
    public class OldAgeFear : IFearBuff
    {
        [SerializeField] private float _warmthConsumptionMultiplier = 0.5f;
        [SerializeField] private float _speedMultiplier = 2f;

        public void Apply(WarmthSystem warmthSystem, PlayerMovement playerMovement)
        {
            if (warmthSystem != null)
            {
                warmthSystem.SetWarmthConsumptionMultiplier(_warmthConsumptionMultiplier);
            }

            if (playerMovement != null)
            {
                playerMovement.SetSpeedMultiplier(_speedMultiplier);
            }
        }

        public void Remove(WarmthSystem warmthSystem, PlayerMovement playerMovement)
        {
            if (warmthSystem != null)
            {
                warmthSystem.SetWarmthConsumptionMultiplier(1f);
            }

            if (playerMovement != null)
            {
                playerMovement.SetSpeedMultiplier(1f);
            }
        }
    }
}
