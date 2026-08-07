// =====================================================================
//  Neon Cipher — Traffic System
//  File:    TrafficSystem.cs
//  Notes:   Waypoint graph on every road. Lanes = offset from centre line.
//           TrafficLightController and HackableTrafficLight bridge to hacking.
// =====================================================================
using System.Collections.Generic;
using UnityEngine;

namespace NeonCipher.Traffic
{
    public sealed class TrafficWaypoint : MonoBehaviour
    {
        public TrafficWaypoint Next;
        public float SpeedLimit = 13.89f; // ~50 km/h
    }

    /// <summary>Drives a single civilian/NPC car along the waypoint graph.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class TrafficAgent : MonoBehaviour
    {
        [SerializeField] private TrafficWaypoint _start;
        [SerializeField] private float _stopDistance = 4f;
        private TrafficLightController _lightObserved;
        private Rigidbody _rb;
        private TrafficWaypoint _current;
        public Vector3 LaneOffset = Vector3.zero;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
        }

        public void Initialize(TrafficWaypoint start, Vector3 laneOffset)
        {
            _start = start;
            LaneOffset = laneOffset;
            _current = start;
        }

        private void FixedUpdate()
        {
            if (_current == null || _current.Next == null) return;
            var dest = _current.Next.transform.position + _current.Next.transform.right * LaneOffset.x
                       + _current.Next.transform.up * LaneOffset.y;
            float speed = _current.Next.SpeedLimit * (ShouldStop() ? 0f : 1f);
            _rb.MovePosition(Vector3.MoveTowards(transform.position, dest, speed * Time.fixedDeltaTime));
            if ((transform.position - dest).sqrMagnitude < 0.5f) _current = _current.Next;
        }

        private bool ShouldStop()
        {
            // Hook Signal Override hacking: read red light in front
            return _lightObserved != null && _lightObserved.CurrentSignal == LightSignal.Red
                   && Vector3.Distance(transform.position, _lightObserved.transform.position) < _stopDistance;
        }

        public void Observe(TrafficLightController light) => _lightObserved = light;
    }

    public enum LightSignal { Red, Amber, Green }

    /// <summary>Single traffic light. Public surface is what hacking mutates.</summary>
    public sealed class TrafficLightController : MonoBehaviour
    {
        public LightSignal CurrentSignal = LightSignal.Red;
        public void Override(LightSignal sig) => CurrentSignal = sig;
    }
}
