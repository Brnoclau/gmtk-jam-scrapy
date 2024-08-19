using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapy.Player
{
    public abstract class ActionPlayerComponent : PlayerComponent
    {
        // Set from player
        [NonSerialized] public ActionHotkey Hotkey;

        public event Action<bool> ActiveChanged;

        public bool IsInputActive { get; private set; }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                if (Config.actionMode == ActionMode.Activate && !_isActive) return;
                ActiveChanged?.Invoke(_isActive);
            }
        }

        private bool _isActive;

        private Coroutine _activateCoroutine = null;
        private float _lastUsedAt = 0;
        private float _charge = 0;
        private bool _chargeFinishedDuringThisInput = false;

        protected virtual void Start()
        {
            _charge = Config.chargeSec;
            Debug.Log("Hotkey " + Hotkey);
            ActiveChanged += (val) => Debug.Log("Active " + val);
        }

        protected virtual void Update()
        {
            if (Config.actionMode == ActionMode.Activate)
            {
                if (Input.GetKeyDown(HotkeyToKeycode(Hotkey)))
                {
                    Activate();
                }
            }

            if (Config.actionMode == ActionMode.Hold)
            {
                IsInputActive = Input.GetKey(HotkeyToKeycode(Hotkey));
            }

            if (Config.actionMode == ActionMode.Toggle)
            {
                if (Input.GetKeyDown(HotkeyToKeycode(Hotkey)))
                {
                    IsInputActive = !IsInputActive;
                }
            }

            if (Config.actionMode is ActionMode.Toggle or ActionMode.Hold)
            {
                if (!Config.usesCharge)
                {
                    IsActive = IsInputActive;
                    return;
                }

                if (IsInputActive && (!Config.usesCharge || _charge > 0) && !_chargeFinishedDuringThisInput)
                {
                    IsActive = true;
                    _charge -= Time.deltaTime;
                    if (_charge < 0)
                    {
                        _charge = 0;
                        if (Config.actionMode == ActionMode.Hold) _chargeFinishedDuringThisInput = true;
                        else if (Config.actionMode == ActionMode.Toggle) IsInputActive = false;
                    }

                    _lastUsedAt = Time.time;
                }
                else
                {
                    IsActive = false;

                    if (_chargeFinishedDuringThisInput && !IsInputActive)
                    {
                        _chargeFinishedDuringThisInput = false;
                    }

                    if (Time.time - _lastUsedAt > Config.chargeReloadDelay)
                    {
                        _charge += Config.chargeReloadPerSec * Time.deltaTime;
                        if (_charge > Config.chargeSec) _charge = Config.chargeSec;
                    }
                }
            }
        }

        public virtual void Activate()
        {
            if (IsActive) return;
            if (_lastUsedAt != 0 && _lastUsedAt + Config.activationCooldown > Time.time)
            {
                return;
            }

            _activateCoroutine = StartCoroutine(ActiveCoroutine());
        }

        public virtual IEnumerator ActiveCoroutine()
        {
            IsActive = true;
            yield return new WaitForSeconds(Config.activeTime);
            IsActive = false;
            _lastUsedAt = Time.time;
            _activateCoroutine = null;
        }

        private static KeyCode HotkeyToKeycode(ActionHotkey hotkey)
        {
            return hotkey switch
            {
                ActionHotkey.Q => KeyCode.Q,
                ActionHotkey.W => KeyCode.W,
                ActionHotkey.E => KeyCode.E,
                ActionHotkey.R => KeyCode.R,
                ActionHotkey.Key1 => KeyCode.Alpha1,
                ActionHotkey.Key2 => KeyCode.Alpha2,
                ActionHotkey.Key3 => KeyCode.Alpha3,
                ActionHotkey.Key4 => KeyCode.Alpha4,
                ActionHotkey.Key5 => KeyCode.Alpha5,
                _ => throw new Exception("Unsupported action hotkey " + hotkey)
            };
        }
    }

    public enum ActionHotkey
    {
        Q,
        W,
        E,
        R,
        Key1,
        Key2,
        Key3,
        Key4,
        Key5
    }
}