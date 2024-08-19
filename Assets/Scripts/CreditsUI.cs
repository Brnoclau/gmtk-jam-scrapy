using System;
using System.Collections;
using DG.Tweening;
using Scrapy.UI;
using UnityEngine;

namespace Scrapy
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] private FadeCanvas fadeCanvas;
        [SerializeField] private Animator animator;
        [SerializeField] private float creditsLength = 10;

        public event Action FinishedCredits;
        
        Coroutine _creditsCoroutine;

        public void ShowCredits()
        {
            if (_creditsCoroutine != null)
            {
                Debug.LogWarning("Already showing credits!");
                return;
            }
            DOTween.To(() => Time.timeScale, (value) => Time.timeScale = value, 0, fadeCanvas.OpenFadeDuration);
            fadeCanvas.SetOpen(true);
            animator.SetBool("play", true);
            _creditsCoroutine = StartCoroutine(CreditsCoroutine());
        }

        private void Update()
        {
            if (_creditsCoroutine != null && Input.GetKeyDown(KeyCode.Escape))
            {
                ExitCredits();
            }
        }

        public void ExitCredits()
        {
            if (_creditsCoroutine == null) return;
            _creditsCoroutine = null;
            DOTween.To(() => Time.timeScale, (value) => Time.timeScale = value, 1, fadeCanvas.CloseFadeDuration);
            animator.SetBool("play", false);
            fadeCanvas.SetOpen(false);
            FinishedCredits?.Invoke();
        }

        private IEnumerator CreditsCoroutine()
        {
            yield return new WaitForSecondsRealtime(creditsLength);
            ExitCredits();
        }
    }
}