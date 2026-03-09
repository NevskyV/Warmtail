using System.Diagnostics;
using System.IO;
using Systems;
using UnityEngine;
using Zenject;

namespace Entities.UI.Buttons
{
    public class FilesActionsButton : MonoBehaviour
    {
        [SerializeField] private string _path;
        [SerializeField] private bool _usePersistent;
        [Inject] private ScreenshotSystem _screenshotSystem;
        
        public void OpenFolder()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Process.Start(new ProcessStartInfo()
            {
                FileName = _usePersistent? Application.persistentDataPath + _path: _path,
                UseShellExecute = true,
                Verb = "open"
            });
#endif
        }

        public void DeleteFile()
        {
            _screenshotSystem.DeleteScreenShot();
        }
    }
}