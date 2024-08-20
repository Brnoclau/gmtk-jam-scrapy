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

        private bool _open;
        private Canvas Canvas => _canvas != null ? _canvas : _canvas = GetComponent<Canvas>();
        private Canvas _canvas;
        private CanvasGroup CanvasGroup => _canvasGroup != null ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();
        private CanvasGroup _canvasGroup;

        public event Action<bool> OpenChanged;

        private Tween _tween;

        protected virtual void Awake()
        {
            Canvas.enabled = false;
            CanvasGroup.alpha = 0;
        }


        public virtual void SetOpen(bool value, bool instant = false)
        {
            if (_open == value) return;
            _open = value;
            OpenChanged?.Invoke(value);
            if (_tween is { active: true }) _tween.Kill();
            CanvasGroup.DOKill();
            if (value)
            {
                Canvas.enabled = true;
                if (!instant) _tween = CanvasGroup.DOFade(1, closeFadeDuration).SetUpdate(true);
                else CanvasGroup.alpha = 1;
            }
            else
            {
                if (!instant) _tween = CanvasGroup.DOFade(0, closeFadeDuration).SetUpdate(true)
                    .OnComplete(() => _canvas.enabled = false);
                else
                {
                    Canvas.enabled = false;
                    CanvasGroup.alpha = 0;
                }
            }
        }
    }
}