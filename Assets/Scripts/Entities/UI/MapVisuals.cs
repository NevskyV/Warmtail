using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Entities.PlayerScripts;
using TriInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Entities.UI
{
    public class MapVisuals : MonoBehaviour
    {
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        
        [Title("Components")]
        [SerializeField] private RectTransform _rectMap;
        [SerializeField] private Image[] _shardLocations;
        
        [Title("Parameters")]
        [SerializeField] private float _transitionDuration;
        [Tooltip("Pixels per second")]
        [SerializeField] private float _moveSpeed = 1000f;
        [SerializeField] private float _dragFactor = 2f;
        [SerializeField] private float _smoothTime = 0.12f;
        [SerializeField] private float _elasticity = 8f;

        [SerializeField, MinMaxSlider(0, 10)]
        private Vector2 _zoomRange = new (1f, 3f);

        private GlobalData _globalData;
        private PlayerInput _playerInput;
        private UIStateSystem _uiStateSystem;
        private Player _player;

        private Vector2 _targetPos;
        private Vector2 _velocity;
        private float _targetScale = 1f;
        private float _scaleVelocity;
        
        private bool _dragging;
        private Vector2 _lastMouse;

        [Inject]
        public void Construct(GlobalData globalData, PlayerInput playerInput, UIStateSystem uiStateSystem)
        {
            _globalData = globalData;
            _playerInput = playerInput;
            _uiStateSystem = uiStateSystem;

            _playerInput.actions["Map"].performed += ChangeMapVisibility;
            _playerInput.actions["Click"].performed += StartDrag;
            _playerInput.actions["Click"].canceled += EndDrag;
            _playerInput.actions["Scroll"].performed += ScaleMap;

            _targetPos = _rectMap.anchoredPosition;
            _targetScale = _rectMap.localScale.x;

            for (int i = 0; i < _shardLocations.Length; i++)
            {
                _shardLocations[i].material.SetFloat(DissolveAmount,
                    _globalData.Get<WorldData>().CollectedStars.Contains(i) ? 1 : 0);
            }
        }
        

        private void Update()
        {
            Vector2 moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();
            if (moveInput.sqrMagnitude > 0.0001f)
            {
                _targetPos += moveInput * (_moveSpeed * Time.deltaTime);
            }

            UpdatePosition();
            UpdateScale();
        }

        private void ChangeMapVisibility(InputAction.CallbackContext ctx)
        {
            if (_uiStateSystem.CurrentState == UIState.Map)
                _uiStateSystem.SwitchCurrentStateAsync(UIState.Normal).Forget();
            else
                _uiStateSystem.SwitchCurrentStateAsync(UIState.Map).Forget();
        }

        private void StartDrag(InputAction.CallbackContext ctx)
        {
            _dragging = true;
            _lastMouse = Mouse.current.position.ReadValue();
        }

        private void EndDrag(InputAction.CallbackContext ctx)
        {
            _dragging = false;
        }
        
        private void LateUpdate()
        {
            if (!_dragging) return;

            Vector2 mouse = Mouse.current.position.ReadValue();
            Vector2 delta = (mouse - _lastMouse) *  _dragFactor;
            _lastMouse = mouse;

            float scale = Mathf.Max(0.0001f, _rectMap.localScale.x);
            RectTransform parent = _rectMap.parent as RectTransform;
            Vector2 limit = GetBounds();

            Vector2 newTarget = _targetPos - delta / scale;

            if (parent)
            {
                Vector2 parentSize = parent.rect.size;
                if (Mathf.Abs(newTarget.x) > limit.x)
                {
                    float over = newTarget.x - Mathf.Sign(newTarget.x) * limit.x;
                    float rubber = RubberDelta(over, parentSize.x);
                    newTarget.x = Mathf.Sign(newTarget.x) * (limit.x + rubber);
                }
                if (Mathf.Abs(newTarget.y) > limit.y)
                {
                    float over = newTarget.y - Mathf.Sign(newTarget.y) * limit.y;
                    float rubber = RubberDelta(over, parentSize.y);
                    newTarget.y = Mathf.Sign(newTarget.y) * (limit.y + rubber);
                }
            }

            _targetPos = newTarget;
        }
        
        private void ScaleMap(InputAction.CallbackContext ctx)
        {
            float scroll = ctx.ReadValue<Vector2>().y;
            if (Mathf.Abs(scroll) < 0.0001f) return;
            
            const float zoomStep = 0.1f;
            _targetScale = Mathf.Clamp(_targetScale + scroll * zoomStep, _zoomRange.x, _zoomRange.y);
        }

        private void UpdatePosition()
        {
            Vector2 pos = _rectMap.anchoredPosition;
            Vector2 limit = GetBounds();

            Vector2 clampedTarget = new Vector2(
                Mathf.Clamp(_targetPos.x, -limit.x, limit.x),
                Mathf.Clamp(_targetPos.y, -limit.y, limit.y)
            );

            Vector2 smoothTarget = _dragging ? _targetPos: clampedTarget;

            pos = Vector2.SmoothDamp(pos, -smoothTarget, ref _velocity, _smoothTime);

            _rectMap.anchoredPosition = pos;
        }
        
        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (1f - 1f / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1f)) * viewSize * Mathf.Sign(overStretching);
        }

        private void UpdateScale()
        {
            float current = _rectMap.localScale.x;
            float smooth = Mathf.SmoothDamp(current, _targetScale, ref _scaleVelocity, _smoothTime);
            _rectMap.localScale = Vector3.one * smooth;
        }
        
        private Vector2 GetBounds()
        {
            var parent = _rectMap.parent as RectTransform;
            if (!parent) return Vector2.zero;

            Vector2 mapSize = Vector2.Scale(_rectMap.rect.size, new Vector2(_rectMap.localScale.x, _rectMap.localScale.y));
            Vector2 parentSize = parent.rect.size;
            
            Vector2 diff = (mapSize - parentSize) / 2f;
            diff.x = Mathf.Max(0f, diff.x);
            diff.y = Mathf.Max(0f, diff.y);
            return diff;
        }

        public async void OpenLocation(int index)
        {
            if (_globalData.Get<WorldData>().CollectedStars.Contains(index)) return;

            await _uiStateSystem.SwitchCurrentStateAsync(UIState.Map);
            await _shardLocations[index].material.DOFloat(1, DissolveAmount, _transitionDuration).AsyncWaitForCompletion();
            _globalData.Edit<WorldData>(x => x.CollectedStars.Add(index));
            _uiStateSystem.SwitchCurrentStateAsync(UIState.Normal).Forget();
        }
    }
}