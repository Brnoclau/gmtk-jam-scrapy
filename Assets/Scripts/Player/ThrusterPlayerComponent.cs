using System;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    public class ThrusterPlayerComponent : ActionPlayerComponent
    {
        [SerializeField] private ParticleSystem particles;
        
        private Rigidbody2D _rb;
        
        protected override void Start()
        {
            base.Start();
            var player = GetComponentInParent<Player>();
            _rb = player.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (!IsActive)
            {
                if (particles) particles.Stop();
                return;
            }
            if (particles && !particles.isEmitting) particles.Play();
            
            _rb.AddForceAtPosition(transform.up.XY() * Config.thrusterForceAtPoint,transform.position.XY());
            
            var localRotation = transform.localRotation.eulerAngles.z;
            _rb.AddRelativeForce(Quaternion.Euler(0, 0, localRotation) * Vector3.up * Config.thrusterRelativeForce);
        }
    }
}