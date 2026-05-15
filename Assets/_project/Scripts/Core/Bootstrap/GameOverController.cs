using _project.Scripts.Core.Infrastructure;
using _project.Scripts.Core.State;
using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using UnityEngine;
using VContainer;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameOverController : MonoBehaviour
    {
        private MainTower mainTower;
        private IWaveService waveService;
        private IGameStateMachine stateMachine;
        private ISceneLoader sceneLoader;

        [Inject]
        public void Construct(
            MainTower mainTower,
            IWaveService waveService,
            IGameStateMachine stateMachine,
            ISceneLoader sceneLoader)
        {
            this.mainTower = mainTower;
            this.waveService = waveService;
            this.stateMachine = stateMachine;
            this.sceneLoader = sceneLoader;

            if (mainTower?.Health != null)
                mainTower.Health.OnDied += HandleMainTowerDied;

            if (waveService != null)
                waveService.OnAllWavesCompleted += HandleAllWavesCompleted;
        }

        private void HandleMainTowerDied() => stateMachine.Lose();
        private void HandleAllWavesCompleted() => stateMachine.Win();

        private void OnDestroy()
        {
            if (mainTower?.Health != null)
                mainTower.Health.OnDied -= HandleMainTowerDied;
            if (waveService != null)
                waveService.OnAllWavesCompleted -= HandleAllWavesCompleted;
        }

        private void OnGUI()
        {
            if (!stateMachine.IsFinished) return;

            const int w = 320;
            const int h = 160;
            var rect = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);

            var won = stateMachine.Current == GameState.Won;
            GUI.Box(rect, won ? "Victory!" : "Game Over");

            var labelRect = new Rect(rect.x + 20, rect.y + 35, w - 40, 30);
            GUI.Label(labelRect, won ? "All waves cleared." : "Main tower destroyed.");

            var btnRect = new Rect(rect.x + 85, rect.y + 100, 150, 40);
            if (GUI.Button(btnRect, "Reset"))
                sceneLoader.ReloadCurrentScene();
        }
    }
}
