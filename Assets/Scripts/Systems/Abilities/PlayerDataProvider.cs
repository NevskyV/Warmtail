using System;
using System.Linq;
using Data;
using Data.Player;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;
using Zenject;

namespace Systems.Abilities
{
    public class PlayerDataProvider
    {
        private GlobalData _globalData;

        [Inject]
        public void Construct(GlobalData globalData)
        {
            _globalData = globalData;
        }

        public string GetProperty(string name)
        {
            var obj = _globalData.Get<RuntimePlayerData>();
            return obj.GetType().GetField(name).GetValue(obj).ToString();
        }

        public void SetProperty(string name, string value)
        {
            try
            {
                _globalData.Edit<RuntimePlayerData>(data =>
                {
                    var field = data.GetType().GetField(name);
                    var t = field.GetType();
                    field.SetValue(data, Convert.ChangeType(value,t));
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

