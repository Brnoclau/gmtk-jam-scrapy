using System;
using UnityEngine;

namespace Scrapy.Player
{
    public class WheelPlayerComponent : PlayerComponent
    {
        // set from player
        [NonSerialized] public WheelJoint2D WheelJoint2D;
        
        private void Start()
        {
            
        }

        private void FixedUpdate()
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            // _currentWheelsSpeed =
            //     Mathf.Lerp(_currentWheelsSpeed, -horizontalInput * _maxWheelsSpeed, _lerp * Time.deltaTime);
            WheelJoint2D.motor = new JointMotor2D()
            {
                motorSpeed = -Config.wheelSpeed * horizontalInput,
                maxMotorTorque = Config.wheelMaxTorque
            };
        }
    }
}