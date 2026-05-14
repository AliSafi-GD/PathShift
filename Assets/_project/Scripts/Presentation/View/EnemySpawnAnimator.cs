using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // انیمیشن اسپان enemy. DOTween Scale + Punch.
    // Prepare() قبل از render اول صدا زده میشه تا scale=0 ست بشه و pop نکنه.
    // Play() انیمیشن اصلی رو اجرا و یه Tween برمی‌گردونه (واسه OnComplete chaining).
    public class EnemySpawnAnimator : MonoBehaviour
    {
        [Header("Scale-in")]
        [Tooltip("مدت انیمیشن اصلی scale 0 → اندازه طبیعی.")]
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private Ease ease = Ease.OutBack;
        [Tooltip("شدت overshoot برای OutBack. ۱.۷ پیش‌فرض دات‌توین.")]
        [SerializeField] private float overshoot = 2.5f;

        [Header("Punch (extra juice on land)")]
        [Tooltip("شدت پانج. 0 = غیرفعال.")]
        [SerializeField] private float punchStrength = 0.18f;
        [SerializeField] private float punchDuration = 0.18f;
        [SerializeField] private int punchVibrato = 6;
        [Range(0f, 1f)]
        [SerializeField] private float punchElasticity = 0.5f;

        [Header("Optional rotation flair")]
        [Tooltip("درجه چرخش هنگام اسپان (روی Y). 0 = غیرفعال.")]
        [SerializeField] private float spinDegrees = 180f;

        private Vector3 targetScale;
        private Quaternion targetRotation;
        private bool prepared;

        // اگه قبل از Awake/Start صدا زده بشه: scale واقعی رو از پریفب نگه می‌داره.
        public void Prepare()
        {
            if (prepared) return;
            prepared = true;

            var t = transform;
            targetScale = t.localScale;
            targetRotation = t.localRotation;

            t.localScale = Vector3.zero;
            if (spinDegrees > 0f)
                t.localRotation = targetRotation * Quaternion.Euler(0f, -spinDegrees, 0f);
        }

        public Tween Play()
        {
            Prepare();

            var seq = DOTween.Sequence();

            // Scale 0 → target با OutBack
            var scaleTween = transform.DOScale(targetScale, duration)
                .SetEase(ease, overshoot);
            seq.Append(scaleTween);

            // چرخش همزمان (در صورت داشتن spin)
            if (spinDegrees > 0f)
            {
                var rotTween = transform.DOLocalRotateQuaternion(targetRotation, duration)
                    .SetEase(Ease.OutCubic);
                seq.Join(rotTween);
            }

            // پانج وقتی scale تموم شد - بیشتر حس land
            if (punchStrength > 0f)
            {
                var punch = transform.DOPunchScale(targetScale * punchStrength, punchDuration, punchVibrato, punchElasticity);
                seq.Append(punch);
            }

            return seq;
        }

        private void OnDisable()
        {
            // قطع tweenها روی همین transform اگه شیء غیرفعال شد (مرگ زودرس و غیره)
            transform.DOKill();
        }
    }
}
