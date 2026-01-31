using Interfaces;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "FearConfig", menuName = "Configs/FearConfig")]
    public class FearConfig : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeReference] private IFearBuff _buff;

        public int Id => _id;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public IFearBuff Buff => _buff;
    }
}
