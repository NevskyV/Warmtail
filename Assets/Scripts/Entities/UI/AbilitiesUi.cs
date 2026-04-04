using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Player;
using DG.Tweening;
using EasyTextEffects;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Entities.Localization;
using Entities.UI.SDF;
using Systems.Abilities;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        [SerializeField] private float _transitionDuration = 0.3f;
        
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
        [SerializeField] private float _disableOpacity;
        [Title("New ability")] 
        [SerializeField] private Transform _mainObject;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private InputActionReference _confirmAction;
        [SerializeField] private InputActionReference _useAction;
        [SerializeField] private NewAbilityUI _newAbilityUI; 
        
        private List<WarmthAbility> _warmthAbilities;
        private PlayerConfig _playerConfig;
        private AbilitiesSystem _abilitiesSystem;
        private TipsVisuals _tipsVisuals;
        private GlobalData _globalData;
        private bool _isCasting;

        [Inject]
        private void Construct(PlayerConfig playerConfig, AbilitiesSystem abilitiesSystem, TipsVisuals tipsVisuals, 
            GlobalData globalData)
        {
            _playerConfig = playerConfig;
            _abilitiesSystem =  abilitiesSystem;
            _tipsVisuals = tipsVisuals;
            _globalData = globalData;
            _warmthAbilities = _playerConfig.Abilities.OfType<WarmthAbility>().ToList();
            
            _abilitiesSystem.OnSelect += SelectAbility;
            _abilitiesSystem.OnConfirm += ConfirmAbility;
            _abilitiesSystem.OnCast += Cast;
            _abilitiesSystem.OnStopCast += StopCast;
            _abilitiesSystem.OnAddAbility += AddAbility;
            
            _globalData.SubscribeTo<RuntimePlayerData>(() => HasCells());
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
                var parent = image.transform.parent.GetComponent<SdfGroup>();
                parent.GroupProperty.OutlineThickness = 0;
                parent.GroupProperty.InOutlineThickness = 0;
                parent.GroupProperty.InlineThickness = 0;
                parent.GroupProperty.WaveAmp = 0;
                parent.GroupProperty.Alpha = 1;
            }
        }

        private void ShowAbilities(bool show = true)
        {
            _confirmButton.interactable = false;
            _mainObject.DOLocalMoveY(-300, 2f);
            _confirmButton.transform.DOLocalMoveY(-300, 1.5f);
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
                    imges[0].DOSizeDelta(new Vector2(100,100), _transitionDuration);
                    foreach (var img in imges)
                    {
                        img.DOLocalMove(new Vector3(-(maxDist - size) / 2 + (_distance + size) * index, 100), _transitionDuration);
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
                imges[0].DOSizeDelta(new Vector2(0,0), _transitionDuration);
                foreach (var img in imges)
                {
                    img.DOLocalMove(new Vector3(0, -300), _transitionDuration);
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
            }, targetSize, _transitionDuration);
            
            var confirmedParent = _images[index].transform.parent.GetComponent<SdfGroup>();
            
            var confirmedInlineWidth = confirmedParent.GroupProperty.InlineThickness;
            DOTween.To(() => confirmedInlineWidth, x =>{
                confirmedInlineWidth = x;
                confirmedParent.GroupProperty.InlineThickness = x;
            }, notConfirmed? _confirmedInWidth : _defaultInWidth, _transitionDuration);
        }

        private void Cast(List<int> warmthAbilities)
        {
            _isCasting = true;
            if (!HasCells()) return;
            SetAbilitiesGrey(warmthAbilities, _activeOpacity, _activeAmplitude);
            SetAbilitiesOutline(warmthAbilities);
        }
        
        private void StopCast(List<int> warmthAbilities)
        {
            _isCasting = false;
            if (!HasCells()) return;
            SetAbilitiesGrey(warmthAbilities, _defaultOpacity, _defaultAmplitude);
            SetAbilitiesOutline(warmthAbilities);
        }

        private void CreateOutline(int index, bool selected)
        {
            var selectedParent = _images[index].transform.parent.GetComponent<SdfGroup>();
            
            DOTween.To(() => selectedParent.GroupProperty.OutlineThickness, x =>{
                selectedParent.GroupProperty.OutlineThickness = x;
            }, selected? _selectedOutWidth : _defaultOutWidth, _transitionDuration);
            
            DOTween.To(() => selectedParent.GroupProperty.InOutlineThickness, x =>{
                selectedParent.GroupProperty.InOutlineThickness = x;
            }, selected? _selectedInOutWidth : _defaultInOutWidth, _transitionDuration);
            selectedParent.transform.parent.GetComponent<Image>().SetMaterialDirty();
        }
        
        private void AddAbility(int index)
        {
            var config = _abilitiesConfigs.Find(x => GetAbilityType(x.Type) == _warmthAbilities[index].GetType());
            _newAbilityUI.Icon.sprite = config.Sprite;
            _newAbilityUI.Name.SetNewKey(config.Name);
            _newAbilityUI.Description.SetNewKey(config.Description);
            _mainObject.DOLocalMoveY(240, 2f);
            _confirmButton.transform.DOLocalMoveY(560, 1.5f);
            _confirmButton.interactable = true;
            EventSystem.current.SetSelectedGameObject(_confirmButton.gameObject);
            
            _newAbilityUI.Name.GetComponent<TextEffect>().Refresh();
            _tipsVisuals?.ShowTip(_confirmAction);
            _tipsVisuals?.ShowTip(_useAction);
            HideAbilities();
        }

        private bool HasCells()
        {
            if (_globalData.Get<RuntimePlayerData>().CurrentCells == 0)
            {
                SetAbilitiesGrey(new(), _disableOpacity, _activeAmplitude);
                return false;
            }
            if(!_isCasting)SetAbilitiesGrey(new(), _defaultOpacity, _defaultAmplitude);
            return true;
            
        }

        private void SetAbilitiesGrey(List<int> warmthAbilities, float endOpacity, float endAmplitude)
        {
            for (int i = 0; i < _images.Length; i++)
            {
                if (!_images[i]) return;
                var parent = _images[i].transform.parent.GetComponent<SdfGroup>();
                if (!warmthAbilities.Contains(i))
                {
                    var opacity = parent.GroupProperty.Alpha;
                    DOTween.To(() => opacity, x =>{
                        opacity = x;
                        parent.GroupProperty.Alpha = x;
                    }, endOpacity, _transitionDuration);
                }
                else
                {
                    var amplitude = parent.GroupProperty.WaveAmp;
                    DOTween.To(() => amplitude, x =>{
                        amplitude = x;
                        parent.GroupProperty.WaveAmp = x;
                    }, endAmplitude, _transitionDuration);
                }
            }
        }

        private void SetAbilitiesOutline(List<int> warmthAbilities)
        {
            for (int i = 0; i < _images.Length; i++)
            {
                if (!warmthAbilities.Contains(i))
                {
                    CreateOutline(i, false);
                }
                else
                {

                    CreateOutline(i, true);
                }
            }
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
