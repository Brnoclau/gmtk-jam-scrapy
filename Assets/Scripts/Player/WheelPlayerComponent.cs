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
            WheelJoint2D.useMotor = false;
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.State != GameState.Playing)
            {
                // if (WheelJoint2D.motor.motorSpeed != 0)
                // {
                //     WheelJoint2D.motor = new JointMotor2D()
                //     {
                //         motorSpeed = 0,
                //         maxMotorTorque = Config.wheelMaxTorque
                //     };
                // }

                return;
            }
            
            var direction = -Input.GetAxis("Horizontal");

            if ((direction > 0 && WheelJoint2D.jointSpeed > Config.wheelSpeed) ||
                (direction < 0 && WheelJoint2D.jointSpeed < -Config.wheelSpeed))
            {
                // don't apply torque to the same direction if already
                return;
            }
            
            WheelJoint2D.connectedBody.AddTorque(direction * Config.wheelMaxTorque);
            // WheelJoint2D.motor = new JointMotor2D()
            // {
            //     motorSpeed = -Config.wheelSpeed * horizontalInput,
            //     maxMotorTorque = Config.wheelMaxTorque
            // };
        }
    }
}