using System;
using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // انیمیشن مرگ enemy. spawner این رو قبل از Destroy صدا می‌زنه.
    [DisallowMultipleComponent]
    public class EnemyDeathAnimator : MonoBehaviour
    {
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private float upDistance = 0.6f;
        [SerializeField] private float spinDegrees = 540f;

        private bool playing;

        public void Play(Action onComplete)
        {
            if (playing) return;
            playing = true;

            // walk anim و سایر tweenها روی subtree متوقف بشن
            transform.DOKill(true);
            foreach (var child in GetComponentsInChildren<Transform>(true))
                child.DOKill();

            // colliderها خاموش تا با راست/پروژایل تداخل نکنه
            foreach (var c in GetComponentsInChildren<Collider>(true)) c.enabled = false;

            JuiceFx.DespawnPopUp(transform, duration, upDistance, onComplete);
            // spin اضافه روی root
            transform.DOLocalRotate(transform.localEulerAngles + new Vector3(0f, spinDegrees, 0f),
                duration, RotateMode.FastBeyond360).SetEase(Ease.InCubic);
        }
    }
}
