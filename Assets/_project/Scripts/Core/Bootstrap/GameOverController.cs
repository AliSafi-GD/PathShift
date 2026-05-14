using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameOverController : MonoBehaviour
    {
        private MainTower mainTower;
        private IWaveService waveService;
        private bool isFinished;
        private bool won;

        [Inject]
        public void Construct(MainTower mainTower, IWaveService waveService)
        {
            this.mainTower = mainTower;
            this.waveService = waveService;

            var health = this.mainTower?.Health;
            if (health != null)
                health.OnDied += HandleMainTowerDied;

            if (waveService != null)
                waveService.OnAllWavesCompleted += HandleAllWavesCompleted;
        }

        private void HandleMainTowerDied()
        {
            if (isFinished) return;
            isFinished = true;
            won = false;
            Time.timeScale = 0f;
        }

        private void HandleAllWavesCompleted()
        {
            if (isFinished) return;
            isFinished = true;
            won = true;
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            if (mainTower?.Health != null)
                mainTower.Health.OnDied -= HandleMainTowerDied;
            if (waveService != null)
                waveService.OnAllWavesCompleted -= HandleAllWavesCompleted;
        }

        private void OnGUI()
        {
            if (!isFinished) return;

            const int w = 320;
            const int h = 160;
            var rect = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);

            GUI.Box(rect, won ? "Victory!" : "Game Over");

            var labelRect = new Rect(rect.x + 20, rect.y + 35, w - 40, 30);
            GUI.Label(labelRect, won ? "All waves cleared." : "Main tower destroyed.");

            var btnRect = new Rect(rect.x + 85, rect.y + 100, 150, 40);
            if (GUI.Button(btnRect, "Reset"))
                Reset();
        }

        private void Reset()
        {
            Time.timeScale = 1f;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
