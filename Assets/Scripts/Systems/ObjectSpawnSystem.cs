using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Systems
{
    public class ObjectSpawnSystem
    {
        public static async Task<T> Spawn<T>(T obj, Vector3 position = new(), Transform parent = null, int delay = 0) where T : Component
        {
            await UniTask.Delay(delay);
            return Object.Instantiate(obj, position, Quaternion.identity, parent);
        }
    }
}