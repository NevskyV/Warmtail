using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public enum Chapter {World, Characters, Puzzles}
    public class ManualVisuals : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<Chapter, List<GameObject>> _pages = new();
        [SerializeField] private SerializedDictionary<Chapter, Button> _buttons = new();
        [SerializeField] private RectTransform _leftSide;
        [SerializeField] private RectTransform _rightSide;
        
        [Inject] private GlobalData _globalData;
        private Chapter _currentChapter;
        private GameObject _leftPage, _rightPage;
        private int _currentPageIndex;

        private void Start()
        {
            foreach (var pair in _buttons)
            {
                pair.Value.onClick.AddListener(() => SwitchBookmarks(pair.Key));
            }
        }

        private void SwitchBookmarks(Chapter chapter)
        {
            for (int i = 0; i < _pages.Keys.Count; ++i)
            {
                _buttons[(Chapter)i].GetComponent<RectTransform>().
                    DOLocalMoveX(i == (int)chapter? -615: -700, 1);
            }

            _currentPageIndex = 0;
            _currentChapter = chapter;
            LoadPages();
        }

        public void LoadPages(int baseIndex = 0)
        {
            if(_leftPage)Destroy(_leftPage);
            if(_rightPage)Destroy(_rightPage);
            int x = _currentPageIndex + baseIndex;
            if(_pages[_currentChapter].Count < x)
                _leftPage = Instantiate(_pages[_currentChapter][x], _leftSide);
            if(_pages[_currentChapter].Count < x + 1)
                _rightPage = Instantiate(_pages[_currentChapter][x + 1], _rightSide);
        }
    }
}