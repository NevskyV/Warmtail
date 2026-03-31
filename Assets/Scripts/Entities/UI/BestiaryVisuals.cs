using System.Collections.Generic;
using Data;
using Entities.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class BestiaryVisuals : MonoBehaviour
    {
        [SerializeField] private List<CreatureConfig> _creatures;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _cardsParent;
        [SerializeField] private TMP_Text _progress;
        [Inject] private GlobalData _globalData;
        [Inject] private UIStateSystem _uiStateSystem;

        private void Start()
        {
            _uiStateSystem.OnStateChange += state =>
            {
                if(state == UIState.Bestiary) LoadConfigs();
            };
        }
        
        private void LoadConfigs()
        {
            for(int i = 0; i < _cardsParent.childCount; i++)
            {
                Destroy(_cardsParent.GetChild(i).gameObject);
            }

            int count = 0;
            foreach(var id in _globalData.Get<WorldData>().CreaturesIds)
            {
                var creature = _creatures[id];
                var obj = Instantiate(_cardPrefab, _cardsParent).transform;
                obj.GetChild(0).GetComponent<LocalizedText>().SetNewKey(creature.Name);
                obj.GetChild(1).GetComponent<LocalizedText>().SetNewKey(creature.Description);
                obj.GetChild(2).GetComponent<Image>().sprite = creature.Icon;
                count++;
            }
            
            _progress.text = Mathf.Ceil((count * 1.0f / _globalData.Get<WorldData>().MaxCreatures) * 100) + "%";
            if (_progress.text == "100%") _progress.color = Color.lightGreen;
        }
    }
}