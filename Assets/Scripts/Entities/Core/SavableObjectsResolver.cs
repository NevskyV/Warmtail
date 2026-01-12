using System.Collections.Generic;
using Data;
using Entities.NPC;
using Entities.Probs;
using UnityEngine;
using Zenject;

namespace Entities.Core
{
    public class SavableObjectsResolver : MonoBehaviour
    {
        [Inject] private GlobalData _data;
        private static Dictionary<string, GameObject> _objects = new();

        private void Start()
        {
            var changed = _data.Get<WorldData>().SavableObjects;
            var changedNpc = _data.Get<WorldData>().SavableNpcState;

            foreach (var obj in FindObjectsByType<SavableStateObject>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                if (_objects.ContainsKey(obj.Id)) _objects.Remove(obj.Id);
                _objects.Add(obj.Id, obj.gameObject);
                if (changed.ContainsKey(obj.Id))
                {
                    obj.gameObject.SetActive(changed[obj.Id]);
                }

                if (changedNpc.ContainsKey(obj.Id))
                {
                    ((SpeakableCharacter)obj).SavableState[changedNpc[obj.Id]].Invoke();
                }
            }
        }

        public static GameObject FindObjectById(string id)
        {
            _objects.TryGetValue(id, out var o);
            return o;
        }
        
        public static T FindObjectById<T>(string id) where T : MonoBehaviour
        {
            _objects.TryGetValue(id, out var o);
            T t = null;
            o?.TryGetComponent(out t);
            return t;
        }
    }
}