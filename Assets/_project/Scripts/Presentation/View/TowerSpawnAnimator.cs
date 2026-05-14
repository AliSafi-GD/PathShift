using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // انیمیشن drop-from-sky برای tower. Prepare() قبل از render تنظیم میشه،
    // Play() تا موقعیت زمین tween می‌کنه با bounce + punch.
    [DisallowMultipleComponent]
    public class TowerSpawnAnimator : MonoBehaviour
    {
        [Header("Drop")]
        [SerializeField] private float dropHeight = 8f;
        [SerializeField] private float dropDuration = 0.55f;
        [SerializeField] private Ease dropEase = Ease.OutBounce;

        [Header("Land punch")]
        [SerializeField] private float punchStrength = 0.22f;
        [SerializeField] private float punchDuration = 0.25f;
        [SerializeField] private int punchVibrato = 6;
        [Range(0f, 1f)]
        [SerializeField] private float punchElasticity = 0.5f;

        private Vector3 targetPos;
        private bool prepared;

        public void Prepare()
        {
            if (prepared) return;
            prepared = true;
            targetPos = transform.position;
            transform.position = targetPos + Vector3.up * dropHeight;
        }

        public Tween Play()
        {
            Prepare();

            var seq = DOTween.Sequence();
            seq.Append(transform.DOMove(targetPos, dropDuration).SetEase(dropEase));

            if (punchStrength > 0f)
                seq.Append(transform.DOPunchScale(transform.localScale * punchStrength, punchDuration, punchVibrato, punchElasticity));

            return seq;
        }

        private void OnDisable() => transform.DOKill();
    }
}
