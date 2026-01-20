using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR
#endif

// Demo Script Usage:
// Add a child SpriteShape Object with a fill texture and only edit the parent object.
// Add this MonoBehavior to Child Object and set the paret and desired size.
// The spline for the child will automatically be updated while editing the parent to
// form an outer geometry.
namespace SpriteShapeExtras
{

    [ExecuteInEditMode]
    public class ConformingInvertedSpline : MonoBehaviour
    {
        public float m_OuterBoundSize = 16;
        public float m_ShortenLength = 0.9999f;
        public GameObject m_ParentObject;
        private int hashCode;
    
        // Use this for initialization
        void Start()
        {
    
        }
    
        // Update is called once per frame
        void Update()
        {
            if (m_ParentObject != null)
            {
                hashCode = CopySpline(m_ParentObject, gameObject, hashCode, GetExtendedBounds(), m_ShortenLength);
            }
        }

        Bounds GetExtendedBounds()
        {
            Bounds newBounds = m_ParentObject.GetComponent<SpriteShapeRenderer>().bounds;
            newBounds.Expand(m_OuterBoundSize);
            return newBounds;
        }

        private static int GetNearestRectPoint(Spline spline, List<Vector3> shapeRect)
        {
            int x = 0;
            float d = (spline.GetPosition(0) - shapeRect[0]).magnitude;
            for (int i = 1; i < 4; ++i)
            {
                float _d = (spline.GetPosition(0) - shapeRect[i]).magnitude;
                if (_d < d)
                {
                    d = _d;
                    x = i;
                }
            }
            return x;
        }

        private static Vector3 SetPositionJustShort(Spline spline, int index, Vector3 start, Vector3 stop, float mag)
        {
            var last = stop - start;
            var ldir = last.normalized;
            var lmag = last.magnitude * mag;
            var rlst = (ldir * lmag) + start;
            spline.SetPosition(index, rlst);
            return rlst;
        }

        private static int CopySpline(GameObject src, GameObject dst, int hashCode, Bounds bounds, float shorten)
        {
#if UNITY_EDITOR
            var parentSpriteShapeController = src.GetComponent<SpriteShapeController>();
            var mirrorSpriteShapeController = dst.GetComponent<SpriteShapeController>();
    
            if (parentSpriteShapeController != null && mirrorSpriteShapeController != null && parentSpriteShapeController.spline.GetHashCode() != hashCode)
            {
                var dstSpline = mirrorSpriteShapeController.spline;
                var srcSpline = parentSpriteShapeController.spline;

                List<Vector3> shapeRect = new List<Vector3>();
                shapeRect.Add(src.transform.localToWorldMatrix * new Vector3(bounds.min.x, bounds.min.y, 0));
                shapeRect.Add(src.transform.localToWorldMatrix * new Vector3(bounds.min.x, bounds.max.y, 0));
                shapeRect.Add(src.transform.localToWorldMatrix * new Vector3(bounds.max.x, bounds.max.y, 0));
                shapeRect.Add(src.transform.localToWorldMatrix * new Vector3(bounds.max.x, bounds.min.y, 0));

                int x = GetNearestRectPoint(dstSpline, shapeRect);
                dstSpline.Clear();

                for (int z = 0; z < 6 + srcSpline.GetPointCount(); ++z)
                    dstSpline.InsertPointAt(z, new Vector3(z, z, 0));

                dstSpline.SetPosition(0, srcSpline.GetPosition(0));
                dstSpline.SetPosition(1, shapeRect[x]);
                dstSpline.SetPosition(2, shapeRect[(x + 1) % 4]);
                dstSpline.SetPosition(3, shapeRect[(x + 2) % 4]);
                dstSpline.SetPosition(4, shapeRect[(x + 3) % 4]);

                var lst = SetPositionJustShort(dstSpline, 5, shapeRect[(x + 3) % 4], shapeRect[x], shorten );
                SetPositionJustShort(dstSpline, 6, lst, srcSpline.GetPosition(0), shorten );

                for (int j =7, k = 1; j < 6 + srcSpline.GetPointCount(); ++j, ++k)
                {
                    dstSpline.SetTangentMode(j, srcSpline.GetTangentMode(k));
                    dstSpline.SetLeftTangent(j, srcSpline.GetLeftTangent(k));
                    dstSpline.SetRightTangent(j, srcSpline.GetRightTangent(k));
                    dstSpline.SetPosition(j, srcSpline.GetPosition(k));
                }
                return parentSpriteShapeController.spline.GetHashCode();
            }
#endif
            return hashCode;
        }
    
    }

}