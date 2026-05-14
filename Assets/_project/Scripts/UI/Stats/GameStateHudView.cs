using _project.Scripts.Core.Stats;
using _project.Scripts.Core.Tower;
using _project.Scripts.Domain.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _project.Scripts.UI.Stats
{
    // HUD مرکزی state بازی. هر فیلد اختیاریه — هر کدوم ست بود نشون داده میشه.
    public class GameStateHudView : MonoBehaviour
    {
        [Header("Wave")]
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private string waveFormat = "Wave {0}/{1}";

        [Header("Enemies")]
        [SerializeField] private TMP_Text aliveText;
        [SerializeField] private string aliveFormat = "Alive: {0}";
        [SerializeField] private TMP_Text remainingText;
        [SerializeField] private string remainingFormat = "Remaining: {0}";
        [SerializeField] private TMP_Text killsText;
        [SerializeField] private string killsFormat = "Kills: {0}";
        [SerializeField] private TMP_Text leakedText;
        [SerializeField] private string leakedFormat = "Leaked: {0}";

        [Header("Base HP")]
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private string hpFormat = "Base HP: {0}/{1}";
        [SerializeField] private Slider hpBar;

        private IGameStatsService stats;
        private MainTower mainTower;
        private IHealth mainHealth;

        [Inject]
        public void Construct(IGameStatsService stats, MainTower mainTower)
        {
            this.stats = stats;
            this.mainTower = mainTower;
            this.mainHealth = mainTower?.Health;
        }

        private void Start()
        {
            if (stats != null)
            {
                stats.StatsChanged += RefreshStats;
                RefreshStats();
            }
            if (mainHealth != null)
            {
                mainHealth.OnHealthChanged += OnHpChanged;
                OnHpChanged(mainHealth.CurrentHealth, mainHealth.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (stats != null) stats.StatsChanged -= RefreshStats;
            if (mainHealth != null) mainHealth.OnHealthChanged -= OnHpChanged;
        }

        private void RefreshStats()
        {
            if (stats == null) return;
            Set(waveText, waveFormat, stats.CurrentWave, stats.TotalWaves);
            Set(aliveText, aliveFormat, stats.EnemiesAlive);
            Set(remainingText, remainingFormat, stats.EnemiesRemainingToSpawn);
            Set(killsText, killsFormat, stats.EnemiesKilledByTower);
            Set(leakedText, leakedFormat, stats.EnemiesReachedEnd);
        }

        private void OnHpChanged(float current, float max)
        {
            if (hpText != null) hpText.text = string.Format(hpFormat, Mathf.CeilToInt(current), Mathf.CeilToInt(max));
            if (hpBar != null)
            {
                hpBar.maxValue = max;
                hpBar.value = current;
            }
        }

        private static void Set(TMP_Text label, string fmt, params object[] args)
        {
            if (label == null) return;
            label.text = string.Format(fmt, args);
        }
    }
}
