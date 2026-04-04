using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Entities.Core;
using Entities.UI;
using UnityEngine;
using Zenject;

namespace Systems
{
    public class WarmMemoriesSystem : IDisposable
    {
        private readonly ScreenshotSystem _screenshotSystem;
        private readonly SceneLoader _sceneLoader;
        private readonly WarmMemoriesUI _ui;

        private bool _triggered;

        [Inject]
        public WarmMemoriesSystem(ScreenshotSystem screenshotSystem, SceneLoader sceneLoader,
            [InjectOptional] WarmMemoriesUI ui)
        {
            _screenshotSystem = screenshotSystem;
            _sceneLoader = sceneLoader;
            _ui = ui;

            _sceneLoader.SceneStartLoading += OnSceneStartLoading;
        }

        private void OnSceneStartLoading()
        {
            if (_triggered) return;
            _triggered = true;
            ShowSlideshowAsync().Forget();
        }

        private async UniTaskVoid ShowSlideshowAsync()
        {
            var paths = CollectScreenshotPaths();
            if (paths.Count == 0 || _ui == null)
                return;

            var textures = await LoadTexturesAsync(paths);
            if (textures.Count == 0) return;

            await _ui.PlaySlideshow(textures);

            foreach (var tex in textures)
                UnityEngine.Object.Destroy(tex);
        }

        private List<string> CollectScreenshotPaths()
        {
            var root = Path.Combine(Application.persistentDataPath, "Screenshots");
            var result = new List<string>();

            foreach (var type in new[] { "User", "Temp" })
            {
                var dir = Path.Combine(root, type);
                if (!Directory.Exists(dir)) continue;
                result.AddRange(Directory.GetFiles(dir, "*.png"));
            }

            result.Sort();
            return result;
        }

        private async UniTask<List<Texture2D>> LoadTexturesAsync(List<string> paths)
        {
            var textures = new List<Texture2D>();
            foreach (var path in paths)
            {
                await UniTask.SwitchToMainThread();
                try
                {
                    var bytes = await UniTask.RunOnThreadPool(() => File.ReadAllBytes(path));
                    var tex = new Texture2D(2, 2);
                    if (tex.LoadImage(bytes))
                        textures.Add(tex);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"WarmMemories: failed to load {path}: {e.Message}");
                }
            }
            return textures;
        }

        public void Dispose()
        {
            _sceneLoader.SceneStartLoading -= OnSceneStartLoading;
        }
    }
}
