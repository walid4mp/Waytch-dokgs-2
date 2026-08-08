// =====================================================================
//  Neon Cipher — World Streaming (district load/unload)
//  File:    WorldStreaming.cs
//  Notes:   Splits Lumen Bay into chunks; unloads distant chunks to keep
//           mobile VRAM under control. Deterministic, GC-light.
// =====================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeonCipher.World
{
    public sealed class WorldStreaming : MonoBehaviour
    {
        [SerializeField] private Transform _streamer;
        [SerializeField] private float     _radiusActive = 600f;
        [SerializeField] private float     _radiusDespawn = 800f;

        private readonly Dictionary<string, AsyncOperation> _loading = new();
        private readonly HashSet<string> _active = new();
        private readonly List<string> _candidates = new();

        private void Update()
        {
            _candidates.Clear();
            Vector3 p = _streamer.position;
            // Sample 8x8 grid; nearest 9 chunks.
            for (int x = -1; x <= 1; x++)
                for (int z = -1; z <= 1; z++)
                {
                    var sc = $"LumenBay_{x}_{z}";
                    _candidates.Add(sc);
                    if (!_active.Contains(sc)) BeginLoad(sc);
                }
            // Despawn far chunks
            foreach (var name in _active)
                if (!_candidates.Contains(name)) BeginUnload(name);
        }

        private void BeginLoad(string scene)
        {
            if (_loading.ContainsKey(scene)) return;
            var op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            if (op != null) { _loading[scene] = op; op.completed += _ => _active.Add(scene); }
        }

        private void BeginUnload(string scene)
        {
            if (_loading.ContainsKey(scene)) return;
            var op = SceneManager.UnloadSceneAsync(scene);
            if (op != null) op.completed += _ => _active.Remove(scene);
        }
    }
}
