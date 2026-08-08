// =====================================================================
//  Neon Cipher — Mission Triggers
//  File:    MissionTrigger.cs
//  Notes:   Volume trigger that advances the active mission when the
//           player enters with the required objective radius.
// =====================================================================
using UnityEngine;

namespace NeonCipher.World
{
    public sealed class MissionTrigger : MonoBehaviour
    {
        public string MissionId;
        public int    StepRequired;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            var ctrl = NeonCipher.Core.GameServices.Current?.Get<NeonCipher.Mission.MissionController>();
            if (ctrl == null) return;
            var cur = ctrl.CurrentStep();
            if (cur == null) return;
            if (other.transform.position.y - cur.ObjectiveWorldPos.y < -100f) return; // rough tag guard
            ctrl.ProgressToNextStep();
        }
    }
}
