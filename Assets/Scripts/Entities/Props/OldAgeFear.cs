using System;
using Interfaces;
using Systems;
using Systems.Abilities;
using UnityEngine;

namespace Entities.Props
{
    /// <summary>
    /// Fear buff: reduces warmth consumption and doubles movement speed.
    /// </summary>
    [Serializable]
    public class OldAgeFear : IFearBuff
    {
        [SerializeField] private float _warmthConsumptionMultiplier = 0.5f;
        [SerializeField] private float _speedMultiplier = 2f;

        [NonSerialized] private bool _applied;
        [NonSerialized] private float _prevWarmthMultiplier = 1f;
        [NonSerialized] private float _prevMoveForce;

        public void Apply(WarmthSystem warmthSystem, PlayerMovement playerMovement)
        {
            if (_applied) return;
            _applied = true;

            if (warmthSystem != null)
            {
                _prevWarmthMultiplier = warmthSystem.WarmthConsumptionMultiplier;
                warmthSystem.WarmthConsumptionMultiplier = _prevWarmthMultiplier * _warmthConsumptionMultiplier;
            }

            if (playerMovement != null)
            {
                _prevMoveForce = playerMovement.MoveForce;
                playerMovement.MoveForce = _prevMoveForce * _speedMultiplier;
            }
        }

        public void Remove(WarmthSystem warmthSystem, PlayerMovement playerMovement)
        {
            if (!_applied) return;
            _applied = false;

            if (warmthSystem != null)
            {
                warmthSystem.WarmthConsumptionMultiplier = _prevWarmthMultiplier;
            }

            if (playerMovement != null)
            {
                playerMovement.MoveForce = _prevMoveForce;
            }
        }
    }
}

