using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities.UI
{
    [AddComponentMenu("Layout/Curved Radial Layout Group", 150)]
    [ExecuteAlways]

    public class CurvedLayoutGroup : LayoutGroup
    {

        [Header("General Settings")]

        public bool IgnoreInactive;

        public float RotateOffset;
        [Header("Animation Curve Settings")]
        public bool UseAnimationCurve;
        public AnimationCurve Curve;
        public float CurveScale = 1;

        [Header("Radial Settings")]
        public bool Radial = false;
        public float StartAngle = 0;
        public float EndAngle = 360;

        public bool RotateTowards = false;
        public float Radius = 5f;

        [Header("Curving Settings")]
        public bool Horizontal;
        public float Width;
        public float Height;


        protected override void OnEnable() { base.OnEnable(); CalcAlongCurve(); }
        public override void SetLayoutHorizontal()
        {
        }
        public override void SetLayoutVertical()
        {
        }
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongCurve();
        }
        public override void CalculateLayoutInputHorizontal()
        {
            CalcAlongCurve();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CalcAlongCurve();
        }
#endif

        void CalcAlongCurve()
        {
            // tracker locks properties in the inspector
            m_Tracker.Clear();
            // if theres no children, do nothing
            if (transform.childCount == 0)
                return;

            // grab all children
            List<RectTransform> childRects = new List<RectTransform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = (RectTransform)transform.GetChild(i);
                // ignore inactive gameobjects
                if (IgnoreInactive)
                {
                    if (child.gameObject.activeInHierarchy)
                    {
                        childRects.Add(child);
                    }
                }
                else
                {
                    childRects.Add(child);
                }
            }
            float childCount = childRects.Count;
            float spacing = Width / childCount;
            float angleStep = (StartAngle - EndAngle) / childCount;
            if (Radial)
            {
                for (int i = 0; i < childCount; i++)
                {
                    RectTransform child = childRects[i];
                    Vector2 size = child.sizeDelta;
                    float extra = Curve.Evaluate((1 / childCount * CurveScale) * i);
                    float angle = (0 + i * angleStep) * Mathf.Deg2Rad;
                    float extraRadius = Radius;
                    if (UseAnimationCurve)
                    {
                        extraRadius = Radius * extra;
                    }
                    float x = Mathf.Cos(angle) * extraRadius;
                    float y = Mathf.Sin(angle) * extraRadius;
                    child.anchoredPosition = new Vector2(x, y) + new Vector2(padding.horizontal, padding.vertical);

                    // rotate towards
                    if (RotateTowards)
                    {
                        child.rotation = Quaternion.Euler(0, 0, (angle + RotateOffset) * Mathf.Rad2Deg - 90);
                    }
                    else
                    {
                        child.rotation = Quaternion.Euler(0, 0, RotateOffset * Mathf.Rad2Deg); ;
                    }
                    //lock values in the inspector
                    m_Tracker.Add(this, child, DrivenTransformProperties.AnchoredPosition);

                }
            }
            else
            {
                for (int i = 0; i < childCount; i++)
                {
                    float extra = Curve.Evaluate(1 / childCount * CurveScale * i);
                    // curve
                    RectTransform child = childRects[i];
                    if (child != null)
                    {
                        float x = -Width / 2f + spacing * i;
                        float y = Mathf.Sin(i / (childCount - 1) * Mathf.PI) * Height;
                        Vector2 size = child.sizeDelta;

                        if (Horizontal)
                        {
                            child.anchoredPosition = UseAnimationCurve ? new Vector2(x, Height * extra) + GetOffset(childCount * size) : new Vector2(x, y) + GetOffset(childCount * size);
                        }
                        else
                        {
                            child.anchoredPosition = UseAnimationCurve ? new Vector2(Height * extra, x) + GetOffset(childCount * size) : new Vector2(y, x) + GetOffset(childCount * size);
                        }

                        child.rotation = Quaternion.Euler(0, 0, RotateOffset * Mathf.Rad2Deg);

                        //lock values in the inspector
                        m_Tracker.Add(this, child, DrivenTransformProperties.AnchoredPosition);
                    }
                }
            }
            // bit wonky
            Vector2 GetOffset(Vector2 requiredSpaceWithoutPadding)
            {
                float requiredSpaceX = (requiredSpaceWithoutPadding.x + padding.horizontal) * GetAlignmentOnAxis(0);
                float requiredSpaceY = requiredSpaceWithoutPadding.y + padding.vertical;
                Vector2 availableSpace = rectTransform.rect.size;
                float x = padding.left + (availableSpace.x - requiredSpaceX);
                float y = padding.top + (availableSpace.y - requiredSpaceY) * GetAlignmentOnAxis(1);
                Vector2 result = new Vector2(x, y);
                return result; ;
            }

        }
    }
}

