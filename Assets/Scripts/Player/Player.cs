using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using Script.Util;
using UnityEngine;

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
    
    public PlayerConfig componentsConfig = new();

    private readonly List<PlayerComponent> _attachedComponents = new();

    public float ThrusterCharge { get; private set; }
    public float MaxThrusterCharge => _maxThrusterCharge;

    ConstantForce2D _thrusterForce;
    List<WheelJoint2D> _wheelJoints;
    private bool _thrusterFinishedDuringThisInput;
    private float _currentWheelsSpeed;
    private float _thrusterLastUsedTime;

    private void Awake()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameState.Workshop)
        {
            var rbs = GetComponentsInChildren<Rigidbody2D>();
            foreach (var rb in rbs)
            {
                rb.simulated = false;
            }
        }
        if (newState == GameState.Playing)
        {
            var rbs = GetComponentsInChildren<Rigidbody2D>();
            foreach (var rb in rbs)
            {
                rb.simulated = true;
            }
        }
    }

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
        AssembleFromConfig();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        var horizontalInput = Input.GetAxis("Horizontal");
        _currentWheelsSpeed =
            Mathf.Lerp(_currentWheelsSpeed, -horizontalInput * _maxWheelsSpeed, _lerp * Time.deltaTime);
        SetWheelsSpeed(_currentWheelsSpeed);
        UpdateThruster();
    }

    public void AttachNewComponent(AttachedComponentConfig componentConfig, bool addToConfig)
    {
        var component = Instantiate(componentConfig.component.prefab, transform);
        component.transform.localPosition = componentConfig.position.XY0();
        component.transform.localRotation = Quaternion.Euler(0, 0, componentConfig.rotation);
        SetupComponent(component, componentConfig.component);
        // This disables components that are just attached if we are in the workshop state
        OnStateChanged(GameManager.Instance.State, GameManager.Instance.State);
        if (addToConfig) componentsConfig.attachedComponents.Add(componentConfig);
    }
    
    void AssembleFromConfig()
    {
        foreach (var componentConfig in componentsConfig.attachedComponents)
        {
            AttachNewComponent(componentConfig, false);
        }
    }

    private void SetupComponent(GameObject component, PlayerComponentConfig config)
    {
        var componentClass = component.GetComponent<PlayerComponent>();
        if (componentClass == null)
        {
            Debug.LogError("Tried to setup component without PlayerComponent");
            return;
        }
        _attachedComponents.Add(componentClass);
        switch (config.type)
        {
            case PlayerComponentType.Body:
                break;
            case PlayerComponentType.Wheel:
                var rb = component.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    Debug.LogError(
                        $"Tried to assemble wheel component but prefab {config.prefab.name} doesn't have a rigidbody");
                    break;
                }
                var wheelJoint = gameObject.AddComponent<WheelJoint2D>();
                wheelJoint.connectedBody = rb;
                wheelJoint.anchor = component.transform.localPosition.XY();
                _wheelJoints.Add(wheelJoint);
                break;
        }
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