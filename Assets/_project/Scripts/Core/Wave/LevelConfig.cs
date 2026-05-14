using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Core.Wave
{
    // یه لول کامل = مجموعه‌ای از waves. پلیر وقتی همه waveها spawn شدن
    // و همه‌ی enemyها مردن، برنده میشه.
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private List<WaveConfig> waves = new();
        [Tooltip("وقفه قبل از شروع اولین wave (ثانیه).")]
        [Min(0f)]
        [SerializeField] private float startDelay = 1f;

        public IReadOnlyList<WaveConfig> Waves => waves;
        public float StartDelay => startDelay;
    }
}
