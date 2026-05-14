using System;
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
        private Tweener _rotateTween;
        public GridCell CurrentCell;
        private Queue<GridCell> _pathQueue = new();
        private bool _isMoving;
        private float speed = 3f;

        [SerializeField] private float turnDuration = 0.15f;

        public event Action OnFinishedMove;

        public void SetSpeed(float value) => speed = Mathf.Max(0.01f, value);

        public void SetPath(List<GridCell> newPath)
        {
            if (newPath == null || newPath.Count == 0)
                return;

            // tween فعلی متوقف بشه چون مسیر عوض شده
            _tween?.Kill();

            _pathQueue = new Queue<GridCell>(newPath);

            // اولین سل اگه دقیقاً همون cell فعلی ماست، dequeue کن
            // در غیر این صورت همه سل‌های مسیر باید پیموده بشن
            if (_pathQueue.Count > 0 && CurrentCell != null &&
                _pathQueue.Peek().Id == CurrentCell.Id)
            {
                _pathQueue.Dequeue();
            }
            else if (CurrentCell == null && _pathQueue.Count > 0)
            {
                // اولین بار است (هنوز spawn شده ولی شروع نکرده)
                CurrentCell = _pathQueue.Dequeue();
            }

            // اگه قبلاً داشت حرکت میکرد، از سل بعدی ادامه بده
            if (_isMoving)
                MoveNext();
        }

        public void Move()
        {
            if (_isMoving) return;
            _isMoving = true;
            MoveNext();
        }

        private void MoveNext()
        {
            if (_pathQueue.Count == 0)
            {
                _isMoving = false;
                OnFinishedMove?.Invoke();
                return;
            }

            var nextCell = _pathQueue.Dequeue();
            var target = nextCell.WorldPosition;

            float dist = Vector3.Distance(transform.position, target);
            float duration = dist / speed;

            FaceTowards(target);
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

        private void FaceTowards(Vector3 target)
        {
            var dir = target - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            var look = Quaternion.LookRotation(dir, Vector3.up);
            _rotateTween?.Kill();
            if (turnDuration <= 0f)
            {
                transform.rotation = look;
                return;
            }
            _rotateTween = transform.DORotateQuaternion(look, turnDuration).SetEase(Ease.OutCubic);
        }

        private void OnDestroy()
        {
            _tween?.Kill();
            _rotateTween?.Kill();
        }
    }
}