// =====================================================================
//  Neon Cipher — Inventory
//  File:    InventoryService.cs
// =====================================================================
using System.Collections.Generic;
using NeonCipher.Core;

namespace NeonCipher.Inventory
{
    public interface IInventory
    {
        void Add(string id, int count);
        bool Has(string id, int count);
        int  Count(string id);
        void AddMoney(int amount);
        void AddXp(int amount);
        int  Money { get; }
        int  Xp    { get; }
        IEnumerable<InventoryEntry> Entries { get; }
    }

    public sealed class InventoryService : IInventory
    {
        private readonly Dictionary<string, int> _stacks = new();
        public int Money { get; private set; } = 500;
        public int Xp    { get; private set; }
        public IEnumerable<InventoryEntry> Entries { get { foreach (var kv in _stacks) yield return new InventoryEntry { Id = kv.Key, Count = kv.Value }; } }

        public void Add(string id, int count)
        {
            if (!string.IsNullOrEmpty(id) && count > 0) _stacks[id] = (_stacks.TryGetValue(id, out var n) ? n : 0) + count;
        }
        public bool Has(string id, int count) => Count(id) >= count;
        public int  Count(string id) => _stacks.TryGetValue(id, out var n) ? n : 0;
        public void AddMoney(int amount) => Money += amount;
        public void AddXp(int amount)    => Xp    += amount;
    }
}
