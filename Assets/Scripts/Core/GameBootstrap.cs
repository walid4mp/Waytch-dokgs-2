// Neon Cipher — Composition Root (registers all services)
using NeonCipher.Audio;
using NeonCipher.Hacking;
using NeonCipher.Inventory;
using NeonCipher.Localization;
using NeonCipher.Mission;
using NeonCipher.PhoneUI;
using NeonCipher.Saving;
using NeonCipher.TimeWeather;
using UnityEngine;

namespace NeonCipher.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static bool _bootedOnce;

        private void Awake()
        {
            if (_bootedOnce) { Destroy(gameObject); return; }
            _bootedOnce = true;
            DontDestroyOnLoad(gameObject);

            var services = new GameServices();
            var settings = new GameSettingsService();
            var time = new WorldClock();
            var save = new SaveSystem(settings);
            var localization = new LocalizationService();
            var audioBus = new AudioBus(settings);
            var weather = new WeatherSystem(time);
            var hackingBus = new HackingBus();
            var inventory = new InventoryService();
            var missions = new MissionController();
            var phone = new PhoneController();

            services.Register<IGameSettings>(settings);
            services.Register<IWorldClock>(time);
            services.Register<ISaveSystem>(save);
            services.Register<ILocalization>(localization);
            services.Register<IAudioBus>(audioBus);
            services.Register<IWeather>(weather);
            services.Register<IHackingBus>(hackingBus);
            services.Register<IInventory>(inventory);
            services.Register<MissionController>(missions);
            services.Register<PhoneController>(phone);

            missions.AttachInventory(inventory);
            hackingBus.AttachAudio(audioBus);
            phone.AttachHacking(hackingBus);

            var driverGo = new GameObject("[WorldClockDriver]");
            driverGo.transform.SetParent(transform);
            driverGo.AddComponent<WorldClockDriver>();
        }

        private void OnDestroy()
        {
            if (GameServices.Current != null) GameServices.Current.Shutdown();
            _bootedOnce = false;
        }
    }
}
