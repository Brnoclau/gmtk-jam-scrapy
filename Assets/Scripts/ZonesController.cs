using System;
using System.Collections.Generic;
using DG.Tweening;
using Scrapy.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scrapy
{
    public class ZonesController : MonoBehaviour
    {
        [SerializeField] private Zone[] zones;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float crossFadeTime;
        [SerializeField] private Ease crossFadeOutEase = Ease.OutCubic;
        [SerializeField] private Ease crossFadeInEase = Ease.OutCubic;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float musicFadeTime;

        private Zone _currentZone;

        private void Awake()
        {
            foreach (var zone in zones)
            {
                foreach (var zoneTriggerCollider in zone.triggerColliders)
                {
                    zoneTriggerCollider.TriggerEntered += () =>
                    {
                        ChangeZone(zone, _currentZone == null);
                    };
                }

                foreach (var parallaxEntry in zone.parallaxEntries)
                {
                    parallaxEntry.spriteRenderer.color = new Color(1, 1, 1, 0);
                }
            }
        }

        private void Update()
        {
            if (_currentZone == null) return;
            foreach (var entry in _currentZone.parallaxEntries)
            {
                entry.spriteRenderer.transform.position = new Vector3(
                    cameraTransform.position.x * entry.parallaxFactor.x,
                    cameraTransform.position.y * entry.parallaxFactor.y) + entry.offset.XY0();
            }
        }

        private void ChangeZone(Zone zone, bool instant = false)
        {
            if (_currentZone == zone) return;
            Debug.Log($"Chaging zone to {zone.name}");

            if (_currentZone != null)
            {
                foreach (var parallaxEntry in _currentZone.parallaxEntries)
                {
                    if (instant) parallaxEntry.spriteRenderer.color = new Color(1, 1, 1, 0);
                    else parallaxEntry.spriteRenderer.DOFade(0, crossFadeTime).SetEase(crossFadeOutEase);
                }
            }

            _currentZone = zone;


            foreach (var parallaxEntry in _currentZone.parallaxEntries)
            {
                if (instant) parallaxEntry.spriteRenderer.color = new Color(1, 1, 1, 1);
                else parallaxEntry.spriteRenderer.DOFade(1, crossFadeTime).SetEase(crossFadeInEase);
            }

            if (instant)
            {
                audioSource.clip = zone.musicClip;
                audioSource.Play();
            }
            else
            {
                audioSource.DOFade(0, musicFadeTime).OnComplete(() =>
                {
                    audioSource.clip = zone.musicClip;
                    audioSource.Play();
                    audioSource.DOFade(1, musicFadeTime);
                });
            }
        }
    }

    [Serializable]
    public class Zone
    {
        public string name;
        public List<ParallaxEntry> parallaxEntries;
        public List<TriggerCollider> triggerColliders;
        public AudioClip musicClip;
    }


    [Serializable]
    public class ParallaxEntry
    {
        public SpriteRenderer spriteRenderer;
        public Vector2 parallaxFactor;
        public Vector2 offset;
    }
}