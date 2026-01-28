using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Systems
{
    public class ScreenshotSystem : IDisposable
    {
        private readonly string _rootFolderPath;
        private CancellationTokenSource _cts;
        public Action<bool> ScreenShotState = null;

        public ScreenshotSystem()
        {
            _cts = new CancellationTokenSource();
            _rootFolderPath = Path.Combine(Application.persistentDataPath, "Screenshots");
        }

        public async void TakeScreenShot(ScreenshotType type)
        {
            var fileName = $"Screenshot_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.png";
            
            var typeFolderPath = Path.Combine(_rootFolderPath, type.ToString());
            
            if (!Directory.Exists(typeFolderPath))
            {
                Directory.CreateDirectory(typeFolderPath);
            }
            
            var fullFilePath = Path.Combine(typeFolderPath, fileName);

            ScreenShotState?.Invoke(false);
            await UniTask.WaitForEndOfFrame();
            
            ScreenCapture.CaptureScreenshot(fullFilePath);

            await UniTask.WaitForEndOfFrame();
            ScreenShotState?.Invoke(true);

            Debug.Log($"Take {type} screenshot saved to: {fullFilePath}");
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

        public async UniTaskVoid EnableAutoScreenshot()
        {
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