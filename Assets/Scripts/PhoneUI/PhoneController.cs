// Neon Cipher — LinkDeck (in-game phone) state machine
using System;
using NeonCipher.Hacking;

namespace NeonCipher.PhoneUI
{
    public enum PhoneScreen { Locked, Home, Map, HackDeck, Mp3Player, Messages, Darknet, Settings, Inventory, Missions, Camera, Contacts }

    public sealed class PhoneController
    {
        private IHackingBus _hack;
        public PhoneScreen Current { get; private set; } = PhoneScreen.Locked;
        public event Action<PhoneScreen> ScreenChanged;
        public void AttachHacking(IHackingBus h) => _hack = h;
        public bool IsOpen => Current != PhoneScreen.Locked;
        public void Open() => Set(PhoneScreen.Home);
        public void OpenScreen(PhoneScreen s) => Set(s);
        public void Close() => Set(PhoneScreen.Locked);
        public void Toggle() => Set(IsOpen ? PhoneScreen.Locked : PhoneScreen.Home);
        public void RefreshHackDeck() => ScreenChanged?.Invoke(Current);
        private void Set(PhoneScreen s) { if (s == Current) return; Current = s; ScreenChanged?.Invoke(s); }
    }
}
