using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Systems
{
    public class ScreenshotSystem : IDisposable
    {
        private readonly string _folderPath;
        private CancellationTokenSource _cts;
        public Action<bool> ScreenShotState = null;
        
        public ScreenshotSystem()
        {
            _cts = new CancellationTokenSource();
            _folderPath = Application.persistentDataPath + "/Screenshots";
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
                Directory.CreateDirectory(_folderPath + "/User");
                Directory.CreateDirectory(_folderPath + "/Temp");
            }
        }
        
        public async void TakeScreenShot(ScreenshotType type)
        {
            var fileName = $"Screenshot_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.png";
            ScreenShotState?.Invoke(false);
            await UniTask.WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot($"{_folderPath}/{type}/{fileName}");
            await UniTask.WaitForEndOfFrame();
            ScreenShotState?.Invoke(true);
            Debug.Log($"Take {type} screenshot");
        }

        public void Dispose()
        {
            var tempDir = $"{_folderPath}/Temp";
            if (Directory.Exists(tempDir))
            {
                foreach (var file in Directory.GetFiles(tempDir))
                {
                    File.Delete(file);
                }
            }
        }

        public async UniTaskVoid EnableAutoScreenshot()
        {
            _cts = new CancellationTokenSource();
            while (_cts.IsCancellationRequested == false)
            {
                await UniTask.WaitForSeconds(120, cancellationToken: _cts.Token);
                TakeScreenShot(ScreenshotType.Temp);
            }
        }

        public void DisableAutoScreenshot()
        {
            if(_cts != null) _cts.Cancel();
        }
    }
    
    public enum ScreenshotType {User, Temp}
}