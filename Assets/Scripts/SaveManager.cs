using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Scrapy
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public SaveFile CurrentSave { get; set; }

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Removing extra SaveManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (transform.parent) DontDestroyOnLoad(transform.parent.gameObject);
            else DontDestroyOnLoad(gameObject);
        }

        public void ResetSave()
        {
            var savePath = GetSavePath();
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
        }

        public void SaveGame()
        {
            var gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError("Can't save game without game manager in the scene");
                return;
            }

            CurrentSave.player = new PlayerSaveData();
            CurrentSave.player.attachedComponents = gameManager.Player.GetAttachedComponentsSave();
            // Workshop gets what components are unlocked, not the opposite
            // CurrentSave.unlockedComponents = gameManager.WorkshopController.AvailableComponents.Select(x =>
            // {
            //     return new UnlockedComponentData()
            //     {
            //         key = x.componentConfig.key,
            //         maxCount = x.maxCount
            //     };
            // }).ToList();

            var json = JsonUtility.ToJson(CurrentSave, true);
            Debug.Log("Save file json: \n" + json);
            var savePath = GetSavePath();
            Debug.Log("Save path: " + savePath);
            try
            {
                File.WriteAllText(savePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save json file: " + e);
                return;
            }

            Debug.Log("Successfully saved the game");
        }

        public bool SaveFileExists()
        {
            var savePath = GetSavePath();
            return File.Exists(savePath);
        }

        public SaveFile LoadGame()
        {
            var savePath = GetSavePath();
            if (!File.Exists(savePath))
            {
                Debug.Log("No save file, using default save");
                CurrentSave = SaveFile.Default();
                return CurrentSave;
            }

            var saveJson = File.ReadAllText(savePath);
            try
            {
                CurrentSave = JsonUtility.FromJson<SaveFile>(saveJson);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to deserialize save file. Returning new save\n" + e);
                CurrentSave = SaveFile.Default();
            }

            return CurrentSave;
        }

        private string GetSavePath()
        {
            return Application.persistentDataPath + "/save.json";
        }
    }
}