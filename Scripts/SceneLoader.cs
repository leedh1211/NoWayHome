using System.Collections;
using _02.Scripts.Resource;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        public void LoadGameScene()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("GameScene");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(WaitAndInit());
        }

        private IEnumerator WaitAndInit()
        {
            ResourcePool pool = FindObjectOfType<ResourcePool>();

            while (pool == null || !pool.IsInitialized || GameManager.Instance == null)
            {
                yield return null;
                pool = FindObjectOfType<ResourcePool>();
            }

            GameManager.Instance.InitStage();
        }
    }
}