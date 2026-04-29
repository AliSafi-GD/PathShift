using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        public Action OnReachedEnd;

        public void Move(List<Vector3> path)
        {
            if (path == null || path.Count < 2)
            {
                Debug.LogWarning("[EnemyView] Path is null or too short.");
                OnReachedEnd?.Invoke();
                return;
            }

            transform.DOKill();

            float speed = 3f;
            float totalLength = 0f;
            for (int i = 1; i < path.Count; i++)
                totalLength += Vector3.Distance(path[i - 1], path[i]);

            float duration = totalLength / speed;

            transform.DOPath(path.ToArray(), duration, PathType.Linear)
                     .SetEase(Ease.Linear)
                     .OnComplete(() => OnReachedEnd?.Invoke());
        }

        private void OnDisable()
        {
            transform.DOKill();
            OnReachedEnd = null;
        }
    }
}
