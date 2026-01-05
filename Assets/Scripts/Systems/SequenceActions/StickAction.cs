using Data;
using Entities.Core;
using Entities.UI;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Systems.SequenceActions
{
    public class StickAction : ISequenceAction
    {
        [Inject] private GlobalData _globalData;
        public void Invoke()
        {
            _globalData.Edit<DialogueVarData>(data => {
                var a = data.Variables.Find(x => x.Name == "getStick");
                int pos = data.Variables.IndexOf(a);
                data.Variables[pos].Value = "true";
            });
        }
    }
}