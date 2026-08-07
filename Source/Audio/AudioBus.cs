// =====================================================================
//  Neon Cipher — Audio Bus
//  File:    AudioBus.cs
// =====================================================================
using System.Collections.Generic;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Audio
{
    public sealed class AudioBus : IAudioBus
    {
        private readonly IGameSettings _settings;
        private readonly Dictionary<AudioBusKind, float> _linear = new()
        {
            { AudioBusKind.Master,  1f }, { AudioBusKind.Music, 0.7f },
            { AudioBusKind.Sfx,    1f }, { AudioBusKind.Ambient,0.6f }, { AudioBusKind.Voice, 1f },
        };
        private readonly Dictionary<string, AudioClip> _clips = new();
        private readonly List<AudioSource> _ambient = new();
        private GameObject _root;

        public AudioBus(IGameSettings settings) { _settings = settings; Init(); }

        private void Init()
        {
            _root = new GameObject("[AudioBus Root]");
            Object.DontDestroyOnLoad(_root);
            // placeholder: in production, AudioMixer is wired here via Resources.Load
        }

        public void SetBusVolume(AudioBusKind bus, float linear)
        {
            _linear[bus] = Mathf.Clamp01(linear);
            // → AudioMixer.SetFloat in production
        }

        public void Play(string id, Vector3? pos = null)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!_clips.TryGetValue(id, out var clip))
            {
                Debug.LogWarning($"[Audio] clip '{id}' not registered (placeholder silence).");
                return;
            }
            var go = new GameObject($"sfx_{id}");
            go.transform.position = pos ?? Vector3.zero;
            var src = go.AddComponent<AudioSource>();
            src.clip = clip;
            src.volume = _linear[AudioBusKind.Sfx] * _linear[AudioBusKind.Master];
            src.Play();
            Object.Destroy(go, clip.length + 0.1f);
        }

        public void StopAll()
        {
            foreach (var a in _ambient) a.Stop();
        }
    }
}
