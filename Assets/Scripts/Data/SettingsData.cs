using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SettingsData : ISavableData
    {
        [Range(0.001f,1)] public float MainSoundVolume;
        [Range(0.001f,1)] public float MusicVolume;
        [Range(0.001f,1)] public float SfxVolume;
        public int QualityLevel;
        public bool FullscreenMode;
        public bool HDR;
        public int Language;

        [Range(0f, 1)] public float LongLowRumble;
        [Range(0f, 1)] public float LongHighRumble;
        [Range(0f, 1)] public float ShortLowRumble;
        [Range(0f, 1)] public float ShortHighRumble;
    }
}