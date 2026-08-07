// =====================================================================
//  Neon Cipher — World Clock (day/night)
//  File:    WorldClock.cs
//  Notes:   24 in-game hours = DayLengthSeconds real seconds (default 1440
//           → 1 minute per in-game minute). Hooks the sun + skybox material.
// =====================================================================
using System;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.TimeWeather
{
    public enum DayPhase { Dawn = 0, Day = 1, Dusk = 2, Night = 3 }

    public sealed class WorldClock : IWorldClock
    {
        public float CurrentTime { get; private set; } = 6.5f; // 06:30
        public float DayLengthSeconds { get; set; } = 1440f;   // 24 min / day
        public DayPhase Phase { get; private set; } = DayPhase.Dawn;
        public event Action<float> HourChanged;
        public event Action<DayPhase> PhaseChanged;

        private float _lastHourBucket = -1;
        private DayPhase _lastPhase;

        public void Tick(float dt)
        {
            CurrentTime = (CurrentTime + dt / DayLengthSeconds * 24f) % 24f;
            float bucket = Mathf.Floor(CurrentTime);
            if (!Mathf.Approximately(bucket, _lastHourBucket))
            {
                _lastHourBucket = bucket;
                HourChanged?.Invoke(CurrentTime);
            }
            var next = Classify(CurrentTime);
            if (next != _lastPhase)
            {
                _lastPhase = next;
                Phase = next;
                PhaseChanged?.Invoke(next);
            }
        }

        public static DayPhase Classify(float hour) =>
            hour < 5.5f || hour >= 21.5f ? DayPhase.Night
                : hour < 7.5f  ? DayPhase.Dawn
                : hour < 18.5f ? DayPhase.Day
                : DayPhase.Dusk;
    }
}
