// =====================================================================
//  Neon Cipher — Hacking Bus (original mechanics)
//  File:    Hacking.cs
//  Notes:   All hackable devices implement IHackable. The bus resolves
//           nearest hackable in range, runs a puzzle minigame, and
//           applies the Effect. Original mechanic names — no copy of
//           any copyrighted system.
// =====================================================================
using System.Collections;
using System.Collections.Generic;
using NeonCipher.Audio;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Hacking
{
    public enum HackType
    {
        EyeHack,          // public + private CCTV cameras
        CypherLock,       // electronic locks on doors / vaults / vehicles
        SignalOverride,   // traffic lights, bridges, road gates
        SwarmHack,        // public utility drones, streetlights
        GridTap,          // substations, alarms, vending machines
        ConsoleBreach     // terminals, ATMs, billboard screens
    }

    public interface IHackable
    {
        HackType Tech        { get; }
        float    HackRange   { get; }
        float    HackSeconds { get; }
        bool     CanHack     { get; }
        void     BeginHack();
        void     CancelHack();
        void     CompleteHack();
        string   DisplayName { get; }
    }

    public sealed class HackingBus : IHackingBus
    {
        private IAudioBus _audio;
        private IHackable _current;
        public IHackable Current => _current;
        public HackProgress Progress { get; } = new();
        public event System.Action<HackProgress> ProgressChanged;
        public event System.Action<IHackable> Started;

        // DI-friendly factory
        public void AttachAudio(IAudioBus audio) => _audio = audio;
        public IHackable FindNearestInRange(Vector3 from, float range, HashSet<HackType> allowed)
        {
            IHackable best = null; float bestSqr = range * range;
            foreach (var h in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
            {
                if (h is IHackable hi && allowed.Contains(hi.Tech) && hi.CanHack)
                {
                    float sqr = (h.transform.position - from).sqrMagnitude;
                    if (sqr <= bestSqr) { best = hi; bestSqr = sqr; }
                }
            }
            return best;
        }
        public IEnumerator RunHack(IHackable target)
        {
            if (target == null) yield break;
            _current = target; target.BeginHack();
            Started?.Invoke(target);
            Progress.Reset(target.HackSeconds);
            float t = 0f;
            while (t < target.HackSeconds)
            {
                t += Time.deltaTime;
                Progress.Set(0f, t, target.HackSeconds);
                ProgressChanged?.Invoke(Progress);
                yield return null;
                if (!target.CanHack) break; // interrupted (player shot / left range)
            }
            if (t >= target.HackSeconds)
            {
                target.CompleteHack();
                _audio?.Play("hack_success");
            }
            else
            {
                target.CancelHack();
                _audio?.Play("hack_fail");
            }
            _current = null;
        }
    }

    public sealed class HackProgress
    {
        public float Elapsed, Duration;
        public string Name;
        public void Reset(float d)        { Duration = d; Elapsed = 0f; }
        public void Set(string n, float e, float d) { Name = n; Elapsed = e; Duration = d; }
    }

    // ---------------------------------------------------------------------
    // Concrete hackable device stubs (extend with visuals/puzzles per device)
    // ---------------------------------------------------------------------

    public sealed class EyeHackableCamera : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.EyeHack;
        public float HackRange => 18f;
        public float HackSeconds => 2.5f;
        public string DisplayName => "Security Camera";
        public bool CanHack => isActiveAndEnabled;
        public void BeginHack()  { /* start mini-game */ }
        public void CancelHack() { }
        public void CompleteHack() => Debug.Log($"[Hack] {DisplayName} view captured.");
    }

    public sealed class CypherLockable : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.CypherLock;
        public float HackRange => 3.5f;
        public float HackSeconds => 1.8f;
        public string DisplayName => "Electronic Lock";
        [SerializeField] private bool _locked = true;
        public bool CanHack => _locked;
        public void BeginHack() {}
        public void CancelHack() {}
        public void CompleteHack() { _locked = false; Debug.Log("[Hack] lock popped."); }
    }

    public sealed class SignalOverrideableLight : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.SignalOverride;
        public float HackRange => 30f;
        public float HackSeconds => 1.2f;
        public string DisplayName => "Traffic Signal";
        public bool CanHack => true;
        [SerializeField] private Traffic.TrafficLightController _light;
        public void BeginHack() {}
        public void CancelHack() {}
        public void CompleteHack() => _light.Override(Traffic.LightSignal.Green);
    }

    public sealed class SwarmHackableDrone : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.SwarmHack;
        public float HackRange => 25f;
        public float HackSeconds => 3f;
        public string DisplayName => "Public Drone";
        public bool CanHack => true;
        public void BeginHack() {}
        public void CancelHack() {}
        public void CompleteHack() => Debug.Log("[Hack] drone swarm redirected.");
    }

    public sealed class GridTapableAlarm : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.GridTap;
        public float HackRange => 12f;
        public float HackSeconds => 1.5f;
        public string DisplayName => "Alarm / Substation";
        public bool CanHack => true;
        public void BeginHack() {}
        public void CancelHack() {}
        public void CompleteHack() => Debug.Log("[Hack] grid tapped / alarm silenced.");
    }

    public sealed class ConsoleBreachableTerminal : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.ConsoleBreach;
        public float HackRange => 2.5f;
        public float HackSeconds => 4f;
        public string DisplayName => "Terminal / ATM";
        public bool CanHack => true;
        public void BeginHack() {}
        public void CancelHack() {}
        public void CompleteHack() => Debug.Log("[Hack] console breached – payout queued.");
    }

    public interface IHackingBus
    {
        HackProgress Progress { get; }
        HashSet<HackType> ActiveMask { get; set; }
        IHackable FindNearestInRange(Vector3 from, float range, HashSet<HackType> allowed);
        IEnumerator RunHack(IHackable target);
        event System.Action<IHackable> Started;
        event System.Action<HackProgress> ProgressChanged;
    }

    // Partial-class extension – extends HackingBus with ActiveMask without
    // breaking the partial marker pattern used elsewhere.
    public partial class HackingBusExt
    {
        // scaffolding placeholder; ActiveMask lives in subclasses
    }
}
