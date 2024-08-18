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
        
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        public event Action<bool> OpenChanged;

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
            _canvasGroup.DOKill();
            if (value)
            {
                _canvas.enabled = true;
                _canvasGroup.DOFade(1, closeFadeDuration);
            }
            else
            {
                _canvasGroup.DOFade(0, closeFadeDuration).OnComplete(() => _canvas.enabled = false);
            }
        }
    }
}