using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.UI.SDF
{
    [ExecuteAlways]
    public class SdfGroup : MonoBehaviour
    {
        [Header("Shader Settings"),  FormerlySerializedAs("_baseMaterial")] [SerializeField]
        public Material BaseMaterial;

        [SerializeField, FormerlySerializedAs("_instanceMaterial")] public Material InstanceMaterial;
        private List<SdfFigure> _figures = new();


        public List<SdfFigure> Figures => _figures;

        private void Start()
        {
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

        [TriInspector.Button]
        public void CreateMaterial(string suffix = "")
        {
            if (BaseMaterial == null) return;

            if (InstanceMaterial == null)
            {
#if UNITY_EDITOR
                string folder = "Assets/Resources/Materials/SDF_Groups/" + suffix;
                if (!UnityEditor.AssetDatabase.IsValidFolder(folder))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Materials", "SDF_Groups");
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Materials/SDF_Groups",suffix);
                }

                
                string assetPath = $"{folder}/{gameObject.name}_SDFMaterial.mat";
                InstanceMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (!InstanceMaterial)
                {
                    InstanceMaterial = new Material(BaseMaterial);
                    UnityEditor.AssetDatabase.CreateAsset(InstanceMaterial, assetPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
#endif
            }
        }
    }
}