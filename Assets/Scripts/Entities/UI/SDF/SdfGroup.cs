using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI.SDF
{
    [ExecuteAlways]
    public class SdfGroup : MonoBehaviour
    {
        [Header("Shader Settings")]
        [SerializeField] private Material _baseMaterial;
        //[SerializeField] private float _smoothness = 0.1f;
    
        private Material _instanceMaterial;
        private ComputeBuffer _shapeBuffer;
        private List<SdfFigure> _figures = new();
        private Image _image;
        private RectTransform _parentRect;

        private void OnEnable()
        {
            if (TryGetComponent(out Image image))
            {
                _image = image;
            }
            else{
                _image = gameObject.AddComponent<Image>();
            }
            _parentRect = GetComponent<RectTransform>();
            CreateMaterial();
            UpdateFigures();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void Update()
        {
            if (transform.childCount != _figures.Count || NeedUpdate())
            {
                UpdateFigures();
            }
        }

        private bool NeedUpdate()
        {
            foreach (var fig in _figures)
            {
                if (!fig) return true;
            }
            return false;
        }

        private void UpdateFigures()
        {
            _figures = new List<SdfFigure>(transform.GetComponentsInChildren<SdfFigure>());
            UpdateBuffer();
        }

        private void UpdateBuffer()
        {
            if (_figures.Count == 0)
            {
                SetBufferCount(0);
                return;
            }

            ShapeData[] data = new ShapeData[_figures.Count];
            
            Vector2 parentSize = _parentRect.sizeDelta;
            
            for (int i = 0; i < _figures.Count; i++)
            {
                var f = _figures[i];
                var localPos = f.transform.localPosition / parentSize;;
                f.ShapeData.Type = (int)f.Type;
                f.ShapeData.Position = localPos;
                f.ShapeData.Rotation = f.transform.localEulerAngles.z * Mathf.Deg2Rad;
                data[i] = f.ShapeData;
            }

            if (_shapeBuffer != null) _shapeBuffer.Release();
            _shapeBuffer = new ComputeBuffer(data.Length, sizeof(float) * 10 + sizeof(int)); // примерно
            _shapeBuffer.SetData(data);

            if (_instanceMaterial)
            {
                SetBufferCount(data.Length);
                _instanceMaterial.SetBuffer("_Shapes", _shapeBuffer);
                //_instanceMaterial.SetFloat("_Smoothness", _smoothness);
            }
        }

        private void CreateMaterial()
        {
            if (_baseMaterial != null && _instanceMaterial == null)
            {
                _instanceMaterial = new Material(_baseMaterial);
                _image.material = _instanceMaterial;
            }
        }

        private void SetBufferCount(int count)
        {
            if (_instanceMaterial)
            {
                _instanceMaterial.SetInt("_ShapeCount", count);
            }
        }

        private void Cleanup()
        {
            if (_shapeBuffer != null)
            {
                _shapeBuffer.Release();
                _shapeBuffer = null;
            }
            if (_instanceMaterial != null)
            {
                DestroyImmediate(_instanceMaterial);
                _instanceMaterial = null;
            }
        }

        private void OnDestroy()
        {
            Cleanup();
        }
    }
}