// =====================================================================
//  Neon Cipher — Weather
//  File:    WeatherSystem.cs
// =====================================================================
using System;
using System.Collections.Generic;
using NeonCipher.Core;

namespace NeonCipher.TimeWeather
{
    public sealed class WeatherSystem : IWeather
    {
        private readonly IWorldClock _clock;
        private readonly List<(WeatherState s, float weight)> _rotable = new()
        {
            (WeatherState.Clear, 0.45f), (WeatherState.Overcast, 0.20f),
            (WeatherState.Rain,   0.20f), (WeatherState.HeavyRain, 0.05f),
            (WeatherState.Fog,    0.06f), (WeatherState.NeonStorm, 0.04f),
        };
        private WeatherState _current = WeatherState.Clear;
        public WeatherState Current => _current;
        public event Action<WeatherState> WeatherChanged;

        public WeatherSystem(IWorldClock clock) { _clock = clock; _clock.HourChanged += RollWeather; }

        public void SetTarget(WeatherState target, float transitionSeconds = 30f)
        {
            _current = target;
            WeatherChanged?.Invoke(target);
        }

        private float _sinceLastRoll = 0f;
        private void RollWeather(float hour)
        {
            _sinceLastRoll += 1f;
            if (_sinceLastRoll < 4f) return; // every ~4 in-game hours
            _sinceLastRoll = 0f;
            // bias by phase
            float nightBias = _clock.Phase == DayPhase.Night ? 1.5f : 0f;
            var bag = new List<(WeatherState, float)>(_rotable);
            bag.Add((WeatherState.NeonStorm, 0.06f + nightBias));
            float total = 0f; foreach (var (_, w) in bag) total += w;
            float r = (float)UnityEngine.Random.value * total, acc = 0f;
            foreach (var (s, w) in bag)
            {
                acc += w;
                if (r <= acc) { SetTarget(s, 45f); return; }
            }
        }
    }
}
