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
            if (GameManager.Instance.State != GameState.Playing)
            {
                if (WheelJoint2D.motor.motorSpeed != 0)
                {
                    WheelJoint2D.motor = new JointMotor2D()
                    {
                        motorSpeed = 0,
                        maxMotorTorque = Config.wheelMaxTorque
                    };
                }

                return;
            }

            var horizontalInput = Input.GetAxis("Horizontal");
            WheelJoint2D.motor = new JointMotor2D()
            {
                motorSpeed = -Config.wheelSpeed * horizontalInput,
                maxMotorTorque = Config.wheelMaxTorque
            };
        }
    }
}