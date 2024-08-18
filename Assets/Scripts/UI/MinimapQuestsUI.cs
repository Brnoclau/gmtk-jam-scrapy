using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Scrapy.UI
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class MinimapQuestsUI : FadeCanvas
    {
        [SerializeField] private QuestLineUI questLinePrefab;
        [SerializeField] private RectTransform questLinesContainer;
        
        private readonly List<QuestLineUI> _questLines = new();
        
        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance.StateChanged += OnStateChanged;
            QuestManager.Instance.QuestCompleted += OnQuestCompleted;
        }

        private void Start()
        {
            UpdateQuests();
        }

        private void OnQuestCompleted(Quest _)
        {
            UpdateQuests();
        }

        public void UpdateQuests()
        {
            foreach (var questLineUI in _questLines)
            {
                Destroy(questLineUI.gameObject);
            }
            
            _questLines.Clear();
            foreach (var quest in QuestManager.Instance.GetActiveQuests())
            {
                var questLine = Instantiate(questLinePrefab, questLinesContainer);
                questLine.descriptionText.text = quest.description;
                _questLines.Add(questLine);
            }
        }

        private void OnStateChanged(GameState oldState, GameState newState)
        {
            if (oldState == GameState.Playing)
            {
                SetOpen(false);
            }
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing) return;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetOpen(true);
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                SetOpen(false);
            }
        }
    }
}