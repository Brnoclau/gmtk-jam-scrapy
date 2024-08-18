using System;
using System.Collections.Generic;
using System.Linq;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [Header("No components configuration")] [SerializeField]
        private float _maxBodyTorque;

        [SerializeField] private float _maxBodyAngularSpeed;
        [SerializeField] private float _bodyTorqueAcceleration;
        [Header("Obsolete")] [SerializeField] private ParticleSystem _thrusterParticles;
        [SerializeField] private float _maxThrusterForce;
        [SerializeField] private float _maxThrusterTorque;
        [SerializeField] private float _maxThrusterCharge;
        [SerializeField] private float _thrusterRechargeRelay;
        [SerializeField] private float _thrusterRechargePerSecond;

        private readonly List<AttachedComponent> _attachedComponents = new();
        public IReadOnlyList<AttachedComponent> AttachedComponents => _attachedComponents;

        public float ThrusterCharge { get; private set; }
        public float MaxThrusterCharge => _maxThrusterCharge;

        private Rigidbody2D _rb;
        ConstantForce2D _thrusterForce;
        List<WheelJoint2D> _wheelJoints = new();
        private bool _thrusterFinishedDuringThisInput;
        private float _currentWheelsSpeed;
        private float _thrusterLastUsedTime;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            _wheelJoints = new List<WheelJoint2D>();
            foreach (var wheel in GetComponentsInChildren<WheelJoint2D>())
            {
                _wheelJoints.Add(wheel);
            }

            _thrusterForce = GetComponent<ConstantForce2D>();
            ThrusterCharge = _maxThrusterCharge;
            _thrusterParticles.Stop();
        }

        void Update()
        {
            if (GameManager.Instance.State != GameState.Playing) return;

            MoveBodyIfNoWheelsAttached();
            // var horizontalInput = Input.GetAxis("Horizontal");
            // _currentWheelsSpeed =
            // Mathf.Lerp(_currentWheelsSpeed, -horizontalInput * _maxWheelsSpeed, _lerp * Time.deltaTime);
            // SetWheelsSpeed(_currentWheelsSpeed);
            UpdateThruster();
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.State != GameState.Playing) return;
            MoveBodyIfNoWheelsAttached();
        }

        private void MoveBodyIfNoWheelsAttached()
        {
            if (_wheelJoints.Count > 0) return;
            var direction = -Input.GetAxis("Horizontal");
            if ((direction > 0 && _rb.angularVelocity > _maxBodyAngularSpeed) ||
                (direction < 0 && _rb.angularVelocity < -_maxBodyAngularSpeed))
            {
                // don't apply torque to the same direction if already
                return;
            }

            // if ((direction > 0 && _rb.totalTorque > _maxBodyTorque) ||
            //     (direction < 0 && _rb.totalTorque < -_maxBodyTorque))
            // {
            //     return;
            // }

            _rb.totalTorque = direction * _maxBodyTorque;
        }

        public List<AttachedComponentSave> GetAttachedComponentsSave()
        {
            var savedComponents = new List<AttachedComponentSave>();
            for (var i = 0; i < _attachedComponents.Count; i++)
            {
                var parentComponentIndex = -1;
                var attachedComponent = _attachedComponents[i];

                if (attachedComponent.parent != null)
                {
                    var parentAttachedComponent =
                        _attachedComponents.Find(x => x.component == attachedComponent.parent);
                    parentComponentIndex = _attachedComponents.IndexOf(parentAttachedComponent);
                }

                var save = new AttachedComponentSave()
                {
                    key = attachedComponent.config.key,
                    id = i,
                    parentId = parentComponentIndex,
                    position = attachedComponent.position,
                    rotation = attachedComponent.rotation
                };

                savedComponents.Add(save);
            }

            return savedComponents;
        }

        public void LoadFromSaves(List<AttachedComponentSave> saves)
        {
            foreach (var attachedComponent in _attachedComponents)
            {
                Destroy(attachedComponent.component.gameObject);
            }

            _attachedComponents.Clear();
            foreach (var save in saves)
            {
                var config = GlobalConfig.Instance.AllComponents.FirstOrDefault(x => x.key == save.key);
                if (config == null)
                {
                    Debug.LogError($"Can't find component with key {save.key}");
                    continue;
                }

                BodyPlayerComponent parent = null;
                if (save.parentId >= 0)
                {
                    var parentObject = _attachedComponents[save.parentId];
                    if (parentObject.component is not BodyPlayerComponent)
                    {
                        Debug.LogError(
                            "Component in save has parent component which is not BodyPlayerComponent! Aborting.");
                        return;
                    }

                    parent = parentObject.component as BodyPlayerComponent;
                }

                AttachNewComponent(config, save.position, save.rotation, parent);
            }
        }

        public void AttachNewComponent(PlayerComponentConfig config, Vector2 position, float rotation,
            BodyPlayerComponent parent)
        {
            if (parent != null)
            {
                if (_attachedComponents.All(x => x.component != parent))
                {
                    throw new Exception("Tried to attach component to parent which is not in player");
                }
            }

            var component = Instantiate(config.prefab, transform);
            component.transform.localPosition = position.XY0();
            component.transform.localRotation = Quaternion.Euler(0, 0, rotation);

            var componentClass = component.GetComponent<PlayerComponent>();
            if (componentClass == null)
            {
                Destroy(component);
                throw new Exception("Tried to attach component without PlayerComponent");
            }

            if (parent != null)
            {
                parent.AttachedComponents.Add(componentClass);
                componentClass.Parent = parent;
            }

            var attachedComponent = new AttachedComponent()
            {
                config = config,
                component = componentClass,
                parent = parent,
                position = position,
                rotation = rotation
            };

            componentClass.Config = config;
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

                    if (componentClass is not WheelPlayerComponent wheelPlayerComponent)
                    {
                        Debug.LogError("Wheel component doesn't have WheelPlayerComponent");
                        break;
                    }

                    var wheelJoint = gameObject.AddComponent<WheelJoint2D>();
                    wheelPlayerComponent.WheelJoint2D = wheelJoint;
                    wheelJoint.connectedBody = rb;
                    wheelJoint.anchor = component.transform.localPosition.XY();
                    wheelJoint.suspension = new JointSuspension2D()
                    {
                        frequency = 10
                    };
                    wheelJoint.motor = new JointMotor2D()
                    {
                        maxMotorTorque = config.wheelMaxTorque
                    };
                    _wheelJoints.Add(wheelJoint);
                    attachedComponent.WheelJoint = wheelJoint;
                    break;
            }

            _attachedComponents.Add(attachedComponent);
        }

        public void RemoveAttachedComponent(PlayerComponent component, bool removeSelfFromParent = true)
        {
            var attachedComponent = _attachedComponents.FirstOrDefault(x => x.component == component);
            if (attachedComponent == null)
            {
                Debug.LogError("Tried to delete component which is not on the player");
                return;
            }

            if (component is BodyPlayerComponent bodyPlayerComponent)
            {
                foreach (var bodyAttachedComponent in bodyPlayerComponent.AttachedComponents)
                {
                    RemoveAttachedComponent(bodyAttachedComponent, false);
                }
            }

            if (removeSelfFromParent && component.Parent != null)
            {
                component.Parent.AttachedComponents.Remove(component);
            }

            if (attachedComponent.WheelJoint != null)
            {
                _wheelJoints.Remove(attachedComponent.WheelJoint);
                Destroy(attachedComponent.WheelJoint);
            }

            _attachedComponents.Remove(attachedComponent);
            Destroy(component.gameObject);
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

        // private void SetWheelsSpeed(float speed)
        // {
        //     foreach (var wheelJoint in _wheelJoints)
        //     {
        //         wheelJoint.useMotor = true;
        //         JointMotor2D motor = wheelJoint.motor;
        //         motor.motorSpeed = speed;
        //         wheelJoint.motor = motor;
        //     }
        // }
    }

    public class AttachedComponent
    {
        public PlayerComponentConfig config;
        public PlayerComponent component;
        public BodyPlayerComponent parent;
        public Vector2 position;
        public float rotation;

        public WheelJoint2D WheelJoint; // only for wheels
    }

    [Serializable]
    public struct AttachedComponentSave
    {
        public string key;
        public int id;
        public int parentId;
        public Vector2 position;
        public float rotation;
    }
}