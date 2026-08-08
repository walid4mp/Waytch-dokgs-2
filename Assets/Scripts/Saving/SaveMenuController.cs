// Neon Cipher — Save / Load UI binding
using NeonCipher.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NeonCipher.Saving
{
    public sealed class SaveMenuController : MonoBehaviour
    {
        public Text Slot1, Slot2, Slot3;
        public Button SaveTo1, SaveTo2, SaveTo3, Load1, Load2, Load3;

        private void Start()
        {
            Refresh();
            if (SaveTo1) SaveTo1.onClick.AddListener(() => SaveInto(1));
            if (SaveTo2) SaveTo2.onClick.AddListener(() => SaveInto(2));
            if (SaveTo3) SaveTo3.onClick.AddListener(() => SaveInto(3));
            if (Load1) Load1.onClick.AddListener(() => LoadFrom(1));
            if (Load2) Load2.onClick.AddListener(() => LoadFrom(2));
            if (Load3) Load3.onClick.AddListener(() => LoadFrom(3));
        }
        private void SaveInto(int slot) { var s = GameServices.Current.Get<ISaveSystem>(); s.Save(slot, GameStateCollector.Collect()); Refresh(); }
        private void LoadFrom(int slot) { var s = GameServices.Current.Get<ISaveSystem>(); if (s.Load(slot, out var d)) GameStateApplier.Apply(d); }
        public void Refresh()
        {
            if (GameServices.Current == null) return;
            var save = GameServices.Current.Get<ISaveSystem>();
            string Stamp(int s) => save.Load(s, out var d) ? $"{s}: {d.SavedAtIso}  {d.SceneName}" : $"{s}: - empty -";
            if (Slot1) Slot1.text = Stamp(1); if (Slot2) Slot2.text = Stamp(2); if (Slot3) Slot3.text = Stamp(3);
        }
    }
}
