using Cysharp.Threading.Tasks;
using Data;
using Entities.Localization;
using TriInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    [DeclareFoldoutGroup("Sliders")]
    [DeclareFoldoutGroup("Switchers")]
    [DeclareFoldoutGroup("Toggles")]
    public class SettingsUI : MonoBehaviour
    {
        [Title("Assets")]
        [SerializeField] private AudioMixer _mixer;
        
        [GroupNext("Sliders")]
        [SerializeField] private Slider _mainSoundSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;

        [GroupNext("Switchers")] 
        [SerializeField] private Switcher _graphicsSwitcher;
        [SerializeField] private Switcher _languageSwitcher;
        
        [GroupNext("Toggles")] 
        [SerializeField] private Toggle _fullScreenToggle;

        private GlobalData _globalData;
        private SettingsData _localData;
        private bool _isSaving;
        
        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
        }

        private void Awake()
        {
            //Load Data
            _localData = _globalData.Get<SettingsData>();
            //Apply Data
            Screen.fullScreenMode = _localData.FullscreenMode ? 
                FullScreenMode.FullScreenWindow : FullScreenMode.MaximizedWindow;
            LocalizationManager.CurrentLanguage.Value = (Language)_localData.Language;
            LocalizationManager.CurrentLanguage.ForceNotify();
            QualitySettings.SetQualityLevel(_localData.QualityLevel);
            ChangeVolume("MainVolume", _localData.MainSoundVolume);
            ChangeVolume("MusicVolume", _localData.MusicVolume);
            ChangeVolume("SfxVolume", _localData.SfxVolume);
            //UpdateUI
            _fullScreenToggle.isOn = _localData.FullscreenMode;
            _graphicsSwitcher.CurrentValue = _localData.QualityLevel;
            _languageSwitcher.CurrentValue = _localData.Language;
            _mainSoundSlider.value = _localData.MainSoundVolume;
            _musicSlider.value = _localData.MusicVolume;
            _sfxSlider.value = _localData.SfxVolume;
            //Add Listeners
            _fullScreenToggle.onValueChanged.AddListener(ChangeFullScreenState);
            _graphicsSwitcher.Event.AddListener(ChangeQuality);
            _languageSwitcher.Event.AddListener(ChangeLanguage);
            _mainSoundSlider.onValueChanged.AddListener(ChangeMainVolume);
            _musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            _sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
        }
        
        public void ChangeFullScreenState(bool value)
        {
            Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.MaximizedWindow;
            _localData.FullscreenMode = value;
            SaveData();
        }
        
        public void ChangeQuality(int value)
        {
            QualitySettings.SetQualityLevel(value);
            _localData.QualityLevel = value;
            SaveData();
        }
        
        public void ChangeLanguage(int value)
        {
            LocalizationManager.CurrentLanguage.Value = (Language)value;
            _localData.Language = value;
            SaveData();
        }
        
        public void ChangeMainVolume(float value)
        {
            ChangeVolume("MainVolume", value);
            _localData.MainSoundVolume = value;
            SaveData();
        }

        public void ChangeMusicVolume(float value)
        {
            ChangeVolume("MusicVolume", value);
            _localData.MusicVolume = value;
            SaveData();
        }
        
        public void ChangeSfxVolume(float value)
        {
            ChangeVolume("SfxVolume", value);
            _localData.SfxVolume = value;
            SaveData();
        }

        private void ChangeVolume(string groupName, float value)
        {
            value = 20 * Mathf.Log10(value);
            _mixer.SetFloat(groupName, value);
        }

        private async void SaveData()
        {
            if (_isSaving) return;
            _isSaving = true;
            await UniTask.Delay(1000);
            _globalData.Edit<SettingsData>(x => x = _localData);
            _isSaving = false;
        }
    }
}
