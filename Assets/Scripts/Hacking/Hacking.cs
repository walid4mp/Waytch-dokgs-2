// Neon Cipher — Hacking Bus (100% original mechanics)
using System;
using System.Collections;
using System.Collections.Generic;
using NeonCipher.Audio;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Hacking
{
    public enum HackType { EyeHack, CypherLock, SignalOverride, SwarmHack, GridTap, ConsoleBreach }

    public interface IHackable
    {
        HackType Tech { get; }
        float HackRange { get; }
        float HackSeconds { get; }
        bool CanHack { get; }
        void BeginHack();
        void CancelHack();
        void CompleteHack();
        string DisplayName { get; }
    }

    public interface IHackingBus
    {
        HackProgress Progress { get; }
        HashSet<HackType> ActiveMask { get; set; }
        IHackable Current { get; }
        bool IsBusy { get; }
        void AttachAudio(IAudioBus audio);
        IHackable FindNearestInRange(Vector3 from, float range, HashSet<HackType> allowed);
        IEnumerator RunHack(IHackable target);
        event Action<IHackable> Started;
        event Action<IHackable> Completed;
        event Action<HackProgress> ProgressChanged;
    }

    public sealed class HackProgress
    {
        public float Elapsed;
        public float Duration;
        public string Name;
        public void Reset(float d) { Duration = d; Elapsed = 0f; }
        public void Set(string n, float e, float d) { Name = n; Elapsed = e; Duration = d; }
        public float Ratio => Duration > 0.001f ? Mathf.Clamp01(Elapsed / Duration) : 0f;
    }

    public sealed class HackingBus : IHackingBus
    {
        private IAudioBus _audio;
        private IHackable _current;
        public IHackable Current => _current;
        public bool IsBusy => _current != null;
        public HackProgress Progress { get; } = new();
        public HashSet<HackType> ActiveMask { get; set; } = new HashSet<HackType>
        { HackType.EyeHack, HackType.CypherLock, HackType.SignalOverride, HackType.SwarmHack, HackType.GridTap, HackType.ConsoleBreach };
        public event Action<IHackable> Started;
        public event Action<IHackable> Completed;
        public event Action<HackProgress> ProgressChanged;
        public void AttachAudio(IAudioBus audio) => _audio = audio;

        public IHackable FindNearestInRange(Vector3 from, float range, HashSet<HackType> allowed)
        {
            IHackable best = null; float bestSqr = range * range;
            var all = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] is IHackable hi && hi.CanHack && (allowed == null || allowed.Contains(hi.Tech)))
                {
                    float sqr = (all[i].transform.position - from).sqrMagnitude;
                    if (sqr <= bestSqr) { best = hi; bestSqr = sqr; }
                }
            }
            return best;
        }

        public IEnumerator RunHack(IHackable target)
        {
            if (target == null) yield break;
            _current = target; target.BeginHack(); Started?.Invoke(target);
            Progress.Reset(target.HackSeconds);
            float t = 0f; bool cancelled = false;
            while (t < target.HackSeconds)
            {
                t += Time.deltaTime;
                Progress.Set(target.DisplayName, t, target.HackSeconds);
                ProgressChanged?.Invoke(Progress);
                yield return null;
                if (!target.CanHack) { cancelled = true; break; }
            }
            if (!cancelled) { target.CompleteHack(); _audio?.Play(SfxId.HackSuccess); Completed?.Invoke(target); }
            else { target.CancelHack(); _audio?.Play(SfxId.HackFail); }
            _current = null;
        }
    }

    public sealed class EyeHackableCamera : MonoBehaviour, IHackable
    { public HackType Tech => HackType.EyeHack; public float HackRange => 18f; public float HackSeconds => 2.5f; public string DisplayName => "Security Camera"; public bool CanHack => isActiveAndEnabled; public void BeginHack(){} public void CancelHack(){} public void CompleteHack() => Debug.Log("[Hack] camera captured."); }

    public sealed class CypherLockable : MonoBehaviour, IHackable
    { [SerializeField] private bool _locked = true; public HackType Tech => HackType.CypherLock; public float HackRange => 3.5f; public float HackSeconds => 1.8f; public string DisplayName => "Electronic Lock"; public bool CanHack => _locked; public void BeginHack(){} public void CancelHack(){} public void CompleteHack() { _locked = false; Debug.Log("[Hack] lock popped."); } }

    public sealed class SignalOverrideableLight : MonoBehaviour, IHackable
    {
        public HackType Tech => HackType.SignalOverride; public float HackRange => 30f; public float HackSeconds => 1.2f; public string DisplayName => "Traffic Signal"; public bool CanHack => true;
        public void BeginHack(){} public void CancelHack(){}
        public void CompleteHack() { var l = GetComponent<NeonCipher.Traffic.TrafficLightController>(); if (l != null) l.Override(NeonCipher.Traffic.LightSignal.Green); }
    }

    public sealed class SwarmHackableDrone : MonoBehaviour, IHackable
    { public HackType Tech => HackType.SwarmHack; public float HackRange => 25f; public float HackSeconds => 3f; public string DisplayName => "Public Drone"; public bool CanHack => true; public void BeginHack(){} public void CancelHack(){} public void CompleteHack() => Debug.Log("[Hack] drone redirected."); }

    public sealed class GridTapableAlarm : MonoBehaviour, IHackable
    { public HackType Tech => HackType.GridTap; public float HackRange => 12f; public float HackSeconds => 1.5f; public string DisplayName => "Alarm / Substation"; public bool CanHack => true; public void BeginHack(){} public void CancelHack(){} public void CompleteHack() => Debug.Log("[Hack] grid tapped."); }

    public sealed class ConsoleBreachableTerminal : MonoBehaviour, IHackable
    { public HackType Tech => HackType.ConsoleBreach; public float HackRange => 2.5f; public float HackSeconds => 4f; public string DisplayName => "Terminal / ATM"; public bool CanHack => true; public void BeginHack(){} public void CancelHack(){} public void CompleteHack() => Debug.Log("[Hack] console breached."); }
}
