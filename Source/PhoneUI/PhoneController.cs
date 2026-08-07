// =====================================================================
//  Neon Cipher — LinkDeck (in-game phone)
//  File:    PhoneController.cs
//  Notes:   Pure C# state machine. UI is UGUI or UI Toolkit at runtime.
// =====================================================================
using System;
using NeonCipher.Core;
using NeonCipher.Hacking;
using UnityEngine;

namespace NeonCipher.PhoneUI
{
    public enum PhoneScreen { Locked, Home, Map, HackDeck, Mp3Player, Messages, Darknet, Settings }

    public sealed class PhoneController
    {
        private IHackingBus _hack;
        public PhoneScreen Current { get; private set; } = PhoneScreen.Locked;
        public event Action<PhoneScreen> ScreenChanged;

        public void AttachHacking(IHackingBus h) => _hack = h;
        public bool IsOpen => Current != PhoneScreen.Locked && Current != PhoneScreen.Home && Current != PhoneScreen.Settings ? true : Current == PhoneScreen.Home ? true : false;

        public void Open()
        {
            Set(PhoneScreen.Home);
        }
        public void OpenScreen(PhoneScreen s)
        {
            Set(s);
        }
        public void Close() => Set(PhoneScreen.Locked);

        public void RefreshHackDeck()
        {
            // UI listens to this event and re-binds active hack-type icons
            Set(Current);
        }

        private void Set(PhoneScreen s)
        {
            if (s == Current) return;
            Current = s;
            ScreenChanged?.Invoke(s);
        }
    }
}
