using System;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace Entities.UI
{
    public class ResourcesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _shells;
        [SerializeField] private TMP_Text _stars;
        [SerializeField] private TMP_Text _addPrefab;
        [SerializeField] private Vector2 _startEndYPos = new Vector2(-78f, 0);
        [SerializeField] private float _transitionDuration = 1f;
        [SerializeField] private float _strength = 5f;
        private GlobalData _globalData;
        private bool _needToAnimate = false;
        
        [Inject]
        public void Construct(GlobalData globalData)
        {
            _globalData = globalData;
            _globalData.SubscribeTo<SavablePlayerData>(UpdateUI);
        }

        private void Start()
        {
            UpdateUI();
            _needToAnimate = true;
        }

        private async void UpdateUI()
        {
            if (!_addPrefab || !_shells || !_stars) return;
            var data = _globalData.Get<SavablePlayerData>();
            if (_needToAnimate)
            {
                if (_shells.text != data.Shells.ToString())
                {
                    var newShells = Instantiate(_addPrefab, _shells.transform);
                    newShells.transform.localPosition = new Vector3(0, _startEndYPos.x, 0);
                    newShells.transform.DOLocalMoveY(_startEndYPos.y, _transitionDuration);
                    DOTween.To(() => newShells.alpha, x => newShells.alpha = x, 0, _transitionDuration);
                    _shells.transform.DOShakePosition(_transitionDuration, _strength);
                }

                if (_stars.text != data.Stars.ToString())
                {
                    var newStars = Instantiate(_addPrefab, _stars.transform);
                    newStars.transform.localPosition = new Vector3(0, _startEndYPos.x, 0);
                    newStars.transform.DOLocalMoveY(_startEndYPos.y, _transitionDuration);
                    DOTween.To(() => newStars.alpha, x => newStars.alpha = x, 0, _transitionDuration);
                    _stars.transform.DOShakePosition(_transitionDuration, _strength);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_transitionDuration) * 0.9f);
            }

            _shells.text = data.Shells.ToString();
            _stars.text = data.Stars.ToString();
        }
    }
}