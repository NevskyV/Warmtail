using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    [Serializable]public enum Chapter {World, Characters, Puzzles}
    public class ManualVisuals : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<Chapter, List<GameObject>> _pages = new();
        [SerializeField] private SerializedDictionary<Chapter, Button> _buttons = new();
        [SerializeField] private RectTransform _leftSide;
        [SerializeField] private RectTransform _rightSide;
        
        [Inject] private ManualSystem _manualSystem;
        [Inject] private GlobalData _globalData;
        [Inject] private PlayerInput _playerInput;
        [Inject] private UIStateSystem _uiStateSystem;
        private Chapter _currentChapter;
        private GameObject _leftPage, _rightPage;
        private int _currentPageIndex;

        private void Start()
        {
            foreach (var pair in _buttons)
            {
                pair.Value.onClick.AddListener(() => SwitchBookmarks(pair.Key));
            }
            _playerInput.actions["Manual"].performed += _ =>
            {
                if (_globalData.Get<DialogueVarData>().Variables.Find(x => x.Name == "manualOpen").Value == "true")
                {
                    if(_uiStateSystem.CurrentState == UIState.Normal)
                        _uiStateSystem.SwitchCurrentStateAsync(UIState.Manual).Forget();
                    else if (_uiStateSystem.CurrentState == UIState.Manual)
                        _uiStateSystem.SwitchCurrentStateAsync(UIState.Normal).Forget();
                }
            };
            _uiStateSystem.OnStateChange += state =>
            {
                if(state == UIState.Manual) SwitchBookmarks(0);
            };
        }
        
        public void SwitchBookmarks(int chapter) => SwitchBookmarks((Chapter)chapter);

        public void SwitchBookmarks(Chapter chapter)
        {
            for (int i = 0; i < _pages.Keys.Count; ++i)
            {
                _buttons[(Chapter)i].GetComponent<RectTransform>().
                    DOLocalMoveX(i == (int)chapter? -700 : -615, 1);
            }

            _currentPageIndex = 0;
            _currentChapter = chapter;
            LoadPages();
        }

        public void LoadPages(int baseIndex = 0)
        {
            if(_leftPage)Destroy(_leftPage);
            if(_rightPage)Destroy(_rightPage);
            int x = (_currentPageIndex + baseIndex);
            List<int> unlockedPages = _manualSystem.GetUnlockedPages(_currentChapter);
            if (unlockedPages.Count == 0) return;
            _leftPage = Instantiate(_pages[_currentChapter][unlockedPages[x % unlockedPages.Count]], _leftSide);
            _leftPage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            _rightPage = Instantiate(_pages[_currentChapter][unlockedPages[(x+1) % unlockedPages.Count]], _rightSide);
            _rightPage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }
}