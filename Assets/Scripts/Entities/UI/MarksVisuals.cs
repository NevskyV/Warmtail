using System;
using System.Collections.Generic;
using Data;
using Systems;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class MarksVisuals : MonoBehaviour
    {
        [Serializable]
        public class Mark
        {
            public Sprite Sprite;
            [HideInInspector] public Vector2 WorldPosition;
        }

        [Serializable]
        public class DynamicMark : Mark
        {
            public Transform Target;
        }
        
        [SerializeField] private Image _hudMarkPrefab;
        [SerializeField] private RectTransform _hudMarkParent;
        [SerializeField] private Image _mapMarkPrefab;
        [SerializeField] private RectTransform _mapMarkParent;
        [SerializeField] private SpriteRenderer _locationSpriteRenderer;
        [SerializeField] private List<DynamicMark> _dynamicMarks = new();
        
        [SerializeField, Tooltip("x: horizontal\ny: top\nz:bottom")] private Vector3 _markOffset;
        [SerializeField, PreviewObject] private Sprite _hudSprite;
        private Camera _camera;
        
        private Dictionary<Mark, GameObject> _hudMarks = new();
        private Dictionary<Mark, GameObject> _mapMarks = new();

        [Inject] private PlayerInput _playerInput;

        private void Start()
        {
            _camera = Camera.main;
            foreach (var mark in _dynamicMarks)
            {
                SpawnMark(mark, false);
            }

            _playerInput.actions["Map"].performed += _ =>
            {
                foreach (var mark in _dynamicMarks)
                {
                    mark.WorldPosition = mark.Target.position;
                    CalculateRectPosition(mark);
                }
            };

            SpawnMark(new Mark(){Sprite = _dynamicMarks[0].Sprite, WorldPosition = new Vector2(50, 10)}, true);
        }
        
        public void SpawnMark(Mark mark, bool inHud)
        {
            var mapMark = Instantiate(_mapMarkPrefab, _mapMarkParent);
            mapMark.sprite = mark.Sprite;
            _mapMarks.TryAdd(mark, mapMark.gameObject);
            CalculateRectPosition(mark);
            
            if (!inHud) return;
            var hudMark = Instantiate(_hudMarkPrefab, _hudMarkParent);
            hudMark.sprite = _hudSprite;
            _hudMarks.TryAdd(mark, hudMark.gameObject);
        }

        private void Update()
        {
            CalculateMarksPositions();
        }
        
        private void CalculateRectPosition(Mark mark)
        {
            var mapMark = _mapMarks[mark];
            
            mapMark.GetComponent<RectTransform>().anchoredPosition = 
                (mark.WorldPosition - (_locationSpriteRenderer.transform.position.ToVector2() - _locationSpriteRenderer.bounds.size.ToVector2() / 2))
                    / _locationSpriteRenderer.bounds.size * _mapMarkParent.rect.size;
        }
        
        private void CalculateMarksPositions()
        {
            foreach(var (mark, obj) in _hudMarks)
            {
                var screenPos = _camera.WorldToScreenPoint(mark.WorldPosition);

                Vector2 newScreenPos = new Vector2(screenPos.x, screenPos.y);
                if (screenPos.x > Screen.width - Screen.width * _markOffset.x)
                {
                    newScreenPos.x = Screen.width - Screen.width * _markOffset.x;
                }
                else if (screenPos.x < Screen.width * _markOffset.x)
                {
                    newScreenPos.x = Screen.width * _markOffset.x;
                }
                
                if (screenPos.y > Screen.height - Screen.height * _markOffset.y)
                {
                    newScreenPos.y = Screen.height - Screen.height * _markOffset.y;
                }
                else if (screenPos.y < Screen.height * _markOffset.z)
                {
                    newScreenPos.y = Screen.height * _markOffset.z;
                }
                Vector2 toTarget = (Vector2)screenPos - newScreenPos;
                if (toTarget.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
                    obj.transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90);
                }
                obj.transform.position = newScreenPos;
            }
        }
        
        public void DestroyMark(Mark mark)
        {
            if(_mapMarks.ContainsKey(mark)) Destroy(_mapMarks[mark]);
            if(_hudMarks.ContainsKey(mark)) Destroy(_hudMarks[mark]);
        }
    }
}