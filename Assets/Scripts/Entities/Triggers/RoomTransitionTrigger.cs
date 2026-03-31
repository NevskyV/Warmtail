using System;
using System.Collections.Generic;
using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using Systems;
using Systems.Effects;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

namespace Entities.Triggers
{
    public class RoomTransitionTrigger : MonoBehaviour
    {
        [SerializeField] private float _transitionDuration = 2;
        [SerializeField] private Vector2 _dotsStartPos;
        [SerializeField] private Collider2D[] _normalColliders;
        [SerializeField] private Collider2D[] _hiddenColliders;
        [SerializeField] private SpriteShapeRenderer[] _normalSprites;
        [SerializeField] private SpriteShapeRenderer[] _hiddenSprites;
        private SpriteShapeController[] _normalControllers;
        private SpriteShapeController[] _hiddenControllers;

        private List<int> _firstPoints = new();
        private List<List<Vector2>> _differentPoints = new();
        private bool _isHidden = true;
        
        private void Start()
        {
            for (int i = 0; i < _normalColliders.Length; i++)
            {
                _normalColliders[i].OnTriggerEnter2DAsObservable().Subscribe(TriggerEnter2D);
                _hiddenColliders[i].OnTriggerEnter2DAsObservable().Subscribe(HiddenTriggerEnter2D);
            }
            
            /*for (int j = 0; j < _hiddenControllers.Length; j++)
            {
                var minSpline = _normalControllers[j].spline;
                var maxSpline = _hiddenControllers[j].spline;
                List<Vector3> samePoints = new();
                bool found = false;
                _firstPoints.Add(0);
                _differentPoints.Add(new List<Vector2>());
                for (int i = 0; i < minSpline.GetPointCount(); i++)
                {
                    samePoints.Add(minSpline.GetPosition(i));
                }
                for (int i = 0; i < maxSpline.GetPointCount(); i++)
                {
                    if (!samePoints.Contains(maxSpline.GetPosition(i)))
                    {
                        _differentPoints[j].Add(maxSpline.GetPosition(i));
                        found = true;
                    }
                    else if (!found) _firstPoints[j]++;
                }
            }*/
        }
        
        private void TriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && _isHidden)
            {
                for (int i = 0; i < _hiddenSprites.Length; i++)
                {
                    Fade(_normalSprites[i], _hiddenSprites[i]);
                }
                _normalColliders.ForEach(c => c.gameObject.SetActive(false));
                _hiddenColliders.ForEach(c => c.gameObject.SetActive(true));
                _isHidden = false;
            }
        }

        private void HiddenTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_isHidden)
            {
                for (int i = 0; i < _hiddenSprites.Length; i++)
                {
                    Fade(_hiddenSprites[i], _normalSprites[i]);
                }
                _normalColliders.ForEach(c => c.gameObject.SetActive(true));
                _hiddenColliders.ForEach(c => c.gameObject.SetActive(false));
                _isHidden = true;
            }
        }

        private async void Fade(SpriteShapeRenderer normalSprite, SpriteShapeRenderer hiddenSprite)
        {
            var normalColor = normalSprite.color;
            var hiddenColor = hiddenSprite.color;
            await foreach (var (a, b) in  CrossfadeEffect.CrossfadeTwins(1))
            {
                normalColor.a = a;
                hiddenColor.a = b;
                hiddenSprite.color = hiddenColor;
                normalSprite.color = normalColor;
            }
        }

        private void FadeIn(SpriteShapeController normalSprite, SpriteShapeController hiddenSprite)
        {
            Spline maxSpline = hiddenSprite.spline;
            Spline minSpline = normalSprite.spline;
            var index = _normalControllers.IndexOfItem(normalSprite);
            var firstPoint = _firstPoints[index];
            var differentPoints = _differentPoints[index];
            
            var diff = minSpline.GetPosition(firstPoint) - minSpline.GetPosition(firstPoint + 1);
            int c = 0;
            var pos = _dotsStartPos;
            var minDist = 1000000f;
            var maxDist = 0f;
            foreach (var point in differentPoints)
            {
                minDist = Mathf.Min(minDist, Vector2.Distance(pos,point));
                maxDist = Mathf.Max(maxDist, Vector2.Distance(pos,point));
            }
            
            foreach (var point in differentPoints)
            {
                minSpline.InsertPointAt(firstPoint + c, pos + diff.ToVector2() / differentPoints.Count * c);
                
                minSpline.SetPosition(firstPoint + c, pos);
                
                minSpline.SetTangentMode(firstPoint + c, ShapeTangentMode.Continuous);
                minSpline.SetLeftTangent(firstPoint + c, maxSpline.GetLeftTangent(firstPoint + c));
                minSpline.SetRightTangent(firstPoint + c, maxSpline.GetRightTangent(firstPoint + c));
                var c1 = c;
                DOTween.To(() => pos, x =>
                {
                    pos = x;
                    minSpline.SetPosition(firstPoint + c1, pos);
                }, point, _transitionDuration);
                
                c++;
            }
        }
        
        private void FadeOut(SpriteShapeController normalSprite, SpriteShapeController hiddenSprite)
        {
            Spline maxSpline = hiddenSprite.spline;
            Spline minSpline = normalSprite.spline;
            var index = _normalControllers.IndexOfItem(normalSprite);
            var firstPoint = _firstPoints[index];
            var differentPoints = _differentPoints[index];
            
            int c = differentPoints.Count-1;
            
            var minDist = 1000000f;
            var maxDist = 0f;
            foreach (var point in differentPoints)
            {
                minDist = Mathf.Min(minDist, Vector2.Distance(_dotsStartPos,point));
                maxDist = Mathf.Max(maxDist, Vector2.Distance(_dotsStartPos,point));
            }
            for (var i = differentPoints.Count-1; i >= 0; i--)
            {
                var pos = differentPoints[i];
                minSpline.SetTangentMode(firstPoint + c, ShapeTangentMode.Continuous);
                minSpline.SetLeftTangent(firstPoint + c, Vector2.zero);
                minSpline.SetRightTangent(firstPoint + c, Vector2.zero);
                var c1 = c;
                DOTween.To(() => pos, x =>
                {
                    pos = x;
                    minSpline.SetPosition(firstPoint + c1, pos);
                }, _dotsStartPos, _transitionDuration).OnComplete(() => minSpline.RemovePointAt(firstPoint + c1));
                c--;
            }
        }
    }
}