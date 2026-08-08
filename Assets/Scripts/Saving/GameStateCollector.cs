// Neon Cipher — Collect & Apply runtime state <-> SaveData
using NeonCipher.Core;
using NeonCipher.Inventory;
using NeonCipher.Mission;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeonCipher.Saving
{
    public static class GameStateCollector
    {
        public static SaveData Collect()
        {
            var gs = GameServices.Current;
            var data = new SaveData { SceneName = SceneManager.GetActiveScene().name, Language = gs.Get<ILocalization>().CurrentLanguage };
            if (gs.TryGet<IWorldClock>(out var clock) && gs.TryGet<IWeather>(out var weather))
                data.World = new WorldState { TimeOfDayHours = clock.CurrentTime, Weather = weather.Current };
            var inv = gs.TryGet<IInventory>(out var invSvc) ? invSvc : null;
            var player = Object.FindObjectOfType<Player.PlayerController>();
            if (player != null)
                data.Player = new PlayerSaveState
                { PosX = player.transform.position.x, PosY = player.transform.position.y, PosZ = player.transform.position.z, RotY = player.transform.eulerAngles.y, Money = inv?.Money ?? 0 };
            if (inv != null) foreach (var e in inv.Entries) data.Inventory.Add(new InventoryEntry { Id = e.Id, Count = e.Count });
            if (gs.TryGet<MissionController>(out var m))
                foreach (var r in m.All) data.Missions.Add(new MissionSaveState { Id = r.Data.Id, Status = r.Status, StepIndex = r.StepIndex });
            return data;
        }
    }

    public static class GameStateApplier
    {
        private static SaveData _pending;
        public static void Apply(SaveData data)
        {
            if (data == null) return;
            var gs = GameServices.Current;
            gs.Get<ILocalization>().SetLanguage(data.Language);
            if (gs.TryGet<IWeather>(out var w)) w.SetTarget(data.World.Weather, 8f);
            SceneManager.LoadScene(data.SceneName, LoadSceneMode.Single);
            SceneManager.sceneLoaded += SceneReady;
            _pending = data;
        }
        private static void SceneReady(UnityEngine.SceneManagement.Scene s, LoadSceneMode m)
        {
            var data = _pending;
            if (data == null || s.name != data.SceneName) return;
            SceneManager.sceneLoaded -= SceneReady; _pending = null;
            var p = Object.FindObjectOfType<Player.PlayerController>();
            if (p != null) p.transform.position = new Vector3(data.Player.PosX, data.Player.PosY, data.Player.PosZ);
            if (GameServices.Current.TryGet<IInventory>(out var inv)) foreach (var e in data.Inventory) inv.Add(e.Id, e.Count);
        }
    }
}
