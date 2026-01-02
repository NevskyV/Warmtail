using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Systems.Effects
{
    public class CrossfadeEffect
    {
        public static async IAsyncEnumerable<(float, float value)> CrossfadeTwins(float duration)
        {
            var value = 0f;
            var tween = DOTween.To(() => value,x => value = x, 1f, duration);
            
            while (tween.IsActive())
            {
                yield return (1f - value, value);
                await UniTask.Yield();
            }
            
            yield return (0f, 1f);
        }
    }
}