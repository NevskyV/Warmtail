using System;
using System.Collections.Generic;
using System.Linq;
using Data.Player;
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Systems.Abilities;
using TriInspector;
using UnityEngine;
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
        
        private List<WarmthAbility> _warmthAbilities;
        private int _selectedIndex;
        private PlayerConfig _playerConfig;
        private AbilitiesSystem _abilitiesSystem;

        [Inject]
        private void Construct(PlayerConfig playerConfig, AbilitiesSystem abilitiesSystem)
        {
            _playerConfig = playerConfig;
            _abilitiesSystem =  abilitiesSystem;
            _warmthAbilities = _playerConfig.Abilities.OfType<WarmthAbility>().ToList();
            ShowAbilities();
            
            _abilitiesSystem.OnSelect += SelectAbility;
            _abilitiesSystem.OnConfirm += ConfirmAbility;
            _abilitiesSystem.OnCast += Cast;
            _abilitiesSystem.OnStopCast += StopCast;
            _abilitiesSystem.OnAddAbility += AddAbility;
        }
        
        private void OnDisable()
        {
            _abilitiesSystem.OnSelect -= SelectAbility;
            _abilitiesSystem.OnConfirm -= ConfirmAbility;
            _abilitiesSystem.OnCast -= Cast;
            _abilitiesSystem.OnStopCast -= StopCast;
            _abilitiesSystem.OnAddAbility -= AddAbility;

            foreach (var image in _images)
            {
                image.material.SetFloat("_OutlineThickness",0);
                image.material.SetFloat("_InOutlineThickness",0);
                image.material.SetFloat("_InlineThickness",0);
                image.material.SetFloat("_WaveAmplitude",0);
                image.material.SetFloat("_Alpha",1);
            }
        }

        private void ShowAbilities(bool show = true)
        {
            int index = 0;
            var size = _images[0].GetComponent<RectTransform>().sizeDelta.x;
            var inUse = _warmthAbilities.Where(x => x.InUse).ToList();
            _warmthAbilities.Where(x => !x.InUse).ForEach( x =>
                _images[_warmthAbilities.IndexOf(x)].transform.parent.gameObject.SetActive(!show));
            var maxDist = inUse.Count * size + _distance * (inUse.Count - 1);
            
            foreach (var ability in inUse)
            {
                var imges = _imagesArray[_warmthAbilities.IndexOf(ability)].Images;
                imges[2].GetComponent<Image>().sprite = ability.Visual.Icon;
                imges[2].transform.parent.gameObject.SetActive(show);
                foreach (var img in imges)
                {
                    img.DOLocalMoveX(-(maxDist - size) / 2 + (_distance + size) * index, 0.5f);
                }
                index++;
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
            bool notConfirmed = rect.sizeDelta == new Vector2(_defaultRhombusSize, _defaultRhombusSize);
            var confirmedRhombusSize = rect.sizeDelta.x;
            DOTween.To(() => confirmedRhombusSize, x =>
            {
                rect.sizeDelta = new Vector2(x, x);
                confirmedRhombusSize = x;
            }, notConfirmed? _confirmedRhombusSize : _defaultRhombusSize, 0.5f);
            
            var confirmedParent = _images[index].transform.parent.GetComponent<Image>();
            
            var confirmedInlineWidth = confirmedParent.materialForRendering.GetFloat("_InlineThickness");
            DOTween.To(() => confirmedInlineWidth, x =>{
                confirmedInlineWidth = x;
                confirmedParent.materialForRendering.SetFloat("_InlineThickness", x);
            }, notConfirmed? _confirmedInWidth : _defaultInWidth, 0.5f);
        }

        private void Cast(List<int> warmthAbilities)
        {
            for (int i = 0; i < _images.Length; i++)
            {
                if (!warmthAbilities.Contains(i))
                {
                    var parent = _images[i].transform.parent.GetComponent<Image>();
                    var opacity = parent.materialForRendering.GetFloat("_Alpha");
                    DOTween.To(() => opacity, x =>{
                        opacity = x;
                        parent.materialForRendering.SetFloat("_Alpha", x);
                    }, _activeOpacity, 0.5f);
                    CreateOutline(i, false);
                }
                else
                {
                    var parent = _images[i].transform.parent.GetComponent<Image>();
                    var amplitude = parent.materialForRendering.GetFloat("_WaveAmplitude");
                    DOTween.To(() => amplitude, x =>{
                        amplitude = x;
                        parent.materialForRendering.SetFloat("_WaveAmplitude", x);
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
                var amplitude = parent.materialForRendering.GetFloat("_WaveAmplitude");
                DOTween.To(() => amplitude, x =>{
                    amplitude = x;
                    parent.materialForRendering.SetFloat("_WaveAmplitude", x);
                }, _defaultAmplitude, 0.5f);
            }

            for (int i = 0; i < _images.Length; i++)
            {
                if (!warmthAbilities.Contains(i))
                {
                    var parent = _images[i].transform.parent.GetComponent<Image>();
                    var opacity = parent.materialForRendering.GetFloat("_Alpha");
                    DOTween.To(() => opacity, x =>{
                        opacity = x;
                        parent.materialForRendering.SetFloat("_Alpha", x);
                    }, _defaultOpacity, 0.5f);
                }
            }
        }

        private void CreateOutline(int index, bool selected)
        {
            var selectedParent = _images[index].transform.parent.GetComponent<Image>();
            
            DOTween.To(() => selectedParent.materialForRendering.GetFloat("_OutlineThickness"), x =>{
                selectedParent.materialForRendering.SetFloat("_OutlineThickness", x);
            }, selected? _selectedOutWidth : _defaultOutWidth, 0.5f);
            
            DOTween.To(() => selectedParent.material.GetFloat("_InOutlineThickness"), x =>{
                selectedParent.materialForRendering.SetFloat("_InOutlineThickness", x);
            }, selected? _selectedInOutWidth : _defaultInOutWidth, 0.5f);
            selectedParent.SetMaterialDirty();
        }
        
        private void AddAbility(int index)
        {
            ShowAbilities();
        }
    }
}
