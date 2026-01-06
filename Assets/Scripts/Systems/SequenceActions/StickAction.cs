using Entities.Core;
using Entities.UI;
using System;
using Interfaces;

namespace Systems.SequenceActions
{
    public class StickAction : ISequenceAction
    {
        public static Action OnStickTaked;

        public void Invoke()
        {
            OnStickTaked?.Invoke();
        }
    }
}