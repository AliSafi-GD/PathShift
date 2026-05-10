using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Domain.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    public class UnityMovement : MonoBehaviour , IMovable
    {
        private Tweener _tween;
        public GridCell CurrentCell;
        private Queue<GridCell> _pathQueue = new();
        
        public void SetPath(List<GridCell> newPath)
        {
            if (newPath == null || newPath.Count < 2)
                return;

            _pathQueue = new Queue<GridCell>(newPath);
            CurrentCell = _pathQueue.Dequeue();
        }

        private void MoveNext()
        {
            if (_pathQueue.Count == 0)
            {
                return;
            }
            float speed = 3f;
            var nextCell = _pathQueue.Dequeue();
            var target = new Vector3(nextCell.Position.X, 1, nextCell.Position.Y);

            float dist = Vector3.Distance(transform.position, target);
            float duration = dist / speed;

            MoveTo(target, duration, () =>
            {
                CurrentCell = nextCell;
                MoveNext();
            });
        }
        private void MoveTo(Vector3 target, float duration, TweenCallback onComplete)
        {
            _tween?.Kill();

            _tween = transform.DOMove(target, duration)
                .SetEase(Ease.Linear)
                .OnComplete(onComplete);
        }

        public void Move()
        {
            MoveNext();
        }
    }
}