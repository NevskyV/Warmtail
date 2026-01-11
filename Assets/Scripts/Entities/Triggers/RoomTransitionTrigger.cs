using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Systems.Effects;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

namespace Entities.Triggers
{
    public class RoomTransitionTrigger : MonoBehaviour
    {
        [SerializeField] private Collider2D[] _normalColliders;
        [SerializeField] private Collider2D[] _hiddenColliders;
        [SerializeField] private SpriteShapeRenderer[] _normalSprites;
        [SerializeField] private SpriteShapeRenderer[] _hiddenSprites;
        [SerializeField] private ShadowCaster2D[] _normalShadowCasters;
        [SerializeField] private ShadowCaster2D[] _hiddenShadowCasters;

        private void Start()
        {
            for (int i = 0; i < _normalColliders.Length; i++)
            {
                _normalColliders[i].OnTriggerEnter2DAsObservable().Subscribe(TriggerEnter2D);
                _hiddenColliders[i].OnTriggerEnter2DAsObservable().Subscribe(HiddenTriggerEnter2D);
            }
        }
        
        private void TriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                for (int i = 0; i < _hiddenSprites.Length; i++)
                {
                    Fade(_normalSprites[i], _hiddenSprites[i]);
                }
                _normalColliders.ForEach(c => c.gameObject.SetActive(false));
                _hiddenColliders.ForEach(c => c.gameObject.SetActive(true));
                
                _normalShadowCasters.ForEach(c => c.enabled = false);
                _hiddenShadowCasters.ForEach(c => c.enabled = true);
            }
        }

        private void HiddenTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                for (int i = 0; i < _hiddenSprites.Length; i++)
                {
                    Fade(_hiddenSprites[i], _normalSprites[i]);
                }
                _normalColliders.ForEach(c => c.gameObject.SetActive(true));
                _hiddenColliders.ForEach(c => c.gameObject.SetActive(false));
                
                _normalShadowCasters.ForEach(c => c.enabled = true);
                _hiddenShadowCasters.ForEach(c => c.enabled = false);
            }
        }

        private async void Fade(SpriteShapeRenderer normalSprite, SpriteShapeRenderer hiddenSprite)
        {
            var normalColor = normalSprite.color;
            var hiddenColor = hiddenSprite.color;
            await foreach (var (a, b) in  CrossfadeEffect.CrossfadeTwins(1))
            {
                normalColor.a = a;
                hiddenColor.a = b;
                hiddenSprite.color = hiddenColor;
                normalSprite.color = normalColor;
            }
        }
    }
}