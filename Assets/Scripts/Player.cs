using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] float _maxWheelsSpeed;
    [SerializeField] float _lerp;
    [SerializeField] private ParticleSystem _thrusterParticles;
    [SerializeField] private float _maxThrusterForce;
    [SerializeField] private float _maxThrusterTorque;
    [SerializeField] private float _maxThrusterCharge;
    [SerializeField] private float _thrusterRechargeRelay;
    [SerializeField] private float _thrusterRechargePerSecond;
    
    
    public float ThrusterCharge { get; private set; }
    public float MaxThrusterCharge => _maxThrusterCharge;
    
    ConstantForce2D _thrusterForce;
    List<WheelJoint2D> _wheelJoints;
    private bool _thrusterFinishedDuringThisInput;
    private float _currentWheelsSpeed;
    private float _thrusterLastUsedTime;
    
    // Start is called before the first frame update
    void Start()
    {
        _wheelJoints = new List<WheelJoint2D>();
        foreach (var wheel in GetComponentsInChildren<WheelJoint2D>())
        {
            _wheelJoints.Add(wheel);
        }
        _thrusterForce = GetComponent<ConstantForce2D>();
        ThrusterCharge = _maxThrusterCharge;
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        
        // if (Input.GetKey(KeyCode.A))
        // {
        //     _currentSpeed = Mathf.Lerp(_currentSpeed, _maxWheelsSpeed, _lerp * Time.deltaTime);
        // } else if (Input.GetKey(KeyCode.D))
        // {
        //     _currentSpeed = Mathf.Lerp(_currentSpeed, -_maxWheelsSpeed, _lerp * Time.deltaTime);
        // }
        // else
        // {
        //     _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _lerp * Time.deltaTime);
        //     // _currentSpeed = 0;
        // }
        
        _currentWheelsSpeed = Mathf.Lerp(_currentWheelsSpeed, -horizontalInput * _maxWheelsSpeed, _lerp * Time.deltaTime);
        SetWheelsSpeed(_currentWheelsSpeed);
        UpdateThruster();
    }

    void UpdateThruster()
    {
        var horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W) && ThrusterCharge > 0 && !_thrusterFinishedDuringThisInput)
        {
            _thrusterForce.relativeForce = new Vector2(0, _maxThrusterForce);
            _thrusterForce.torque = -_maxThrusterTorque * horizontalInput;
            if (!_thrusterParticles.isEmitting) _thrusterParticles.Play();
            ThrusterCharge -= Time.deltaTime;
            if (ThrusterCharge < 0)
            {
                ThrusterCharge = 0;
                _thrusterFinishedDuringThisInput = true;
            }
            _thrusterLastUsedTime = Time.time;
        }
        else
        {
            _thrusterForce.relativeForce = new Vector2(0, 0);
            _thrusterForce.torque = 0;
            _thrusterParticles.Stop();
            if (_thrusterFinishedDuringThisInput && !Input.GetKey(KeyCode.W))
            {
                _thrusterFinishedDuringThisInput = false;
            }
            if (Time.time - _thrusterLastUsedTime > _thrusterRechargeRelay)
            {
                ThrusterCharge += _thrusterRechargePerSecond * Time.deltaTime;
                if (ThrusterCharge > _maxThrusterCharge) ThrusterCharge = _maxThrusterCharge;
            }
        }
    }
    
    private void SetWheelsSpeed(float speed)
    {
        foreach (var wheelJoint in _wheelJoints)
        {
            wheelJoint.useMotor = true;
            JointMotor2D motor = wheelJoint.motor;
            motor.motorSpeed = speed;
            wheelJoint.motor = motor;
        }
    }
}
