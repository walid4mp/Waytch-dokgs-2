// =====================================================================
//  Neon Cipher — Core Service Locator
//  File:    GameServices.cs
//  Purpose: Lightweight service container (composition root). Keeps the
//           code free of statics while still allowing subsystems to be
//           looked up by interface.
// =====================================================================
using System;
using System.Collections.Generic;

namespace NeonCipher.Core
{
    public sealed class GameServices
    {
        public static GameServices Current { get; private set; }
        private readonly Dictionary<Type, object> _services = new();

        public GameServices()
        {
            if (Current != null)
                throw new InvalidOperationException("GameServices already initialized. Call Shutdown() first.");
            Current = this;
        }

        public void Register<TService>(TService impl) where TService : class
        {
            if (impl == null) throw new ArgumentNullException(nameof(impl));
            var t = typeof(TService);
            if (_services.ContainsKey(t))
                throw new InvalidOperationException($"Service {t.Name} already registered.");
            _services[t] = impl;
        }

        public TService Get<TService>() where TService : class
        {
            if (_services.TryGetValue(typeof(TService), out var s)) return (TService)s;
            throw new InvalidOperationException($"Service {typeof(TService).Name} not registered.");
        }

        public bool TryGet<TService>(out TService svc) where TService : class
        {
            if (_services.TryGetValue(typeof(TService), out var s))
            { svc = (TService)s; return true; }
            svc = null;
            return false;
        }

        public void Shutdown()
        {
            _services.Clear();
            if (Current == this) Current = null;
        }
    }
}
