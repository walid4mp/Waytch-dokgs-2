// =====================================================================
//  Neon Cipher — Networking stubs (offline-first)
//  File:    NetStubs.cs
//  Notes:   Code uses define NEONCIPHER_NETWORKING to enable. Offline
//           path is identical to the in-game path; online is opt-in.
// =====================================================================
using System;
using UnityEngine;

namespace NeonCipher.Networking
{
    public interface INetClient
    {
        bool Connect(string host, int port);
        void Disconnect();
        bool IsConnected { get; }
        void Send(string channel, byte[] payload);
        event Action<string, byte[]> OnPacket;
    }

    public sealed class OfflineNetClient : INetClient
    {
        public bool Connect(string host, int port) => false;
        public void Disconnect() { }
        public bool IsConnected => false;
        public void Send(string channel, byte[] payload) { }
        public event Action<string, byte[]> OnPacket;
    }
}
