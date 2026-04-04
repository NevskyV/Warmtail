using UnityEngine;

namespace Data
{
    public enum CompanionType { Fish, Jellyfish, Crab, SeaRabbit }

    public class CompanionItemData : BuyableItemData
    {
        public GameObject CompanionPrefab;
        public CompanionType Type;
    }
}