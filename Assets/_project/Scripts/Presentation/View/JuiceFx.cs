using System;
using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // helperهای کوچیک برای انیمیشن‌های زندگی/مرگ object‌ها.
    public static class JuiceFx
    {
        // shrink + spin + (اختیاری) flat → کال‌بک onComplete (که معمولاً Destroy می‌کنه).
        public static Tween DespawnShrinkSpin(Transform t, float duration, Action onComplete = null)
        {
            if (t == null) { onComplete?.Invoke(); return null; }

            duration = Mathf.Max(0.05f, duration);
            t.DOKill();

            var seq = DOTween.Sequence();
            seq.Append(t.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            seq.Join(t.DOLocalRotate(t.localEulerAngles + new Vector3(0f, 360f, 0f), duration, RotateMode.FastBeyond360)
                      .SetEase(Ease.InCubic));
            seq.OnComplete(() => onComplete?.Invoke());
            return seq;
        }

        // up + shrink + spin (انفجار به بالا) → onComplete.
        public static Tween DespawnPopUp(Transform t, float duration, float upDistance, Action onComplete = null)
        {
            if (t == null) { onComplete?.Invoke(); return null; }

            duration = Mathf.Max(0.05f, duration);
            t.DOKill();

            var seq = DOTween.Sequence();
            seq.Append(t.DOMoveY(t.position.y + upDistance, duration).SetEase(Ease.OutCubic));
            seq.Join(t.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad));
            seq.Join(t.DOLocalRotate(t.localEulerAngles + new Vector3(0f, 540f, 0f), duration, RotateMode.FastBeyond360)
                      .SetEase(Ease.InCubic));
            seq.OnComplete(() => onComplete?.Invoke());
            return seq;
        }
    }
}
