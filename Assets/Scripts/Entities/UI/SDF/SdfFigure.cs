using System;
using UnityEngine;

namespace Entities.UI.SDF
{
    public class SdfFigure : MonoBehaviour
    {
        public enum ShapeType { Circle, Rectangle }

        public ShapeType Type;
        public ShapeData ShapeData;
    }

    [Serializable]
    public struct ShapeData
    {
        [HideInInspector] public int Type;
        [HideInInspector] public Vector2 Position;
        [HideInInspector] public float Rotation;
        public Vector2 Size;
        public Vector4 CornerRadius;
        public float AdditionalRounding;
    }
}