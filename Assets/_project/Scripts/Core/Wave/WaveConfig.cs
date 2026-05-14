using System;
using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using UnityEngine;

namespace _project.Scripts.Core.Wave
{
    [Serializable]
    public struct WaveSpawnGroup
    {
        public EnemyConfig enemy;
        [Min(1)] public int count;
        [Tooltip("فاصله بین اسپان‌های داخل این گروه (ثانیه).")]
        [Min(0f)] public float interval;
        [Tooltip("وقفه قبل از شروع این گروه (ثانیه).")]
        [Min(0f)] public float delayBefore;
    }

    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Configs/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] private List<WaveSpawnGroup> groups = new();
        [Tooltip("وقفه بعد از پایان این wave و قبل از wave بعدی (ثانیه).")]
        [Min(0f)]
        [SerializeField] private float delayAfter = 3f;

        public IReadOnlyList<WaveSpawnGroup> Groups => groups;
        public float DelayAfter => delayAfter;
    }
}
