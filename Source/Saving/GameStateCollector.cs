// =====================================================================
//  Neon Cipher — Collect & Apply runtime state <-> SaveData
// =====================================================================
using System.Collections.Generic;
using NeonCipher.Core;
using NeonCipher.Inventory;
using NeonCipher.Mission;
using NeonCipher.TimeWeather;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeonCipher.Saving
{
    /// <summary>Snapshot builders: keeps gameplay code unaware of JSON shape.</summary>
    public static class GameStateCollector
    {
        public static SaveData Collect()
        {
            var gs     = GameServices.Current;
            var clock  = gs.Get<IWorldClock>();
            var weather= gs.Get<IWeather>();
            var inv    = gs.Get<IInventory>();
            var missions = gs.Get<MissionController>();
            var player = Object.FindObjectOfType<Player.PlayerController>();

            var data = new SaveData
            {
                SceneName = SceneManager.GetActiveScene().name,
                Language  = gs.Get<ILocalization>().CurrentLanguage,
                World     = new WorldState { TimeOfDayHours = clock.CurrentTime, Weather = weather.Current }
            };

            if (player != null)
            {
                data.Player = new PlayerSaveState
                {
                    PosX = player.transform.position.x, PosY = player.transform.position.y, PosZ = player.transform.position.z,
                    RotY = player.transform.eulerAngles.y,
                    Money = inv.Money
                };
            }

            foreach (var kv in inv.Entries)
                data.Inventory.Add(new InventoryEntry { Id = kv.Id, Count = kv.Count });

            foreach (var r in missions.All)
                data.Missions.Add(new MissionSaveState { Id = r.Data.Id, Status = r.Status, StepIndex = r.StepIndex });
            return data;
        }
    }

    public static class GameStateApplier
    {
        public static void Apply(SaveData data)
        {
            var gs = GameServices.Current;
            gs.Get<ILocalization>().SetLanguage(data.Language);
            gs.Get<IWeather>().SetTarget(data.World.Weather, 8f);
            // WorldClock is read-only here — load scene instead
            SceneManager.LoadScene(data.SceneName, LoadSceneMode.Single);
            // OnSceneLoaded callback hook touches player position + mission status
            SceneManager.sceneLoaded += (s, m) =>
            {
                if (s.name != data.SceneName) return;
                var p = Object.FindObjectOfType<Player.PlayerController>();
                if (p != null) p.transform.position = new Vector3(data.Player.PosX, data.Player.PosY, data.Player.PosZ);
                var inv = gs.Get<IInventory>();
                foreach (var e in data.Inventory) inv.Add(e.Id, e.Count);
                var ms = gs.Get<MissionController>();
                foreach (var s2 in data.Missions)
                {
                    ms.Register(ScriptableObject.CreateInstance<MissionSO>());
                    // runtime resume minimal — gameplay reloads by id from MissionCatalog asset
                }
            };
        }
    }
}
