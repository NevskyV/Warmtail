using System.Collections.Generic;
using Data;
using DG.Tweening;
using Entities.Localization;
using Systems;
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
        [SerializeField] private CanvasGroup _mainGroup;
        [SerializeField] private CanvasGroup _cardInfoGroup;
        [SerializeField] private LocalizedText _cardName;
        [SerializeField] private LocalizedText _cardDescription;
        [SerializeField] private Image _cardIcon;

        [SerializeField] private BuyableItemData _rewardData;
        [Inject] private GlobalData _globalData;
        [Inject] private UIStateSystem _uiStateSystem;
        [Inject] private ShoppingSystem _shoppingSystem;

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
                obj.GetComponent<Button>().onClick.AddListener(() => ShowInfo(id));
                obj.GetChild(0).GetComponent<LocalizedText>().SetNewKey(creature.Name);
                
                obj.GetChild(1).GetComponent<Image>().sprite = creature.Icon;
                count++;
            }
            
            _progress.text = Mathf.Ceil((count * 1.0f / _globalData.Get<WorldData>().MaxCreatures) * 100) + "%";
            if (_progress.text == "100%")
            {
                _progress.color = Color.lightGreen;
                _globalData.Edit<DialogueVarData>(data =>
                    data.Variables.Find(x => x.Name == "bestiaryComplete").Value = "true");
            }
        }

        public void GetReward()
        {
            _shoppingSystem.BuyItem(_rewardData, Character.Finix, true);
        }

        private void ShowInfo(int id)
        {
            var creature = _creatures[id];
            _cardName.GetComponent<LocalizedText>().SetNewKey(creature.Name);
            _cardDescription.GetComponent<LocalizedText>().SetNewKey(creature.Description);
            _cardIcon.sprite = creature.Icon;
            
            _mainGroup.interactable = false;
            _mainGroup.blocksRaycasts = false;
            _mainGroup.DOFade(0, 1f);
            
            _cardInfoGroup.interactable = true;
            _cardInfoGroup.blocksRaycasts = true;
            _cardInfoGroup.DOFade(1, 1f);
        }

        public void HideInfo()
        {
            _mainGroup.interactable = true;
            _mainGroup.blocksRaycasts = true;
            _mainGroup.DOFade(1, 1f);
            
            _cardInfoGroup.interactable = false;
            _cardInfoGroup.blocksRaycasts = false;
            _cardInfoGroup.DOFade(0, 1f);
        }
    }
}