using Systems;
using UnityEngine;
using Zenject;

namespace Entities.UI.Buttons
{
    public class CreateScreenshotButton : MonoBehaviour
    {
        [Inject] private ScreenshotSystem _screenshotSystem;

        public void TakeScreenshot()
        {
            _screenshotSystem.TakeScreenShot(ScreenshotType.User);
        }
    }
}