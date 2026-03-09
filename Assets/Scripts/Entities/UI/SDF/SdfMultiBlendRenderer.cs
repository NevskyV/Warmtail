using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace Entities.UI.SDF
{ 
    [ExecuteAlways]
    public class SdfGlobalBlendManager : MonoBehaviour
    {
        private List<SdfGroup> _groups = new List<SdfGroup>();
        [SerializeField] private float _groupSmoothness = 0.1f;
        [SerializeField] private int _interGroupType = 0;
        
        [Header("Shader Settings")] [SerializeField]
        public Material BaseMaterial;

        [SerializeField] public Material InstanceMaterial;

        private ComputeBuffer _shapeBuffer;
        private ComputeBuffer _propBuffer;
        private Image _image;

        private void OnEnable()
        {
            if (!TryGetComponent(out _image))
            {
                _image = gameObject.AddComponent<Image>();
            } 
            if(_image.material && !InstanceMaterial) {InstanceMaterial = _image.material;}
            if (_groups.Count >0&& InstanceMaterial) _image.material = InstanceMaterial;
        }

        private void Update()
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            _groups = GetComponentsInChildren<SdfGroup>().ToList();
            List<ShapeData> allShapes = new List<ShapeData>();
            List<GroupProperty> allProps = new List<GroupProperty>();

            int groupIndex = 0;
            foreach (SdfGroup group in _groups)
            {
                if (!group || !group.gameObject.activeInHierarchy || !InstanceMaterial) 
                {
                    groupIndex++;
                    continue;
                }

                RectTransform parentRect = GetComponent<RectTransform>();
                Vector2 parentSize = parentRect ? parentRect.rect.size : Vector2.one;

                foreach (SdfFigure figure in group.Figures)
                {
                    if (!figure) continue;

                    Vector2 normalizedPos = (figure.UseParent? figure.transform.parent.localPosition +  figure.transform.localPosition: figure.transform.localPosition) / parentSize;

                    allShapes.Add(new ShapeData
                    {
                        Type = (int)figure.Type,
                        Position = normalizedPos,
                        Rotation = figure.transform.localEulerAngles.z * Mathf.Deg2Rad,
                        Size = figure.GetComponent<RectTransform>().rect.size / parentRect.rect.size / 2,
                        ParamsA = figure.ShapeData.ParamsA,
                        ParamsB = figure.ShapeData.ParamsB,
                        GroupIndex = groupIndex
                    });
                }
                
                allProps.Add(group.GroupProperty);

                groupIndex++;
            }

            UpdateShapeBuffer(allShapes);
            UpdatePropBuffer(allProps);
            InstanceMaterial.SetBuffer("_AllShapes", _shapeBuffer);
            InstanceMaterial.SetInt("_ShapeCount", allShapes.Count);
            InstanceMaterial.SetBuffer("_GroupProps", _propBuffer);
            InstanceMaterial.SetInt("_GroupCount", allProps.Count);
            InstanceMaterial.SetFloat("_GroupSmoothness", _groupSmoothness);
            InstanceMaterial.SetInt("_MyGroupIndex", groupIndex);
            InstanceMaterial.SetInt("_IsBaseGroup", groupIndex == 0 ? 1 : 0);
            InstanceMaterial.SetFloat("_InterGroupType", _interGroupType);
        }

        private void UpdateShapeBuffer(List<ShapeData> data)
        {
            if (_shapeBuffer != null) _shapeBuffer.Release();

            if (data.Count > 0)
            {
                _shapeBuffer = new ComputeBuffer(data.Count, Marshal.SizeOf(typeof(ShapeData)));
                _shapeBuffer.SetData(data.ToArray());
            }
        }

        private void UpdatePropBuffer(List<GroupProperty> data)
        {
            if (_propBuffer != null) _propBuffer.Release();

            if (data.Count > 0)
            {
                _propBuffer = new ComputeBuffer(data.Count,  Marshal.SizeOf(typeof(GroupProperty)));
                _propBuffer.SetData(data.ToArray());
            }
        }

        private void OnDestroy()
        {
            if (_shapeBuffer != null) _shapeBuffer.Release();
            if (_propBuffer != null) _propBuffer.Release();
        }
        
        [TriInspector.Button]
        public void CreateMaterial(string suffix = "")
        {
            if (BaseMaterial == null) return;
            
#if UNITY_EDITOR
            string folder = "Assets/Resources/Materials/SDF_Groups/";
            if (!UnityEditor.AssetDatabase.IsValidFolder(folder))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Materials", "SDF_Groups");
            }
            folder += suffix;
            if (!UnityEditor.AssetDatabase.IsValidFolder(folder))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Materials/SDF_Groups", suffix);
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
            _image.material = InstanceMaterial;
        }
    }

    [Serializable]
    public struct GroupProperty
    {
        public int InterType;
        public Color FillColor;
        public float Alpha;
        public Color OutlineColor;
        public float OutlineThickness;
        public Color InlineColor;
        public float InlineThickness;
        public float InOutlineThickness;
        public float WaveFreq;
        public float WaveAmp;
        public float WaveSpeed;
        public float Smoothness;
    }
}