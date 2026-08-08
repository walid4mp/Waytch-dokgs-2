// Neon Cipher — Audio Bus (IAudioBus implementation; AudioBusKind enum)
using System.Collections.Generic;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Audio
{
    public sealed class AudioBus : IAudioBus
    {
        private readonly Dictionary<AudioBusKind, float> _linear = new()
        {
            { AudioBusKind.Master, 1f }, { AudioBusKind.Music, 0.7f },
            { AudioBusKind.Sfx, 1f }, { AudioBusKind.Ambient, 0.6f },
            { AudioBusKind.Voice, 1f },
        };
        private readonly Dictionary<string, AudioClip> _clips = new();
        private readonly List<AudioSource> _ambient = new();

        public AudioBus(IGameSettings settings)
        {
            if (settings != null)
            {
                _linear[AudioBusKind.Master] = Mathf.Clamp01(settings.MasterVolume);
                _linear[AudioBusKind.Music] = Mathf.Clamp01(settings.MusicVolume);
                _linear[AudioBusKind.Sfx] = Mathf.Clamp01(settings.SfxVolume);
            }
            var root = new GameObject("[AudioBus Root]");
            Object.DontDestroyOnLoad(root);
        }
        public void SetBusVolume(AudioBusKind bus, float linear) => _linear[bus] = Mathf.Clamp01(linear);
        public void Register(string id, AudioClip clip) { if (!string.IsNullOrEmpty(id) && clip != null) _clips[id] = clip; }
        public void Play(string id, Vector3? pos = null)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!_clips.TryGetValue(id, out var clip) || clip == null) return;
            var go = new GameObject($"sfx_{id}"); go.transform.position = pos ?? Vector3.zero;
            var src = go.AddComponent<AudioSource>();
            src.clip = clip; src.volume = _linear[AudioBusKind.Sfx] * _linear[AudioBusKind.Master]; src.Play();
            Object.Destroy(go, clip.length + 0.1f);
        }
        public void StopAll() { foreach (var a in _ambient) if (a != null) a.Stop(); }
    }
}
