// =====================================================================
//  Neon Cipher — Composition Root
//  File:    GameBootstrap.cs
// =====================================================================
using System.Collections;
using NeonCipher.Audio;
using NeonCipher.Saving;
using NeonCipher.Localization;
using NeonCipher.Hacking;
using NeonCipher.TimeWeather;
using NeonCipher.World;
using NeonCipher.Inventory;
using NeonCipher.Mission;
using NeonCipher.PhoneUI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeonCipher.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string _firstScene = "LumenBay_District01";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            var services = new GameServices();

            var time          = new WorldClock();
            var settings      = new GameSettingsService();
            var save          = new SaveSystem(settings);
            var localization  = new LocalizationService();
            var audioBus      = new AudioBus(settings);
            var weather       = new WeatherSystem(time);
            var hackingBus    = new HackingBus();
            var inventory     = new InventoryService();
            var missions      = new MissionController();
            var phone         = new PhoneController();

            services.Register<IWorldClock>(time);
            services.Register<IGameSettings>(settings);
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

            StartCoroutine(LoadInitialScene());
        }

        private IEnumerator LoadInitialScene()
        {
            yield return null;
            SceneManager.LoadScene(_firstScene, LoadSceneMode.Additive);
        }

        private void OnDestroy() => GameServices.Current?.Shutdown();
    }
}
