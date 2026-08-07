// =====================================================================
//  Neon Cipher — Save Data DTO
// =====================================================================
using System.Collections.Generic;

namespace NeonCipher.Core
{
    /// <summary>Pure-data DTO. Round-trips through JSON. No engine refs.</summary>
    [System.Serializable]
    public sealed class SaveData
    {
        public string Version = "0.1.0";
        public string SavedAtIso;
        public string SceneName;
        public PlayerSaveState Player = new();
        public List<MissionSaveState> Missions = new();
        public List<InventoryEntry> Inventory = new();
        public WorldState World = new();
        public string Language = "en";
    }

    [System.Serializable]
    public sealed class PlayerSaveState
    {
        public float PosX, PosY, PosZ;
        public float RotY;
        public int Health = 100;
        public int WantedLevel;
        public int Money;
    }

    [System.Serializable]
    public sealed class MissionSaveState
    {
        public string Id;
        public string Status; // available | active | complete | failed
        public int StepIndex;
    }

    [System.Serializable]
    public sealed class InventoryEntry
    {
        public string Id;
        public int Count;
    }

    [System.Serializable]
    public sealed class WorldState
    {
        public float TimeOfDayHours;
        public WeatherState Weather;
    }
}
