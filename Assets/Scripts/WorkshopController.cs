using System;
using System.Collections.Generic;
using System.Linq;
using Script.UI.Workshop;
using Script.Util;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Script
{
    public class WorkshopController : MonoBehaviour
    {
        // TODO JUST FOR TEST, DELETE LATER
        // [SerializeField] private List<AvailableComponent> availableComponents;
        private List<AvailableComponent> _availableComponents = new();

        public IReadOnlyList<AvailableComponent> AvailableComponents => _availableComponents;

        public AvailableComponent SelectedComponent { get; private set; }
        public GameObject SelectedComponentObject { get; private set; }

        public bool CanPlaceComponent { get; private set; }
        public string PlaceObjectError { get; private set; }

        public event Action<bool> ActiveChanged;
        public event Action<AvailableComponent> SelectedComponentChanged;
        public event Action<AvailableComponent> PlacedComponent;

        public bool Active
        {
            get => _active;
            private set
            {
                if (value == _active) return;
                _active = value;
                if (!_active)
                {
                    if (SelectedComponent != null) TrySetSelectedComponent(null);
                }
                ActiveChanged?.Invoke(value);
            }
        }

        private bool _active;

        public void SetAvailableComponents(SaveFile saveFile)
        {
            foreach (var unlockedComponent in saveFile.unlockedComponents)
            {
                var config = GlobalConfig.Instance.AllComponents.FirstOrDefault(x => x.key == unlockedComponent.key);
                if (config == null)
                {
                    Debug.LogError($"Can't find unlocked component from save file: {unlockedComponent.key}");
                }

                var attachedComponents = saveFile.player.attachedComponents.Where(x => x.key == unlockedComponent.key).ToList();
                if (attachedComponents.Count > unlockedComponent.maxCount)
                {
                    Debug.LogError($"Player has more attached components than max. Resetting attached components");
                    saveFile.player.attachedComponents.Clear();
                    attachedComponents.Clear();
                }
                _availableComponents.Add(new AvailableComponent
                {
                    componentConfig = config,
                    count = unlockedComponent.maxCount - attachedComponents.Count,
                    maxCount = unlockedComponent.maxCount
                });
            }
        }

        public void TrySetSelectedComponent(PlayerComponentConfig componentConfig)
        {
            if (SelectedComponent != null && SelectedComponentObject != null)
            {
                Destroy(SelectedComponentObject);
                SelectedComponentObject = null;
            }

            if (componentConfig == null)
            {
                SelectedComponent = null;
                SelectedComponentChanged?.Invoke(null);
                return;
            }

            var availableComponent = _availableComponents.FirstOrDefault(x => x.componentConfig == componentConfig);
            Assert.IsNotNull(availableComponent);
            if (availableComponent.count <= 0)
            {
                Debug.LogError("Tried to select a component with count = 0");
                return;
            }

            SelectedComponent = availableComponent;
            SpawnSelectedObjectPreview();
            SelectedComponentChanged?.Invoke(availableComponent);
        }

        private void SpawnSelectedObjectPreview()
        {
            SelectedComponentObject = Instantiate(SelectedComponent.componentConfig.prefab);
        }

        private void Awake()
        {
            GameManager.Instance.StateChanged += OnStateChanged;
        }

        private void Update()
        {
            if (SelectedComponent == null)
            {
                CanPlaceComponent = false;
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SelectedComponentObject.SetActive(false);
                return;
            }

            SelectedComponentObject.SetActive(true);
            var mousePos = Input.mousePosition;
            var cam = Camera.main;
            var gamePos = cam.ScreenToWorldPoint(mousePos);
            gamePos.z = 0;
            SelectedComponentObject.transform.position = gamePos;

            CheckIfCanPlaceComponent();

            if (Input.GetMouseButtonUp(0))
            {
                if (!CanPlaceComponent)
                {
                    // TODO play an error sound 
                    return;
                }

                PlaceComponent();
            }
        }

        private void CheckIfCanPlaceComponent()
        {
            CanPlaceComponent = true;
        }

        private void PlaceComponent()
        {
            var player = GameManager.Instance.Player;
            Debug.Log($"Player pos {player.transform.position}, Selected object pos {SelectedComponentObject.transform.position}");
            GameManager.Instance.Player.AttachNewComponent(new AttachedComponentConfig()
            {
                component = SelectedComponent.componentConfig,
                position = SelectedComponentObject.transform.position.XY() -
                           GameManager.Instance.Player.transform.position.XY(),
                rotation = SelectedComponentObject.transform.rotation.z -
                           GameManager.Instance.Player.transform.rotation.z
            }, true);

            SelectedComponent.count--;
            PlacedComponent?.Invoke(SelectedComponent);
            if (SelectedComponent.count == 0)
            {
                TrySetSelectedComponent(null);
            }
        }

        private void OnStateChanged(GameState oldState, GameState newState)
        {
            if (oldState == GameState.Workshop)
            {
                Active = false;
            }

            if (newState == GameState.Workshop)
            {
                Active = true;
            }
        }
    }
}