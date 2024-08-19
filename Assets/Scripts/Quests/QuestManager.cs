using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Scrapy
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestsConfig questsConfig;
        public static QuestManager Instance { get; private set; }

        public event Action<string> CheckpointCompleted;
        public event Action<Quest> QuestCompleted;

        public IReadOnlyList<string> CompleteCheckpoints => SaveManager.Instance.CurrentSave.reachedCheckpoints;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There should only be one QuestManager in the scene");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void LoadFromSave()
        {
            foreach (var quest in questsConfig.quests)
            {
                if (CompleteCheckpoints.Contains(quest.checkpointKey))
                {
                    quest.complete = true;
                }
            }
        }

        public void CompleteCheckpoint(string checkpoint)
        {
            if (string.IsNullOrWhiteSpace(checkpoint))
            {
                Debug.LogWarning("Collected empty checkpoint");
                return;
            }
            
            var save = SaveManager.Instance.CurrentSave;
            if (save.reachedCheckpoints.Contains(checkpoint)) return;

            save.reachedCheckpoints.Add(checkpoint);
            CheckpointCompleted?.Invoke(checkpoint);
            SaveManager.Instance.SaveGame();

            if (questsConfig.showCreditsAfterCheckpointKey == checkpoint)
            {
                GameManager.Instance.State = GameState.Credits;
            }

            var quest = questsConfig.quests.FirstOrDefault(x => x.checkpointKey == checkpoint);
            if (quest == null) return;
            CompleteQuest(quest);
        }

        public List<Quest> GetActiveQuests()
        {
            return questsConfig.quests.Where(quest => !quest.complete && IsQuestUnlocked(quest)).ToList();
        }

        public List<Quest> GetUnlockedQuests()
        {
            return questsConfig.quests.Where(IsQuestUnlocked).ToList();
        }

        public bool IsQuestUnlocked(Quest quest)
        {
            return quest.activeWhenCheckpointsComplete.Count == 0 ||
                   quest.activeWhenCheckpointsComplete.All(requiredCheckpoint =>
                       CompleteCheckpoints.Contains(requiredCheckpoint));
        }

        private void CompleteQuest(Quest quest)
        {
            quest.complete = true;
            QuestCompleted?.Invoke(quest);
        }
    }
}