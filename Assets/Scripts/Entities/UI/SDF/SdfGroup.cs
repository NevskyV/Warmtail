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
            _instanceMaterial = _image.material;
            CreateMaterial();
            UpdateFigures();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void Update()
        {
            UpdateFigures();
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
                f.ShapeData.Size = f.GetComponent<RectTransform>().rect.size / parentSize / 2;
                data[i] = f.ShapeData;
            }

            if (_shapeBuffer != null) _shapeBuffer.Release();
            _shapeBuffer = new ComputeBuffer(data.Length, sizeof(float) * 13 + sizeof(int));
            _shapeBuffer.SetData(data);

            if (_instanceMaterial)
            {
                SetBufferCount(data.Length);
                _instanceMaterial.SetBuffer("_Shapes", _shapeBuffer);
            }
        }

        private void CreateMaterial()
        {
            if (_baseMaterial != null && _instanceMaterial == null)
            {
                _instanceMaterial = new Material(_baseMaterial);
                
#if UNITY_EDITOR
                string path = UnityEditor.AssetDatabase.GetAssetPath(this);
                if (string.IsNullOrEmpty(path))
                {
                    string folder = "Assets/Resources/Materials/SDF_Groups";
                    if (!UnityEditor.AssetDatabase.IsValidFolder(folder))
                        UnityEditor.AssetDatabase.CreateFolder("Assets/Materials", "SDF_Groups");

                    string assetPath = $"{folder}/{gameObject.name}_SDFMaterial.mat";
                    UnityEditor.AssetDatabase.CreateAsset(_instanceMaterial, assetPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    Debug.Log($"Создан и сохранён материал: {assetPath}", _instanceMaterial);
                }
#endif
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