using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Scrapy
{
    public class MultiDialogObject : Interactable
    {
        [SerializeField] private DialogConfig defaultDialogConfig;
        [SerializeField] private List<MultiDialogOption> options;
        
        public override void Interact()
        {
            var dialog = GetCurrentOption();
            if (dialog != null)
            {
                DialogManager.Instance.StartDialog(dialog.dialogConfig);
            }
            else
            {
                DialogManager.Instance.StartDialog(defaultDialogConfig);
            }
        }

        public MultiDialogOption GetCurrentOption()
        {
            // from last to first check required checkpoints
            for (int i = options.Count - 1; i >= 0; i--)
            {
                var option = options[i];
                if (option.requiredCheckpoints.Count == 0)
                {
                    return option;
                }
                bool allRequired = true;
                foreach (var checkpoint in option.requiredCheckpoints)
                {
                    if (!QuestManager.Instance.CompleteCheckpoints.Contains(checkpoint))
                    {
                        allRequired = false;
                        break;
                    }
                }

                if (allRequired)
                {
                    return option;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class MultiDialogOption
    {
        public List<string> requiredCheckpoints;
        public DialogConfig dialogConfig;
    }
}