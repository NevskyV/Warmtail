using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Entities.Localization;
using Systems.Abilities;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class AbilitiesUI : MonoBehaviour
    {
        [Serializable]
        public struct AbilityImages
        {
            public List<RectTransform> Images;
        }
        
        [Serializable]
        public struct NewAbilityUI
        {
            public Image Icon;
            public LocalizedText Name;
            public LocalizedText Description;
        }

        [Title("Configs")] 
        [SerializeField] private List<AbilityUIConfig> _abilitiesConfigs;
        
        [Title("Images to fill")] 
        [SerializeField] private AbilityImages[] _imagesArray;
        [SerializeField] private Image[] _images;
        [SerializeField] private Transform[] _rhombuses;
        [SerializeField] private float _distance;
        
        [Title("Selection")] 
        [SerializeField] private float _defaultOutWidth;
        [SerializeField] private float _selectedOutWidth;
        [SerializeField] private float _defaultInOutWidth;
        [SerializeField] private float _selectedInOutWidth;
        [Title("Confirmation")]
        [SerializeField] private float _defaultInWidth;
        [SerializeField] private float _confirmedInWidth;
        [SerializeField] private float _defaultRhombusSize;
        [SerializeField] private float _confirmedRhombusSize;
        [Title("Casting")]
        [SerializeField] private float _defaultAmplitude;
        [SerializeField] private float _activeAmplitude;
        [SerializeField] private float _defaultOpacity;
        [SerializeField] private float _activeOpacity;
        [Title("New ability")] 
        [SerializeField] private Transform _mainObject;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private NewAbilityUI _newAbilityUI; 
        
        private List<WarmthAbility> _warmthAbilities;
        private PlayerConfig _playerConfig;
        private AbilitiesSystem _abilitiesSystem;

        [Inject]
        private void Construct(PlayerConfig playerConfig, AbilitiesSystem abilitiesSystem)
        {
            _playerConfig = playerConfig;
            _abilitiesSystem =  abilitiesSystem;
            _warmthAbilities = _playerConfig.Abilities.OfType<WarmthAbility>().ToList();
            
            _abilitiesSystem.OnSelect += SelectAbility;
            _abilitiesSystem.OnConfirm += ConfirmAbility;
            _abilitiesSystem.OnCast += Cast;
            _abilitiesSystem.OnStopCast += StopCast;
            _abilitiesSystem.OnAddAbility += AddAbility;
        }

        private void Start()
        {
            ShowAbilities();
            _confirmButton.onClick.AddListener(() => ShowAbilities());
        }

        private void OnDestroy()
        {
            _abilitiesSystem.OnSelect -= SelectAbility;
            _abilitiesSystem.OnConfirm -= ConfirmAbility;
            _abilitiesSystem.OnCast -= Cast;
            _abilitiesSystem.OnStopCast -= StopCast;
            _abilitiesSystem.OnAddAbility -= AddAbility;

            foreach (var image in _images)
            {
                var parent = image.transform.parent.GetComponent<Image>();
                parent.material.SetFloat("_OutlineThickness",0);
                parent.material.SetFloat("_InOutlineThickness",0);
                parent.material.SetFloat("_InlineThickness",0);
                parent.material.SetFloat("_WaveAmplitude",0);
                parent.material.SetFloat("_Alpha",1);
            }
        }

        private void ShowAbilities(bool show = true)
        {
            _confirmButton.interactable = false;
            _mainObject.DOLocalMoveY(-300, 2f);
            _confirmButton.transform.parent.DOLocalMoveY(-300, 1.5f);
            int index = 0;
            var size = _images[0].GetComponent<RectTransform>().sizeDelta.x;
            
            var inUse = _warmthAbilities.Where(x => x.InUse).ToList();
            var maxDist = inUse.Count * size + _distance * (inUse.Count - 1);
            
            foreach (var ability in _warmthAbilities)
            {
                var imges = _imagesArray[_warmthAbilities.IndexOf(ability)].Images;
                foreach (var img in imges)
                {
                    img.localPosition = new Vector3(0, -300);
                }
                
                if(inUse.Contains(ability)){
                    
                    imges[2].GetComponent<Image>().sprite = _abilitiesConfigs.Find(x => GetAbilityType(x.Type) == ability.GetType()).Sprite;
                    imges[2].transform.parent.gameObject.SetActive(show);
                    imges[0].DOSizeDelta(new Vector2(100,100), 0.5f);
                    foreach (var img in imges)
                    {
                        img.DOLocalMove(new Vector3(-(maxDist - size) / 2 + (_distance + size) * index, 100), 0.5f);
                    }

                    index++;
                }
                else
                {
                    _images[_warmthAbilities.IndexOf(ability)].transform.parent.gameObject.SetActive(!show);
                }
            }
        }

        private void HideAbilities()
        {
            foreach (var ability in _warmthAbilities)
            {
                var imges = _imagesArray[_warmthAbilities.IndexOf(ability)].Images;
                imges[0].DOSizeDelta(new Vector2(0,0), 0.5f);
                foreach (var img in imges)
                {
                    img.DOLocalMove(new Vector3(0, -300), 0.5f);
                }
            }
        }
        
        private void SelectAbility(int index)
        {
            for(int i = 0;  i < _images.Length; i++)
            {
                if(i != index)CreateOutline(i, false);
            }
            CreateOutline(index, true);
        }

        private void ConfirmAbility(int index)
        {
            var rect = _rhombuses[index].GetComponent<RectTransform>();
            bool notConfirmed = !_rhombuses[index].gameObject.activeSelf;
            var targetSize = notConfirmed ? _confirmedRhombusSize : _defaultRhombusSize;
            var confirmedRhombusSize = rect.sizeDelta.x;
            
            if(notConfirmed) _rhombuses[index].gameObject.SetActive(true);
            
            DOTween.To(() => confirmedRhombusSize, x =>
            {
                rect.sizeDelta = new Vector2(x, x);
                confirmedRhombusSize = x;
                if (Mathf.Approximately(x, targetSize) && !notConfirmed)
                {
                    _rhombuses[index].gameObject.SetActive(false);
                }
            }, targetSize, 0.5f);
            
            var confirmedParent = _images[index].transform.parent.GetComponent<Image>();
            
            var confirmedInlineWidth = confirmedParent.material.GetFloat("_InlineThickness");
            DOTween.To(() => confirmedInlineWidth, x =>{
                confirmedInlineWidth = x;
                confirmedParent.material.SetFloat("_InlineThickness", x);
            }, notConfirmed? _confirmedInWidth : _defaultInWidth, 0.5f);
        }

        private void Cast(List<int> warmthAbilities)
        {
            for (int i = 0; i < _images.Length; i++)
            {
                if (!warmthAbilities.Contains(i))
                {
                    var parent = _images[i].transform.parent.GetComponent<Image>();
                    var opacity = parent.material.GetFloat("_Alpha");
                    DOTween.To(() => opacity, x =>{
                        opacity = x;
                        parent.material.SetFloat("_Alpha", x);
                    }, _activeOpacity, 0.5f);
                    CreateOutline(i, false);
                }
                else
                {
                    var parent = _images[i].transform.parent.GetComponent<Image>();
                    var amplitude = parent.material.GetFloat("_WaveAmplitude");
                    DOTween.To(() => amplitude, x =>{
                        amplitude = x;
                        parent.material.SetFloat("_WaveAmplitude", x);
                    }, _activeAmplitude, 0.5f);
                    CreateOutline(i, true);
                }
            }
        }
        
        private void StopCast(List<int> warmthAbilities)
        {
            foreach (var i in warmthAbilities)
            {
                var parent = _images[i].transform.parent.GetComponent<Image>();
                var amplitude = parent.material.GetFloat("_WaveAmplitude");
                DOTween.To(() => amplitude, x =>{
                    amplitude = x;
                    parent.material.SetFloat("_WaveAmplitude", x);
                }, _defaultAmplitude, 0.5f);
            }

            for (int i = 0; i < _images.Length; i++)
            {
                if (!warmthAbilities.Contains(i))
                {
                    var parent = _images[i].transform.parent.GetComponent<Image>();
                    var opacity = parent.material.GetFloat("_Alpha");
                    DOTween.To(() => opacity, x =>{
                        opacity = x;
                        parent.material.SetFloat("_Alpha", x);
                    }, _defaultOpacity, 0.5f);
                }
            }
        }

        private void CreateOutline(int index, bool selected)
        {
            var selectedParent = _images[index].transform.parent.GetComponent<Image>();
            
            DOTween.To(() => selectedParent.material.GetFloat("_OutlineThickness"), x =>{
                selectedParent.material.SetFloat("_OutlineThickness", x);
            }, selected? _selectedOutWidth : _defaultOutWidth, 0.5f);
            
            DOTween.To(() => selectedParent.material.GetFloat("_InOutlineThickness"), x =>{
                selectedParent.material.SetFloat("_InOutlineThickness", x);
            }, selected? _selectedInOutWidth : _defaultInOutWidth, 0.5f);
            selectedParent.SetMaterialDirty();
        }
        
        private void AddAbility(int index)
        {
            var config = _abilitiesConfigs.Find(x => GetAbilityType(x.Type) == _warmthAbilities[index].GetType());
            _newAbilityUI.Icon.sprite = config.Sprite;
            _newAbilityUI.Name.SetNewKey(config.Name);
            _newAbilityUI.Description.SetNewKey(config.Description);
            _mainObject.DOLocalMoveY(240, 2f);
            _confirmButton.transform.parent.DOLocalMoveY(560, 1.5f);
            _confirmButton.interactable = true;
            EventSystem.current.SetSelectedGameObject(_confirmButton.gameObject);
            HideAbilities();
        }

        private Type GetAbilityType(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Movement: return typeof(PlayerMovement);
                case AbilityType.Interaction: return typeof(InteractionAbility);
                case AbilityType.Warming: return typeof(WarmingAbility);
                case AbilityType.Resonance: return typeof(ResonanceAbility);
                case AbilityType.Metabolism: return typeof(MetabolismAbility);
                case AbilityType.Dash: return typeof(DashAbility);
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
