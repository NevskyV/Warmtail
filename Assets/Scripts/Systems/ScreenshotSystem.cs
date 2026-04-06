using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Entities.Core;
using Entities.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Systems
{
    public class ScreenshotSystem : IDisposable
    {
        private readonly string _rootFolderPath;
        private CancellationTokenSource _cts;
        public Action<bool> ScreenShotState = null;
        private PlayerInput _playerInput;
        private SceneLoader _sceneLoader;
        private UIStateSystem _uiStateSystem;
        
        public static string LastScreenShotPath { get; private set; }

        [Inject]
        public ScreenshotSystem(PlayerInput playerInput, SceneLoader sceneLoader, UIStateSystem uiStateSystem)
        {
            _playerInput =  playerInput;
            _sceneLoader = sceneLoader;
            _uiStateSystem = uiStateSystem;
            _cts = new CancellationTokenSource();
            _rootFolderPath = Path.Combine(Application.persistentDataPath, "Screenshots");
            _sceneLoader.SceneStartLoading += DisableAutoScreenshot;
            _sceneLoader.SceneLoaded += EnableAutoScreenshot;
            _playerInput.actions["Screenshot"].performed += _ => TakeScreenShot(ScreenshotType.User);
        }

        public async void TakeScreenShot(ScreenshotType type)
        {
            if (_uiStateSystem.CurrentState != UIState.Normal) return;
            var fileName = $"Screenshot_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.png";
            
            var typeFolderPath = Path.Combine(_rootFolderPath, type.ToString());
            
            if (!Directory.Exists(typeFolderPath))
            {
                Directory.CreateDirectory(typeFolderPath);
            }
            
            var fullFilePath = Path.Combine(typeFolderPath, fileName);

            LastScreenShotPath =  fullFilePath;
            ScreenShotState?.Invoke(false);
            _uiStateSystem.ChangeObjectState(false);
            await UniTask.WaitForEndOfFrame();
            
            ScreenCapture.CaptureScreenshot(fullFilePath);

            await UniTask.WaitForEndOfFrame();
            ScreenShotState?.Invoke(true);

            _uiStateSystem.ChangeObjectState(true);
            
            Debug.Log($"Take {type} screenshot saved to: {fullFilePath}");
        }

        public void DeleteScreenShot()
        {
            File.Delete(LastScreenShotPath);
        }

        public void Dispose()
        {
            var tempDir = Path.Combine(_rootFolderPath, ScreenshotType.Temp.ToString());

            if (Directory.Exists(tempDir))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(tempDir))
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Could not cleanup temp screenshots: {e.Message}");
                }
            }

            DisableAutoScreenshot();
        }

        public async void EnableAutoScreenshot(string sceneName)
        {
            if (sceneName == "Start") return;
            if (_cts != null) _cts.Dispose();
            _cts = new CancellationTokenSource();

            while (_cts.IsCancellationRequested == false)
            {
                await UniTask.WaitForSeconds(100, cancellationToken: _cts.Token);
                TakeScreenShot(ScreenshotType.Temp);
            }
        }

        public void DisableAutoScreenshot()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
    public enum ScreenshotType {User, Temp}
}