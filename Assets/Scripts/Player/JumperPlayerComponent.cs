using System;
using System.Collections;
using DG.Tweening;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    public class JumperPlayerComponent : ActionPlayerComponent
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Transform springFromPoint;
        [SerializeField] private float springScaleMulti = 1;
        [SerializeField] private  JumperBit jumpBit;

        private float _jumperLastUsedAt = -10;
        
        private Rigidbody2D _rb;

        private void Awake()
        {
            jumpBit.Collided += JumpBitOnCollided;
            var player = GetComponentInParent<Player>();
            _rb = player.GetComponent<Rigidbody2D>();
        }

        private void JumpBitOnCollided()
        {
            jumpBit.RegisterCollisions = false;
            
            _rb.AddForceAtPosition(transform.up.XY() * Config.jumperImpulseAtPoint, jumpBit.transform.position, ForceMode2D.Impulse);
            Debug.DrawLine(jumpBit.transform.position, jumpBit.transform.position + transform.up, Color.red, 3.0f);
            var localRotation = transform.localRotation.eulerAngles.z;
            _rb.AddRelativeForce(Quaternion.Euler(0, 0, localRotation) * Vector3.up * Config.jumperRelativeImpulse, ForceMode2D.Impulse);
            
            Debug.DrawLine(_rb.transform.position, _rb.transform.position + Quaternion.Euler(0, 0, localRotation) * Vector3.up * Config.jumperRelativeImpulse, Color.blue, 3.0f);
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
            Debug.Log("Use");
            jumpBit.RegisterCollisions = true;
            jumpBit.transform.DOPunchPosition(Vector3.down * Config.usedDistance, Config.retractAfter)
                .OnComplete(() => jumpBit.RegisterCollisions = false);
            // jumpBit.DOPunchPosition(Vector3.down * Config.usedDistance, Config.retractAfter);
            _jumperLastUsedAt = Time.time;
        }
    }
}