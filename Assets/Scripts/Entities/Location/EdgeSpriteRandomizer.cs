using TriInspector;
using UnityEngine;
using UnityEngine.U2D;

namespace Entities.Location
{
    [RequireComponent(typeof(SpriteShapeController))]
    public class EdgeSpriteRandomizer : MonoBehaviour
    {
        [SerializeField] private int _spritesCount = 2;
        private SpriteShapeController _controller;
        
        [Button("Randomize Edges Now")]
        public void RandomizeEdges()
        {
            Spline spline = _controller.spline;
            for (int i = 0; i < spline.GetPointCount(); i++)
            {
                int randomIndex = Random.Range(0, _spritesCount);
                spline.SetSpriteIndex(i, randomIndex);
            }

            _controller.RefreshSpriteShape();
        }
    }
}