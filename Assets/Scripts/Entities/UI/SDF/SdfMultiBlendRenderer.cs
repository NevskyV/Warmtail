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

        private ComputeBuffer _shapeBuffer;
        private ComputeBuffer _propBuffer;
        private Image _image;

        private void OnEnable()
        {
            if (!TryGetComponent(out _image))
            {
                _image = gameObject.AddComponent<Image>();
            } 
            if (!_image.material && _groups.Count >0&& _groups[0].InstanceMaterial) _image.material = _groups[0].InstanceMaterial;
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
                if (!group || !group.gameObject.activeInHierarchy || !group.InstanceMaterial) 
                {
                    groupIndex++;
                    continue;
                }

                RectTransform parentRect = GetComponent<RectTransform>();
                Vector2 parentSize = parentRect ? parentRect.rect.size : Vector2.one;

                foreach (SdfFigure figure in group.Figures)
                {
                    if (!figure) continue;

                    Vector2 normalizedPos = figure.transform.localPosition / parentSize;

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

                Material mat = group.InstanceMaterial;
                
                GroupProperty prop = new GroupProperty
                {
                    InterType = mat.GetInt("_INTERSECTION"),
                    FillColor = mat.GetVector("_FillColor"),
                    Alpha = mat.GetFloat("_Alpha"),
                    OutlineColor = mat.GetVector("_OutlineColor"),
                    OutlineThickness = mat.GetFloat("_OutlineThickness"),
                    InlineColor = mat.GetVector("_InlineColor"),
                    InlineThickness = mat.GetFloat("_InlineThickness"),
                    InOutlineThickness = mat.GetFloat("_InOutlineThickness"),
                    WaveFreq = mat.GetFloat("_WaveFrequency"),
                    WaveAmp = mat.GetFloat("_WaveAmplitude"),
                    WaveSpeed = mat.GetFloat("_WaveSpeed"),
                };

                allProps.Add(prop);

                groupIndex++;
            }

            UpdateShapeBuffer(allShapes);
            UpdatePropBuffer(allProps);

            groupIndex = 0;
            foreach (SdfGroup group in _groups)
            {
                if (!group || !group.gameObject.activeInHierarchy || !group.InstanceMaterial) 
                {
                    groupIndex++;
                    continue;
                }

                group.InstanceMaterial.SetBuffer("_AllShapes", _shapeBuffer);
                group.InstanceMaterial.SetInt("_ShapeCount", allShapes.Count);
                group.InstanceMaterial.SetBuffer("_GroupProps", _propBuffer);
                group.InstanceMaterial.SetInt("_GroupCount", allProps.Count);
                group.InstanceMaterial.SetFloat("_GroupSmoothness", _groupSmoothness);
                group.InstanceMaterial.SetInt("_MyGroupIndex", groupIndex);
                group.InstanceMaterial.SetInt("_IsBaseGroup", groupIndex == 0 ? 1 : 0);
                group.InstanceMaterial.SetFloat("_InterGroupType", _interGroupType);

                groupIndex++;
            }
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
    }

    public struct GroupProperty
    {
        public int InterType;
        public Vector4 FillColor;
        public float Alpha;
        public Vector4 OutlineColor;
        public float OutlineThickness;
        public Vector4 InlineColor;
        public float InlineThickness;
        public float InOutlineThickness;
        public float WaveFreq;
        public float WaveAmp;
        public float WaveSpeed;
    }
}