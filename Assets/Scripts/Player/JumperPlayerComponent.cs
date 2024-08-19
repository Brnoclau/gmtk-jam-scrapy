using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Scrapy.Player
{
    public class JumperPlayerComponent : ActionPlayerComponent
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Transform springFromPoint;
        [SerializeField] private float springScaleMulti = 1;
        public Rigidbody2D jumpBit;

        [NonSerialized] public SliderJoint2D Slider;

        private float _jumperLastUsedAt = -10;

        protected override void Start()
        {
            base.Start();
            Slider.limits = new JointTranslationLimits2D()
            {
                min = Slider.limits.min,
                max = Config.restDistance
            };
            jumpBit.sharedMaterial = new PhysicsMaterial2D()
            {
                bounciness = 0,
                friction = jumpBit.sharedMaterial.friction
            };
        }

        private void FixedUpdate()
        {
            if (IsActive && _jumperLastUsedAt + Config.jumperCooldown < Time.time)
            {
                Use();
            }
        }

        protected override void Update()
        {
            base.Update();
            PlaceSpriteBetweenPoints();
        }

        private void PlaceSpriteBetweenPoints()
        {
            var spriteCenter = (springFromPoint.transform.position + jumpBit.transform.position) / 2;
            var spriteSize = Vector2.Distance(springFromPoint.transform.position, jumpBit.transform.position);
            var angle = Vector2.SignedAngle(Vector2.up, jumpBit.transform.position - springFromPoint.transform.position);
            sprite.transform.position = spriteCenter;
            sprite.transform.localScale = new Vector3( sprite.transform.localScale.x, spriteSize * springScaleMulti, sprite.transform.localScale.z);
            sprite.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void Use()
        {
            StartCoroutine(UseCoroutine());
            // jumpBit.DOPunchPosition(Vector3.down * Config.usedDistance, Config.retractAfter);
            // _jumperLastUsedAt = Time.time;
        }

        IEnumerator UseCoroutine()
        {
            Debug.Log("Activating jumper");
            _jumperLastUsedAt = Time.time;
            Slider.limits = new JointTranslationLimits2D()
            {
                min = Slider.limits.min,
                max = Config.usedDistance
            };
            jumpBit.sharedMaterial = new PhysicsMaterial2D()
            {
                bounciness = Config.bounciness,
                friction = jumpBit.sharedMaterial.friction
            };
            yield return new WaitForSeconds(Config.retractAfter);
            jumpBit.sharedMaterial = new PhysicsMaterial2D()
            {
                bounciness = 0,
                friction = jumpBit.sharedMaterial.friction
            };
            DOTween.To(() => Slider.limits.max, (val) =>
                {
                    Slider.limits = new JointTranslationLimits2D()
                    {
                        min = Slider.limits.min,
                        max = val
                    };
                },
                Config.restDistance, Config.retractTime);
            // Slider.limits = new JointTranslationLimits2D()
            // {
            // min = Slider.limits.min,
            // max = Config.restDistance
            // };
        }
    }
}