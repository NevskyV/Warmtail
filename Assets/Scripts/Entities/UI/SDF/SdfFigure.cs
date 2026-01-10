using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Entities.UI.SDF
{
    public class SdfFigure : MonoBehaviour
    {
        public enum ShapeType
        {
            Circle,
            ChamferBox,
            Box,
            Segment,
            Rhombus,
            Trapezoid,
            Parallelogram,
            EquilateralTriangle,
            TriangleIsosceles,
            Triangle,
            UnevenCapsule,
            Pentagon,
            Hexagon,
            Octogon,
            Hexagram,
            Star5,
            Pie,
            CutDisk,
            Arc,
            Cross,
            RoundedCross,
            //RoundedX,
            //Heart
        }
        
        public static readonly Dictionary<ShapeType, string> ParameterDescriptions = new()
        {
            { ShapeType.Circle, "size.x = radius" },
            { ShapeType.Box, "size = half extents\nparamsA = rounding (x=bottomLeft, y=topLeft, z=topRight, w=bottomRight)\nparamsB.x = allRounding" },
            { ShapeType.ChamferBox, "size = half extents\nparamsA.x = chamfer size" },
            { ShapeType.Segment, "paramsA.xy = start point\nparamsA.zw = end point" },
            { ShapeType.Rhombus, "size = half extents" },
            { ShapeType.Trapezoid, "paramsA.x = bottom base\nparamsA.y = top base\nparamsA.z = height" },
            { ShapeType.Parallelogram, "size.x = width\nsize.y = height\nparamsA.x = skew" },
            { ShapeType.EquilateralTriangle, "size.x = radius" },
            { ShapeType.TriangleIsosceles, "paramsA.xy = base & height vector" },
            { ShapeType.Triangle, "paramsA.xy = p0\nparamsA.zw = p1\nparamsB.xy = p2" },
            { ShapeType.UnevenCapsule, "paramsA.x = radius bottom\nparamsA.y = radius top\nparamsA.z = height" },
            { ShapeType.Pentagon, "size.x = radius" },
            { ShapeType.Hexagon, "size.x = radius" },
            { ShapeType.Octogon, "size.x = radius" },
            { ShapeType.Hexagram, "size.x = radius" },
            { ShapeType.Star5, "size.x = radius" },
            { ShapeType.Pie, "size.x = radius\nparamsA.x = angle (radians)" },
            { ShapeType.CutDisk, "size.x = radius\nparamsA.x = cut height" },
            { ShapeType.Arc, "size.x = radius\nsize.y = thickness\nparamsA.x = angle (radians)" },
            { ShapeType.Cross, "size = arm half sizes (x=horizontal arm, y=vertical arm)\nparamsA.x = rounding" },
            { ShapeType.RoundedCross, "size.x = arm size" },
            //{ ShapeType.RoundedX, "size.x = width\nparamsA.x = rounding" },
            //{ ShapeType.Heart, "No params, scale via size.x" }
        };

        [field: SerializeField,InfoBox("$" + nameof(SType))]
        public ShapeType Type { get; private set; }
        public string SType => ParameterDescriptions[Type];
        public ShapeData ShapeData;
    }

    [Serializable]
    public struct ShapeData
    {
        public int Type;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Size;
        public Vector4 ParamsA;
        public Vector4 ParamsB;
    }
}