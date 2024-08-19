using System;
using System.Collections;
using DG.Tweening;
using Scrapy.UI;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapy
{
    public class CreditsUI : MonoBehaviour
    {
        [SerializeField] private FadeCanvas fadeCanvas;
        [SerializeField] private Animator animator;
        [SerializeField] private float creditsLength = 10;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float musicFadeTime = 0.5f;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private bool closeOnExit = false;

        public event Action FinishedCredits;

        private float _sfxVolume;
        private bool _showingCredits;

        public void ShowCredits()
        {
            if (_showingCredits)
            {
                Debug.LogWarning("Already showing credits!");
                return;
            }
            DOTween.To(() => Time.timeScale, (value) => Time.timeScale = value, 0, fadeCanvas.OpenFadeDuration);
            fadeCanvas.SetOpen(true);
            animator.SetBool("play", true);
            audioSource.Play();
            audioSource.DOFade(1, musicFadeTime);
            _showingCredits = true;
            StartCoroutine(CreditsCoroutine());
            if (mixer.GetFloat("SFXVolume", out var volume))
            {
                _sfxVolume = volume;
            }
            mixer.SetFloat("SFXVolume", -80);
        }

        private void Update()
        {
            if (_showingCredits && Input.GetKeyDown(KeyCode.Escape))
            {
                ExitCredits();
            }
        }

        public void ExitCredits()
        {
            if (!_showingCredits) return;
            _showingCredits = false;
            Debug.Log("Finishing credits");
            DOTween.To(() => Time.timeScale, (value) => Time.timeScale = value, 1, fadeCanvas.CloseFadeDuration)
                .SetUpdate(true);
            animator.SetBool("play", false);
            if (closeOnExit) fadeCanvas.SetOpen(false);
            audioSource.DOFade(0, musicFadeTime).OnComplete(() => audioSource.Stop())
                .SetUpdate(true);
            FinishedCredits?.Invoke();
            mixer.SetFloat("SFXVolume", _sfxVolume);
        }

        private IEnumerator CreditsCoroutine()
        {
            yield return new WaitForSecondsRealtime(creditsLength);
            ExitCredits();
        }
    }
}