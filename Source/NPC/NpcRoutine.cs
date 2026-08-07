// =====================================================================
//  Neon Cipher — Civilian NPC routine AI
//  File:    NpcRoutine.cs
//  Notes:   Six "slots" in the day:
//              Wake → Commute → Work → Lunch → Shop → Home → Sleep.
//           Routines defined in ScriptableObject ⇒ designer-friendly.
// =====================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NeonCipher.NPC
{
    [CreateAssetMenu(menuName = "Neon Cipher/NPC Routine")]
    public sealed class NpcRoutine : ScriptableObject
    {
        [System.Serializable]
        public sealed class Slot
        {
            public string   Label;
            public float    StartHour;
            public float    EndHour;
            public Vector3  Destination;     // editor-placed nav target
            public NpcActivity Activity = NpcActivity.Walk;
        }
        public List<Slot> Slots = new();
    }

    public enum NpcActivity { Idle, Walk, Commute, Work, Shop, Eat, Sleep }

    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NpcRoutineDriver : MonoBehaviour
    {
        public NpcRoutine Routine;
        private NavMeshAgent _agent;
        private NpcRoutine.Slot _slot;
        private float _nextReselect;

        private void Awake() => _agent = GetComponent<NavMeshAgent>();

        private void Update()
        {
            if (Routine == null) return;
            float now = NeonCipher.Core.GameServices.Current.Get<IWorldClock>().CurrentTime;
            if (Time.time > _nextReselect) { _slot = SelectSlot(now); _nextReselect = Time.time + 15f; }
            if (_slot != null && _agent.isOnNavMesh) _agent.SetDestination(_slot.Destination);
        }

        private NpcRoutine.Slot SelectSlot(float hour)
        {
            foreach (var s in Routine.Slots)
                if (hour >= s.StartHour && hour < s.EndHour) return s;
            return null;
        }
    }
}
