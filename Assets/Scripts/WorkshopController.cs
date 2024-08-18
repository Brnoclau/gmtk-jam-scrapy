using System;
using System.Collections.Generic;
using System.Linq;
using Scrapy.Player;
using Scrapy.Util;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Scrapy
{
    public class WorkshopController : MonoBehaviour
    {
        [SerializeField] private LayerMask playerComponentsLayerMask;
        [SerializeField] private LayerMask playerOnlyBodyLayerMask;
        [SerializeField] private Color hoveredColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color cantPlaceColor;
        [SerializeField] private Color canPlaceColor;
        [SerializeField] private SpriteRenderer placementAreaSprite;
        [SerializeField] private Vector2 placementAreaCenter;
        [SerializeField] private Vector2 placementAreaSize;

        private List<AvailableComponent> _availableComponents = new();

        public IReadOnlyList<AvailableComponent> AvailableComponents => _availableComponents;

        public AvailableComponent AddingComponent { get; private set; }
        public GameObject AddingComponentObject { get; private set; }

        public PlayerComponent SelectedComponent
        {
            get => _selectedComponent;
            private set
            {
                if (_selectedComponent == value) return;
                if (_selectedComponent != null) SetComponentSelected(_selectedComponent, false);
                _selectedComponent = value;
                if (_selectedComponent != null) SetComponentSelected(_selectedComponent, true);
                SelectedComponentChanged?.Invoke(_selectedComponent);
            }
        }

        private PlayerComponent _selectedComponent;

        public PlayerComponent HoveredComponent
        {
            get => _hoveredComponent;
            private set
            {
                if (_hoveredComponent == value) return;
                if (_hoveredComponent != null) SetComponentHovered(_hoveredComponent, false);
                _hoveredComponent = value;
                if (_hoveredComponent != null) SetComponentHovered(_hoveredComponent, true);
                HoveredComponentChanged?.Invoke(_hoveredComponent);
            }
        }

        private PlayerComponent _hoveredComponent;

        private Vector2 PlacementAreaCenter;

        private Vector2 PlacementAreaSize; 
        private Rect PlacementAreaRect; 

        public bool CanPlaceComponent { get; private set; }
        public BodyPlayerComponent AttachmentParent { get; private set; }
        public string PlaceObjectError { get; private set; }

        public event Action<bool> ActiveChanged;
        public event Action<AvailableComponent> AddingComponentChanged;
        public event Action<PlayerComponent> HoveredComponentChanged;
        public event Action<PlayerComponent> SelectedComponentChanged;
        public event Action<WorkshopMode> ModeChanged;
        public event Action<AvailableComponent> AvailableComponentChanged;
        public event Action AvailableComponentsChanged;

        public enum WorkshopMode
        {
            Selection,
            Adding,
        }

        public WorkshopMode Mode
        {
            get => _mode;
            private set
            {
                if (_mode == value) return;
                _mode = value;
                Debug.Log($"Changed mode to {_mode}");
                ModeChanged?.Invoke(_mode);
            }
        }

        private WorkshopMode _mode;

        public bool Active
        {
            get => _active;
            private set
            {
                if (value == _active) return;
                _active = value;
                if (_active)
                {
                    UpdateAvailableComponents();
                    UpdatePlacementReact();
                
                    placementAreaSprite.gameObject.SetActive(true);
                    placementAreaSprite.transform.position = PlacementAreaCenter;
                    placementAreaSprite.size = PlacementAreaSize;
                    
                    Mode = WorkshopMode.Selection;
                }
                else
                {
                    placementAreaSprite.gameObject.SetActive(false);
                    if (AddingComponent != null) SetAddingComponent(null);
                }

                ActiveChanged?.Invoke(value);
            }
        }

        private bool _active;
        private Collider2D[] _raycastResults = new Collider2D[10];

        private void Awake()
        {
            GameManager.Instance.StateChanged += OnStateChanged;
            placementAreaSprite.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!Active) return;
            switch (Mode)
            {
                case WorkshopMode.Selection:
                    UpdateHoverAndSelection();
                    break;
                case WorkshopMode.Adding:
                    UpdateAdding();
                    break;
            }
        }

        public void DeleteSelectedComponent()
        {
            GameManager.Instance.Player.RemoveAttachedComponent(SelectedComponent);
            UpdateAvailableComponents();
            AvailableComponentsChanged?.Invoke();
            SelectedComponent = null;
        }

        private void UpdateHoverAndSelection()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                HoveredComponent = null;
                return;
            }

            var mousePos = Input.mousePosition;
            var gameMousePos = Camera.main.ScreenToWorldPoint(mousePos).XY();
            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = playerComponentsLayerMask
            };
            var collidersCount = Physics2D.OverlapPoint(gameMousePos, filter, _raycastResults);

            PlayerComponent newHoveredComponent = null;
            if (collidersCount > 0)
            {
                var components = new List<PlayerComponent>(collidersCount);
                for (int i = 0; i < collidersCount; i++)
                {
                    var component = _raycastResults[i];
                    var playerComponent = component.GetComponentInParent<PlayerComponent>();
                    if (!playerComponent) continue;
                    components.Add(playerComponent);
                }

                newHoveredComponent = components.FirstOrDefault(x => x != SelectedComponent);
            }

            if (newHoveredComponent != HoveredComponent)
            {
                HoveredComponent = newHoveredComponent;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedComponent == HoveredComponent) return; // Shouldn't be the case anyway
                var comp = HoveredComponent;
                HoveredComponent = null;
                SelectedComponent = comp;
            }
        }

        private void UpdateAdding()
        {
            if (AddingComponent == null)
            {
                CanPlaceComponent = false;
                Mode = WorkshopMode.Selection;
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                AddingComponentObject.SetActive(false);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetAddingComponent(null);
                return;
            }

            AddingComponentObject.SetActive(true);
            var mousePos = Input.mousePosition;
            var cam = Camera.main;
            var gamePos = cam.ScreenToWorldPoint(mousePos);
            gamePos.z = 0;
            AddingComponentObject.transform.position = gamePos;

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

        private void SetComponentHovered(PlayerComponent component, bool value)
        {
            var sprites = component.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in sprites)
            {
                spriteRenderer.color = value ? hoveredColor : Color.white;
            }
        }

        private void SetComponentSelected(PlayerComponent component, bool value)
        {
            var sprites = component.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in sprites)
            {
                spriteRenderer.color = value ? selectedColor : Color.white;
            }
        }

        // public void SetAvailableComponents(SaveFile saveFile)
        // {
        //     _availableComponents.Clear();
        //     foreach (var unlockedComponent in saveFile.unlockedComponents)
        //     {
        //         var config = GlobalConfig.Instance.AllComponents.FirstOrDefault(x => x.key == unlockedComponent.key);
        //         if (config == null)
        //         {
        //             Debug.LogError($"Can't find unlocked component from save file: {unlockedComponent.key}");
        //         }
        //
        //         var attachedComponents = saveFile.player.attachedComponents.Where(x => x.key == unlockedComponent.key)
        //             .ToList();
        //         if (attachedComponents.Count > unlockedComponent.maxCount)
        //         {
        //             Debug.LogError($"Player has more attached components than max. Resetting attached components");
        //             saveFile.player.attachedComponents.Clear();
        //             attachedComponents.Clear();
        //         }
        //
        //         _availableComponents.Add(new AvailableComponent
        //         {
        //             componentConfig = config,
        //             count = unlockedComponent.maxCount - attachedComponents.Count,
        //             maxCount = unlockedComponent.maxCount
        //         });
        //     }
        // }

        private void UpdateAvailableComponents()
        {
            var saveFile = SaveManager.Instance.CurrentSave;
            _availableComponents.Clear();
            foreach (var unlockedComponent in saveFile.unlockedComponents)
            {
                var config = GlobalConfig.Instance.AllComponents.FirstOrDefault(x => x.key == unlockedComponent.key);
                if (config == null)
                {
                    Debug.LogError($"Can't find unlocked component from save file: {unlockedComponent.key}");
                }

                var attachedComponents = GameManager.Instance.Player.AttachedComponents
                    .Where(x => x.config.key == unlockedComponent.key)
                    .ToList();
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

        public void SetAddingComponent(PlayerComponentConfig componentConfig)
        {
            if (AddingComponent != null && AddingComponentObject != null)
            {
                Destroy(AddingComponentObject);
                AddingComponentObject = null;
            }

            if (componentConfig == null)
            {
                AddingComponent = null;
                AddingComponentChanged?.Invoke(null);
                Mode = WorkshopMode.Selection;
                return;
            }

            var availableComponent = _availableComponents.FirstOrDefault(x => x.componentConfig == componentConfig);
            Assert.IsNotNull(availableComponent);
            if (availableComponent.count <= 0)
            {
                Debug.LogError("Tried to select a component with count = 0");
                Mode = WorkshopMode.Selection;
                return;
            }

            if (AddingComponent == availableComponent)
            {
                AddingComponent = null;
                AddingComponentChanged?.Invoke(null);
                Mode = WorkshopMode.Selection;
                return;
            }

            AddingComponent = availableComponent;
            SpawnSelectedObjectPreview();
            Mode = WorkshopMode.Adding;
            AddingComponentChanged?.Invoke(availableComponent);
        }

        private void SpawnSelectedObjectPreview()
        {
            AddingComponentObject = Instantiate(AddingComponent.componentConfig.prefab);
            LayerUtility.SetGameLayerRecursive(AddingComponentObject, LayerMask.NameToLayer("PlacementPreview"));
            foreach (var rb in AddingComponentObject.GetComponentsInChildren<Rigidbody2D>())
            {
                rb.simulated = false;
            }
        }

        private void CheckIfCanPlaceComponent()
        {
            var colliders = AddingComponentObject.GetComponentsInChildren<Collider2D>();
            foreach (var collider in colliders)
            {
                Debug.DrawLine(PlacementAreaRect.min, PlacementAreaRect.max, Color.blue);
                if (!PlacementAreaRect.Contains(collider.bounds.min) ||
                    !PlacementAreaRect.Contains(collider.bounds.max))
                {
                    Debug.DrawLine(collider.bounds.min, collider.bounds.max, Color.red);
                    CanPlaceComponent = false;
                    PlaceObjectError = "Component is outside of placement area";
                    return;
                }

                Debug.DrawLine(collider.bounds.min, collider.bounds.max, Color.green);
            }

            var result = AddingComponentObject.GetComponent<PlayerComponent>().CanAttachToPlayer();
            CanPlaceComponent = result.Value;
            PlaceObjectError = result.Error;
            AttachmentParent = result.Parent;

            foreach (var spriteRenderer in AddingComponentObject.GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.color = CanPlaceComponent ? canPlaceColor : cantPlaceColor;
            }
        }

        private void PlaceComponent()
        {
            Transform targetTransform = GameManager.Instance.Player.transform;
            GameManager.Instance.Player.AttachNewComponent(AddingComponent.componentConfig,
                AddingComponentObject.transform.position.XY() - targetTransform.position.XY(),
                AddingComponentObject.transform.rotation.z - targetTransform.rotation.z,
                AttachmentParent);

            GameManager.Instance.FreezePlayer();

            AddingComponent.count--;
            AvailableComponentChanged?.Invoke(AddingComponent);
            if (AddingComponent.count == 0)
            {
                SetAddingComponent(null);
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

        private void UpdatePlacementReact()
        {
            PlacementAreaCenter = placementAreaCenter + (GameManager.Instance.CurrentWorkshopArea != null
                ? GameManager.Instance.CurrentWorkshopArea.transform.position.XY()
                : Vector2.zero);

            PlacementAreaSize = placementAreaSize;
            PlacementAreaRect = new Rect(PlacementAreaCenter - placementAreaSize / 2, placementAreaSize);
        }
    }
}