using System;
using System.Collections;
using Scrapy.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scrapy
{
    public class SceneLoadingManager : MonoBehaviour
    {
        [SerializeField] FadeCanvas loadingCanvas;
        public static SceneLoadingManager Instance { get; private set; }
        
        private Coroutine _loadSceneCoroutine = null;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Removing extra SceneLoadingManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            
        }

        public void LoadScene(string scene)
        {
            if (_loadSceneCoroutine != null)
            {
                Debug.LogError("Tried to change scene when already loading scene");
                return;
            }

            _loadSceneCoroutine = StartCoroutine(LoadSceneCoroutine(scene));
        }

        private IEnumerator LoadSceneCoroutine(string scene)
        {
            loadingCanvas.SetOpen(true);
            yield return new WaitForSecondsRealtime(loadingCanvas.OpenFadeDuration + 0.1f);
            var loading = SceneManager.LoadSceneAsync(scene);
            while (loading != null && !loading.isDone)
            {
                yield return null;
            }
            loadingCanvas.SetOpen(false);
            _loadSceneCoroutine = null;
        }
    }
}