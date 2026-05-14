using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // انیمیشن راه‌رفتن enemy: bob عمودی + wobble z + squash-stretch.
    // فقط روی یه child visual کار می‌کنه چون root با DOMove از UnityMovement حرکت می‌کنه.
    [DisallowMultipleComponent]
    public class EnemyWalkAnimator : MonoBehaviour
    {
        [Header("Visual (child transform). null = اولین child استفاده میشه.")]
        [SerializeField] private Transform visual;

        [Header("Bob (up/down)")]
        [SerializeField] private float bobAmplitude = 0.12f;

        [Header("Wobble (side-to-side roll, Z axis)")]
        [SerializeField] private float wobbleAngle = 8f;

        [Header("Squash & Stretch")]
        [Tooltip("شدت اسکوش. 0 = خاموش. 0.1 یعنی 10% تغییر در Y و معکوسش در X/Z.")]
        [SerializeField] private float squash = 0.08f;

        [Header("Timing")]
        [Tooltip("مدت یک قدم کامل (up→down→up).")]
        [SerializeField] private float period = 0.45f;
        [SerializeField] private float randomPhaseRange = 0.4f;

        private Vector3 basePos;
        private Quaternion baseRot;
        private Vector3 baseScale;

        private Sequence bobSeq;
        private Sequence wobbleSeq;
        private Sequence squashSeq;

        private void Awake()
        {
            ResolveVisual();
            if (visual == null) return;

            basePos = visual.localPosition;
            baseRot = visual.localRotation;
            baseScale = visual.localScale;
        }

        private void OnEnable()
        {
            if (visual == null) ResolveVisual();
            if (visual == null) return;
            PlayWalk();
        }

        private void OnDisable()
        {
            KillTweens();
            if (visual != null)
            {
                visual.localPosition = basePos;
                visual.localRotation = baseRot;
                visual.localScale = baseScale;
            }
        }

        private void ResolveVisual()
        {
            if (visual != null) return;
            if (transform.childCount > 0) visual = transform.GetChild(0);
            // اگه باز null بود، در OnEnable skip میشه.
        }

        private void PlayWalk()
        {
            KillTweens();

            float half = Mathf.Max(0.05f, period * 0.5f);
            float phaseDelay = Random.Range(0f, randomPhaseRange);

            // Bob: Y بالا و پایین
            if (bobAmplitude > 0f)
            {
                bobSeq = DOTween.Sequence().SetDelay(phaseDelay);
                bobSeq.Append(visual.DOLocalMoveY(basePos.y + bobAmplitude, half).SetEase(Ease.InOutSine));
                bobSeq.Append(visual.DOLocalMoveY(basePos.y - bobAmplitude * 0.5f, half).SetEase(Ease.InOutSine));
                bobSeq.SetLoops(-1, LoopType.Restart);
            }

            // Wobble: roll روی Z
            if (wobbleAngle > 0f)
            {
                var euler = baseRot.eulerAngles;
                wobbleSeq = DOTween.Sequence().SetDelay(phaseDelay + half * 0.25f);
                wobbleSeq.Append(visual.DOLocalRotate(new Vector3(euler.x, euler.y, euler.z + wobbleAngle), half).SetEase(Ease.InOutSine));
                wobbleSeq.Append(visual.DOLocalRotate(new Vector3(euler.x, euler.y, euler.z - wobbleAngle), half).SetEase(Ease.InOutSine));
                wobbleSeq.SetLoops(-1, LoopType.Restart);
            }

            // Squash و Stretch همگام با bob
            if (squash > 0f)
            {
                var squashed = new Vector3(baseScale.x * (1f + squash * 0.5f), baseScale.y * (1f - squash), baseScale.z * (1f + squash * 0.5f));
                var stretched = new Vector3(baseScale.x * (1f - squash * 0.3f), baseScale.y * (1f + squash * 0.5f), baseScale.z * (1f - squash * 0.3f));

                squashSeq = DOTween.Sequence().SetDelay(phaseDelay);
                squashSeq.Append(visual.DOScale(stretched, half).SetEase(Ease.InOutSine));
                squashSeq.Append(visual.DOScale(squashed, half).SetEase(Ease.InOutSine));
                squashSeq.SetLoops(-1, LoopType.Restart);
            }
        }

        private void KillTweens()
        {
            bobSeq?.Kill();
            wobbleSeq?.Kill();
            squashSeq?.Kill();
            bobSeq = wobbleSeq = squashSeq = null;
        }

        private void OnDestroy() => KillTweens();
    }
}
