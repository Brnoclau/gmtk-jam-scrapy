using System;
using UnityEngine;

namespace Scrapy.Player
{
    public class WheelPlayerComponent : PlayerComponent
    {
        public Rigidbody2D rigidbody;
        // set from player
        [NonSerialized] public WheelJoint2D WheelJoint2D;

        public bool IsBreaking => WheelJoint2D.useMotor && WheelJoint2D.motor.motorSpeed == 0;
        public float WheelInput { get; private set; }

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
                ApplyBreaks();
                return;
            }

            WheelInput = -Input.GetAxis("Horizontal");
            var breaksDown = Input.GetKey(KeyCode.Space);

            // if ((direction > 0 && WheelJoint2D.jointSpeed > Config.wheelSpeed) ||
            // (direction < 0 && WheelJoint2D.jointSpeed < -Config.wheelSpeed))
            // {
            // don't apply torque to the same direction if already
            // return;
            // }

            if (breaksDown && Mathf.Abs(WheelJoint2D.jointSpeed) > Mathf.Epsilon
                // || 
                // direction > 0 && WheelJoint2D.jointSpeed < 0 ||
                // direction < 0 && WheelJoint2D.jointSpeed > 0
               )
            {
                // Apply break torque
                ApplyBreaks();
                // if (Mathf.Abs(WheelJoint2D.jointSpeed) < 1)
                // {
                // return;
                // }
                // WheelJoint2D.connectedBody.angularDamping = Config.breakDamping;
                // if (Mathf.Abs(WheelJoint2D.connectedBody.angularVelocity) < Config.breakStopWheelsVelocity)
                // {
                //     WheelJoint2D.connectedBody.angularVelocity = 0;
                // }

                // WheelJoint2D.connectedBody.angularVelocity -= Mathf.Sign(WheelJoint2D.jointSpeed) * Config.breakTorque * Time.fixedDeltaTime;

                // WheelJoint2D.connectedBody.AddTorque(-Mathf.Sign(WheelJoint2D.jointSpeed) * Config.breakTorque);
                return;
            }

            WheelJoint2D.useMotor = false;

            var absSpeed = Mathf.Abs(WheelJoint2D.jointSpeed);
            var speedPosClamped = Mathf.Clamp01(absSpeed / Config.wheelSpeed);
            var torque = WheelInput * WheelJoint2D.jointSpeed > 0
                ? Config.wheelMaxTorque * Config.torqueCurve.Evaluate(speedPosClamped)
                : Config.wheelMaxTorque;
            WheelJoint2D.connectedBody.AddTorque(WheelInput * torque);
            // WheelJoint2D.motor = new JointMotor2D()
            // {
            //     motorSpeed = -Config.wheelSpeed * horizontalInput,
            //     maxMotorTorque = Config.wheelMaxTorque
            // };
        }

        private void ApplyBreaks()
        {
            WheelJoint2D.motor = new JointMotor2D()
            {
                motorSpeed = 0,
                maxMotorTorque = Config.breakTorque
            };
            WheelJoint2D.useMotor = true;
        }
    }
}