using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script
{
    public class PlayerConfig
    {
        public List<AttachedComponentConfig> attachedComponents = new();

        public List<AttachedComponentSave> ToSaves()
        {
            var savedComponents = new List<AttachedComponentSave>();
            foreach (var attachedComponent in attachedComponents)
            {
                Debug.Log("Saving component " + attachedComponent.component.key);
                var save = new AttachedComponentSave()
                {
                    key = attachedComponent.component.key,
                    position = attachedComponent.position,
                    rotation = attachedComponent.rotation
                };
                
                savedComponents.Add(save);
            }

            return savedComponents;
        }

        public void LoadFromSaves(List<AttachedComponentSave> saves)
        {
            // List<AttachedComponentSave> saves;
            // try
            // {
            //     saves = JsonUtility.FromJson<List<AttachedComponentSave>>(saveString);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"Failed to deserialize player components from save string: {saveString}");
            //     Debug.LogException(e);
            //     return;
            // }

            attachedComponents.Clear();
            foreach (var save in saves)
            {
                var config = GlobalConfig.Instance.AllComponents.FirstOrDefault(x => x.key == save.key);
                if (config == null)
                {
                    Debug.LogError($"Can't find component with key {save.key}");
                    continue;
                }
                attachedComponents.Add(new AttachedComponentConfig()
                {
                    component = config,
                    position = save.position,
                    rotation = save.rotation
                });
            }
        }
    }
    
    [Serializable]
    public struct AttachedComponentConfig
    {
        public PlayerComponentConfig component;
        public Vector2 position;
        public float rotation;
    }
}

[Serializable]
public struct AttachedComponentSave
{
    public string key;
    public Vector2 position;
    public float rotation;
}