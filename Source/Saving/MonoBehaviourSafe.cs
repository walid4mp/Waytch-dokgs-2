// =====================================================================
//  Neon Cipher — SaveMenuController (correct base class)
// =====================================================================
using System;
using System.IO;
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
            SaveTo1.onClick.AddListener(() => SaveInto(1));
            SaveTo2.onClick.AddListener(() => SaveInto(2));
            SaveTo3.onClick.AddListener(() => SaveInto(3));
            Load1.onClick.AddListener(() => LoadFrom(1));
            Load2.onClick.AddListener(() => LoadFrom(2));
            Load3.onClick.AddListener(() => LoadFrom(3));
        }

        private void SaveInto(int slot)
        {
            var save = GameServices.Current.Get<ISaveSystem>();
            save.Save(slot, GameStateCollector.Collect());
            Refresh();
        }
        private void LoadFrom(int slot)
        {
            var save = GameServices.Current.Get<ISaveSystem>();
            if (save.Load(slot, out var data)) GameStateApplier.Apply(data);
        }

        public void Refresh()
        {
            var save = GameServices.Current.Get<ISaveSystem>();
            string Stamp(int s) => save.Load(s, out var d) ? $"{s}: {d.SavedAtIso}  {d.SceneName}" : $"{s}: — empty —";
            Slot1.text = Stamp(1); Slot2.text = Stamp(2); Slot3.text = Stamp(3);
        }
    }
}
