using System;
using DG.Tweening;
using UnityEngine;

namespace Scrapy.UI
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class FadeCanvas : MonoBehaviour
    {
        [SerializeField] private float openFadeDuration = .3f;
        [SerializeField] private float closeFadeDuration = .3f;

        public float OpenFadeDuration => openFadeDuration;
        public float CloseFadeDuration => closeFadeDuration;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public event Action<bool> OpenChanged;

        private Tween _tween;

        protected virtual void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas.enabled = false;
            _canvasGroup.alpha = 0;
        }


        public virtual void SetOpen(bool value)
        {
            if (_canvas.enabled == value) return;
            OpenChanged?.Invoke(value);
            if (_tween is { active: true }) _tween.Kill();
            if (value)
            {
                _canvas.enabled = true;
                _tween = _canvasGroup.DOFade(1, closeFadeDuration).SetUpdate(true);
            }
            else
            {
                _tween = _canvasGroup.DOFade(0, closeFadeDuration).SetUpdate(true)
                    .OnComplete(() => _canvas.enabled = false);
            }
        }
    }
}