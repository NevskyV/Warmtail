using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI.SDF
{
    [ExecuteAlways]
    public class SdfGroup : MonoBehaviour
    {
        [Header("Shader Settings")] [SerializeField]
        private Material _baseMaterial;

        private Material _instanceMaterial;
        private List<SdfFigure> _figures = new();
        private Image _image;
        private RectTransform _rectTransform;

        public List<SdfFigure> Figures => _figures;
        public Material InstanceMaterial => _instanceMaterial;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (!TryGetComponent(out _image))
            {
                _image = gameObject.AddComponent<Image>();
            }

            CreateMaterial();
            UpdateFigures();
        }

        private void Update()
        {
            UpdateFigures();
        }

        private void UpdateFigures()
        {
            _figures.Clear();
            _figures.AddRange(GetComponentsInChildren<SdfFigure>());
        }

        private void CreateMaterial()
        {
            if (_baseMaterial == null) return;

            if (_instanceMaterial == null)
            {
#if UNITY_EDITOR
                string folder = "Assets/Resources/Materials/SDF_Groups";
                if (!UnityEditor.AssetDatabase.IsValidFolder(folder))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Materials", "SDF_Groups");
                }

                
                string assetPath = $"{folder}/{gameObject.name}_SDFMaterial.mat";
                _instanceMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (!_instanceMaterial)
                {
                    _instanceMaterial = new Material(_baseMaterial);
                    UnityEditor.AssetDatabase.CreateAsset(_instanceMaterial, assetPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
#endif
            }

            _image.material = _instanceMaterial;
        }
    }
}