using UnityEngine;
using UnityEngine.SceneManagement;

namespace _project.Scripts.Core.Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        public void ReloadCurrentScene()
        {
            Time.timeScale = 1f;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
