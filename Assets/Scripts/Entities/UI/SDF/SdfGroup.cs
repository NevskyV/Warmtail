using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.UI.SDF
{
    [ExecuteAlways]
    public class SdfGroup : MonoBehaviour
    {
        public GroupProperty GroupProperty;
        public Material InstanceMaterial;
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

        [Button]
        private void ParseMaterial()
        {
            if (InstanceMaterial == null) return;
            
            GroupProperty prop = new GroupProperty
            {
                InterType = InstanceMaterial.GetInt("_INTERSECTION"),
                FillColor = InstanceMaterial.GetVector("_FillColor"),
                Alpha = InstanceMaterial.GetFloat("_Alpha"),
                OutlineColor = InstanceMaterial.GetVector("_OutlineColor"),
                OutlineThickness = InstanceMaterial.GetFloat("_OutlineThickness"),
                InlineColor = InstanceMaterial.GetVector("_InlineColor"),
                InlineThickness = InstanceMaterial.GetFloat("_InlineThickness"),
                InOutlineThickness = InstanceMaterial.GetFloat("_InOutlineThickness"),
                WaveFreq = InstanceMaterial.GetFloat("_WaveFrequency"),
                WaveAmp = InstanceMaterial.GetFloat("_WaveAmplitude"),
                WaveSpeed = InstanceMaterial.GetFloat("_WaveSpeed"),
            };
            GroupProperty = prop;
        }
    }
}