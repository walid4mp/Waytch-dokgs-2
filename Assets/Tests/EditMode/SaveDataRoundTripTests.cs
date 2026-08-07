// =====================================================================
//  Neon Cipher — Edit-mode tests
//  File:    SaveDataRoundTripTests.cs
//  Notes:   Engine-free where possible. Uses NUnit + Unity TestRunner.
// =====================================================================
using NUnit.Framework;
using NeonCipher.Core;
using NeonCipher.Localization;
using NeonCipher.Mission;
using NeonCipher.Saving;
using UnityEngine;

namespace NeonCipher.Tests
{
    public sealed class SaveDataRoundTripTests
    {
        [Test]
        public void Save_and_Load_roundtrips_through_disk()
        {
            var settings = new GameSettingsService { Language = "en" };
            var save = new SaveSystem(settings);
            // ensure clean slot
            save.Delete(1);

            var data = new SaveData
            {
                SceneName = "TestScene",
                Language  = "en",
                Player = new PlayerSaveState { PosX = 12.4f, PosY = 1.0f, PosZ = -7.8f, Money = 999 }
            };

            Assert.IsTrue(save.Save(1, data), "Save should succeed");
            Assert.IsTrue(save.Load(1, out var loaded), "Load should succeed");
            Assert.AreEqual("TestScene", loaded.SceneName);
            Assert.AreEqual(999, loaded.Player.Money);
            Assert.AreEqual(12.4f, loaded.Player.PosX, 0.001f);
        }

        [Test]
        public void Localization_falls_back_to_English_for_missing_key()
        {
            var loc = new LocalizationService();
            loc.SetLanguage("en");
            Assert.AreEqual("Neon Cipher", loc.T("app.title"));
            Assert.AreEqual("#missing", loc.T("does.not.exist"));
        }

        [Test]
        public void Mission_progress_advances_and_rewards()
        {
            var mc = new MissionController();
            var inv = new InventoryService();
            mc.AttachInventory(inv);
            var mission = ScriptableObject.CreateInstance<MissionSO>();
            mission.Id = "first_run";
            mission.RewardMoney = 250; mission.RewardXp = 50;
            mission.Steps.Add(new MissionSO.Step { Id = "s1" });
            mission.Steps.Add(new MissionSO.Step { Id = "s2" });
            mc.Register(mission);

            Assert.IsTrue(mc.Start("first_run"));
            mc.ProgressToNextStep();
            Assert.AreEqual("active",  mc.All[0].Status);
            mc.ProgressToNextStep();
            Assert.AreEqual("complete", mc.All[0].Status);
            Assert.AreEqual(750, inv.Money); // 500 start + 250 reward
            Assert.AreEqual(50,  inv.Xp);
        }
    }
}
