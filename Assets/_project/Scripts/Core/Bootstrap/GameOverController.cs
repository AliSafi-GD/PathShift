using _project.Scripts.Core.Tower;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameOverController : MonoBehaviour
    {
        private MainTower mainTower;
        private bool isGameOver;

        [Inject]
        public void Construct(MainTower mainTower)
        {
            this.mainTower = mainTower;

            var health = this.mainTower?.Health;
            if (health != null)
                health.OnDied += HandleMainTowerDied;
        }

        private void HandleMainTowerDied()
        {
            if (isGameOver) return;
            isGameOver = true;
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            if (mainTower?.Health != null)
                mainTower.Health.OnDied -= HandleMainTowerDied;
        }

        private void OnGUI()
        {
            if (!isGameOver) return;

            const int w = 300;
            const int h = 140;
            var rect = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);

            GUI.Box(rect, "Game Over");

            var labelRect = new Rect(rect.x + 20, rect.y + 35, w - 40, 30);
            GUI.Label(labelRect, "Main tower destroyed.");

            var btnRect = new Rect(rect.x + 75, rect.y + 80, 150, 40);
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
