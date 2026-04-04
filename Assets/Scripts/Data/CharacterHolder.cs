using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Character Holder", menuName = "Configs/Character Holder")]
    public class CharacterHolder : ScriptableObject
    {
        [SerializeField] private List<CharacterConfig> _characters;
        
        public List<CharacterConfig> Characters => _characters;
        public Sprite UnknownSprite;
    }

    [Serializable]
    public record CharacterConfig
    {
        public Character Character;
        public AudioClip Sound;
        public SerializedDictionary<CharacterEmotion, Sprite> EmotionSprites;
    }
}