// =====================================================================
//  Neon Cipher — Mission System
//  File:    MissionController.cs
//  Notes:   ScriptableObject missions, runtime tracking, branching
//           step-by-step objectives, optional side-objectives.
// =====================================================================
using System;
using System.Collections.Generic;
using NeonCipher.Core;
using UnityEngine;

namespace NeonCipher.Mission
{
    [CreateAssetMenu(menuName = "Neon Cipher/Mission")]
    public sealed class MissionSO : ScriptableObject
    {
        [System.Serializable]
        public sealed class Step
        {
            public string Id;
            public string Title;
            [TextArea] public string Description;
            public Vector3 ObjectiveWorldPos;
            public float  Radius = 6f;
            public string OnCompleteEventId; // hook for next mission
        }
        public string   Id;
        public string   DisplayName;
        [TextArea]    public string Briefing;
        public List<Step> Steps = new();
        public int    RewardMoney = 1500;
        public int    RewardXp    = 250;
    }

    public sealed class MissionRuntime
    {
        public MissionSO Data;
        public int   StepIndex;
        public string Status;  // available | active | complete | failed
    }

    public sealed class MissionController
    {
        private IInventory _inv;
        private readonly List<MissionRuntime> _all = new();
        private MissionRuntime _active;
        public IReadOnlyList<MissionRuntime> All => _all;

        public void AttachInventory(IInventory inv) => _inv = inv;

        public void Register(MissionSO so)
        {
            _all.Add(new MissionRuntime { Data = so, StepIndex = 0, Status = "available" });
        }

        public bool Start(string id)
        {
            var r = _all.Find(m => m.Data.Id == id && m.Status == "available");
            if (r == null) return false;
            _active = r; r.Status = "active";
            return true;
        }

        public void ProgressToNextStep()
        {
            if (_active == null) return;
            _active.StepIndex++;
            if (_active.StepIndex >= _active.Data.Steps.Count)
            {
                _active.Status = "complete";
                _inv?.AddMoney(_active.Data.RewardMoney);
                _inv?.AddXp(_active.Data.RewardXp);
            }
        }

        public MissionSO.Step CurrentStep() =>
            _active != null && _active.StepIndex < _active.Data.Steps.Count
                ? _active.Data.Steps[_active.StepIndex]
                : null;
    }
}
