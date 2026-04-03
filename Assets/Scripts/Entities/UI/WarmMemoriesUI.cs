using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Entities.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class WarmMemoriesUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RawImage _slideImage;
        [SerializeField] private CanvasGroup _slideCanvasGroup;
        [SerializeField] private Image _fadeOverlay;
        [SerializeField] private float _fadeDuration = 0.6f;
        [SerializeField] private float _holdDuration = 2.5f;
        [SerializeField] private float _finalFadeToBlackDuration = 1.5f;

        private SceneLoader _sceneLoader;

        [Inject]
        private void Construct(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            if (_canvasGroup) _canvasGroup.alpha = 0f;
            if (_fadeOverlay)
            {
                var c = _fadeOverlay.color;
                c.a = 0f;
                _fadeOverlay.color = c;
            }
        }

        public async UniTask PlaySlideshow(List<Texture2D> textures)
        {
            if (textures == null || textures.Count == 0) return;

            gameObject.SetActive(true);

            if (_canvasGroup)
                await _canvasGroup.DOFade(1f, _fadeDuration).AsyncWaitForCompletion();

            foreach (var tex in textures)
            {
                if (tex == null) continue;
                await ShowSlide(tex);
            }

            await FadeToBlack();

            gameObject.SetActive(false);
        }

        private async UniTask ShowSlide(Texture2D texture)
        {
            if (_slideImage) _slideImage.texture = texture;

            if (_slideCanvasGroup)
            {
                _slideCanvasGroup.alpha = 0f;
                await _slideCanvasGroup.DOFade(1f, _fadeDuration).AsyncWaitForCompletion();
                await UniTask.Delay(TimeSpan.FromSeconds(_holdDuration));
                await _slideCanvasGroup.DOFade(0f, _fadeDuration).AsyncWaitForCompletion();
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_holdDuration));
            }
        }

        private async UniTask FadeToBlack()
        {
            if (_fadeOverlay == null) return;

            await _fadeOverlay.DOFade(1f, _finalFadeToBlackDuration).AsyncWaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
}
