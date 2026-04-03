using Entities.Location;
using Interfaces;
using UnityEngine;

namespace Entities.Location
{
    [RequireComponent(typeof(DumboSwarmController))]
    public class WarmableDumboSwarm : Warmable
    {
        private DumboSwarmController _dumboController;

        private void Awake()
        {
            _dumboController = GetComponent<DumboSwarmController>();
        }

        public override void WarmComplete()
        {
            if (_dumboController.IsCompressed)
                _dumboController.ExpandFromBall();
            else
                _dumboController.CompressIntoBall();
        }

        public override void Reset()
        {
            base.Reset();
            if (_dumboController.IsCompressed)
                _dumboController.ExpandFromBall();
        }
    }
}
