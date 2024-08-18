using TMPro;
using UnityEngine;

namespace Scrapy.UI
{
    public class CompleteQuestsUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text questName;
        [SerializeField] private ParticleSystem particleSystem;
        
        private static readonly int PlayAnimatorProp = Animator.StringToHash("play");

        protected void Awake()
        {
            QuestManager.Instance.QuestCompleted += OnQuestCompleted;
        }

        private void OnQuestCompleted(Quest quest)
        {
            if (!quest.playCompleteQuestAnimation) return;
            questName.text = quest.name;
            animator.SetTrigger(PlayAnimatorProp);
        }

        // Used by animator
        public void PlayParticles()
        {
            particleSystem.Play();
        }
    }
}