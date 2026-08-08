// Neon Cipher — World Clock (day/night) + runtime driver
using System;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.TimeWeather
{
    public sealed class WorldClock : IWorldClock
    {
        public float CurrentTime { get; private set; } = 6.5f;
        public float DayLengthSeconds { get; set; } = 1440f;
        public DayPhase Phase { get; private set; } = DayPhase.Dawn;
        public event Action<float> HourChanged;
        public event Action<DayPhase> PhaseChanged;
        private float _lastHourBucket = -1f;
        private DayPhase _lastPhase = DayPhase.Dawn;

        public void Tick(float dt)
        {
            if (DayLengthSeconds < 1f) DayLengthSeconds = 1f;
            CurrentTime = (CurrentTime + dt / DayLengthSeconds * 24f) % 24f;
            float bucket = Mathf.Floor(CurrentTime);
            if (!Mathf.Approximately(bucket, _lastHourBucket))
            { _lastHourBucket = bucket; HourChanged?.Invoke(CurrentTime); }
            var next = Classify(CurrentTime);
            if (next != _lastPhase) { _lastPhase = next; Phase = next; PhaseChanged?.Invoke(next); }
        }
        public static DayPhase Classify(float h) =>
            h < 5.5f || h >= 21.5f ? DayPhase.Night : h < 7.5f ? DayPhase.Dawn : h < 18.5f ? DayPhase.Day : DayPhase.Dusk;
    }

    public sealed class WorldClockDriver : MonoBehaviour
    {
        private IWorldClock _clock;
        private void Update()
        {
            if (_clock == null && GameServices.Current != null) GameServices.Current.TryGet(out _clock);
            _clock?.Tick(Time.deltaTime);
        }
    }
}
