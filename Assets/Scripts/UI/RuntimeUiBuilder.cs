// Neon Cipher — Runtime UI Builder (Splash/Loading/Login/Menu/Settings/HUD/Pause/Phone)
using NeonCipher.Core;
using NeonCipher.PhoneUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NeonCipher.UI
{
    public enum UiState { Splash, Loading, Login, MainMenu, Settings, InGame, Pause }

    public sealed class RuntimeUiBuilder : MonoBehaviour
    {
        public event System.Action OnStartGameRequested;
        public event System.Action OnResumeGameRequested;
        public event System.Action OnExitGameRequested;

        private Canvas _canvas;
        private GameObject _splash, _loading, _login, _mainMenu, _settings, _hud, _pause, _phone;
        private HudController _hudController;
        private Slider _loadingBar;
        private Text _loadingLabel;
        private Font _font;

        public HudController Hud => _hudController;

        private void Awake()
        {
            _font = Font.CreateDynamicFontFromOSFont(new[] { "Noto Sans", "Arial", "Liberation Sans" }, 20);
            if (FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("[EventSystem]");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
            BuildCanvas();
            BuildSplash(); BuildLoading(); BuildLogin(); BuildMainMenu();
            BuildSettings(); BuildHud(); BuildPause(); BuildPhone();
            Show(UiState.Splash);
        }

        public void Show(UiState state)
        {
            _splash.SetActive(state == UiState.Splash);
            _loading.SetActive(state == UiState.Loading);
            _login.SetActive(state == UiState.Login);
            _mainMenu.SetActive(state == UiState.MainMenu);
            _settings.SetActive(state == UiState.Settings);
            _hud.SetActive(state == UiState.InGame || state == UiState.Pause);
            _pause.SetActive(state == UiState.Pause);
        }

        public void SetLoadingProgress(float ratio, string label = null)
        {
            if (_loadingBar != null) _loadingBar.value = Mathf.Clamp01(ratio);
            if (_loadingLabel != null && !string.IsNullOrEmpty(label)) _loadingLabel.text = label;
        }

        public void TogglePhone()
        {
            var ph = GameServices.Current?.Get<PhoneController>();
            if (ph == null) return;
            ph.Toggle();
            _phone.SetActive(ph.IsOpen);
        }

        private void BuildCanvas()
        {
            var go = new GameObject("[UI Canvas]");
            go.transform.SetParent(transform, false);
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 10;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
        }

        private void BuildSplash()
        {
            _splash = Panel("Splash", new Color(0.03f, 0.05f, 0.09f, 1f));
            Label(_splash, "NEON CIPHER", new Vector2(0, 60), 84, FontStyle.Bold, new Color(0.35f, 0.95f, 1f));
            Label(_splash, "an original open-world hacking prototype", new Vector2(0, -20), 28, FontStyle.Italic, Color.white);
            Label(_splash, "loading systems...", new Vector2(0, -260), 22, FontStyle.Normal, new Color(1, 1, 1, 0.6f));
        }

        private void BuildLoading()
        {
            _loading = Panel("Loading", new Color(0.02f, 0.03f, 0.06f, 1f));
            Label(_loading, "LOADING LUMEN BAY", new Vector2(0, 80), 48, FontStyle.Bold, new Color(0.35f, 0.95f, 1f));
            _loadingLabel = Label(_loading, "initialising...", new Vector2(0, -20), 24, FontStyle.Normal, Color.white);
            var bar = new GameObject("Bar", typeof(RectTransform), typeof(Image));
            bar.transform.SetParent(_loading.transform, false);
            var br = (RectTransform)bar.transform;
            br.sizeDelta = new Vector2(700, 12); br.anchoredPosition = new Vector2(0, -60);
            bar.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 1);
            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(bar.transform, false);
            var fr = (RectTransform)fill.transform;
            fr.anchorMin = new Vector2(0, 0); fr.anchorMax = new Vector2(1, 1);
            fr.offsetMin = Vector2.zero; fr.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = new Color(0.35f, 0.95f, 1f);
            _loadingBar = bar.AddComponent<Slider>();
            _loadingBar.fillRect = fr; _loadingBar.targetGraphic = fill.GetComponent<Image>();
            _loadingBar.minValue = 0; _loadingBar.maxValue = 1; _loadingBar.value = 0;
        }

        private void BuildLogin()
        {
            _login = Panel("Login", new Color(0.02f, 0.04f, 0.07f, 1f));
            Label(_login, "LOGIN (offline)", new Vector2(0, 220), 44, FontStyle.Bold, new Color(0.35f, 0.95f, 1f));
            var user = InputField(_login, new Vector2(0, 80), "Codename", "Cipher");
            InputField(_login, new Vector2(0, 10), "Cypher Key", "", true);
            Toggle(_login, new Vector2(0, -60), "Remember me on this device");
            Button(_login, new Vector2(0, -160), "Sign in offline", 300, 60, () =>
            {
                Debug.Log($"[Login] offline sign-in as {user.text}");
                Show(UiState.MainMenu);
            });
            Button(_login, new Vector2(0, -230), "Skip", 300, 44, () => Show(UiState.MainMenu));
        }

        private void BuildMainMenu()
        {
            _mainMenu = Panel("MainMenu", new Color(0.02f, 0.03f, 0.06f, 1f));
            var title = Label(_mainMenu, "NEON CIPHER", new Vector2(-520, 260), 72, FontStyle.Bold, new Color(0.35f, 0.95f, 1f));
            title.alignment = TextAnchor.MiddleLeft;
            var sub = Label(_mainMenu, "Lumen Bay is waiting", new Vector2(-520, 200), 24, FontStyle.Italic, new Color(1, 1, 1, 0.6f));
            sub.alignment = TextAnchor.MiddleLeft;
            var col = 420;
            Button(_mainMenu, new Vector2(-col, 60), "New Game", 340, 62, () => OnStartGameRequested?.Invoke());
            Button(_mainMenu, new Vector2(-col, -20), "Continue", 340, 62, () => OnStartGameRequested?.Invoke());
            Button(_mainMenu, new Vector2(-col, -100), "Settings", 340, 62, () => Show(UiState.Settings));
            Button(_mainMenu, new Vector2(-col, -180), "Language", 340, 62, CycleLanguage);
            Button(_mainMenu, new Vector2(-col, -260), "Download", 340, 62, () => Debug.Log("[UI] download-pack requested"));
            Button(_mainMenu, new Vector2(-col, -340), "Exit", 340, 62, () => OnExitGameRequested?.Invoke());
        }

        private void BuildSettings()
        {
            _settings = Panel("Settings", new Color(0.02f, 0.04f, 0.07f, 1f));
            Label(_settings, "SETTINGS", new Vector2(0, 300), 52, FontStyle.Bold, new Color(0.35f, 0.95f, 1f));
            var quality = Dropdown(_settings, new Vector2(0, 200), "Graphics", new[] { "Low", "Medium", "High", "Ultra" });
            var master = SliderRow(_settings, new Vector2(0, 130), "Master", 0f, 1f, 1f);
            var music = SliderRow(_settings, new Vector2(0, 70), "Music", 0f, 1f, 0.7f);
            var sfx = SliderRow(_settings, new Vector2(0, 10), "SFX", 0f, 1f, 1f);
            var vibration = Toggle(_settings, new Vector2(0, -60), "Vibration");
            var subtitles = Toggle(_settings, new Vector2(0, -120), "Subtitles");
            var language = Dropdown(_settings, new Vector2(0, -190), "Language", new[] { "English", "Francais", "العربية", "日本語" });
            Button(_settings, new Vector2(0, -300), "Back", 260, 56, () => Show(UiState.MainMenu));
            var ctrl = gameObject.AddComponent<MainMenuController>();
            ctrl.Bind(quality, master, music, sfx, vibration, subtitles, language);
        }

        private void BuildHud()
        {
            _hud = Panel("HUD", new Color(0, 0, 0, 0f), false);
            var hp = HudSlider(_hud, new Vector2(30, -30), new Vector2(0, 1), 260, 14, Color.red);
            var wanted = HudSlider(_hud, new Vector2(30, -60), new Vector2(0, 1), 260, 10, new Color(1f, 0.4f, 0.2f));
            var hackBar = HudSlider(_hud, new Vector2(0, 60), new Vector2(0.5f, 0f), 380, 12, new Color(0.35f, 0.95f, 1f));
            var timeText = Label(_hud, "06:30", new Vector2(-30, -30), 22, FontStyle.Bold, Color.white, TextAnchor.UpperRight);
            timeText.rectTransform.anchorMin = new Vector2(1, 1); timeText.rectTransform.anchorMax = new Vector2(1, 1);
            var objText = Label(_hud, "", new Vector2(0, -30), 22, FontStyle.Bold, new Color(1f, 0.95f, 0.4f), TextAnchor.UpperCenter);
            objText.rectTransform.anchorMin = new Vector2(0.5f, 1); objText.rectTransform.anchorMax = new Vector2(0.5f, 1);
            var hint = Label(_hud, "WASD move - Shift run - Space jump - C crouch - E interact - Tab phone - Esc pause",
                             new Vector2(0, 130), 18, FontStyle.Italic, Color.white, TextAnchor.LowerCenter);
            hint.rectTransform.anchorMin = new Vector2(0.5f, 0); hint.rectTransform.anchorMax = new Vector2(0.5f, 0);
            var money = Label(_hud, "$ 500", new Vector2(-30, -60), 22, FontStyle.Bold, new Color(0.6f, 1f, 0.6f), TextAnchor.UpperRight);
            money.rectTransform.anchorMin = new Vector2(1, 1); money.rectTransform.anchorMax = new Vector2(1, 1);
            _hudController = _hud.AddComponent<HudController>();
            _hudController.Bind(hp, wanted, hackBar, timeText, objText, hint, money, null, _pause);
        }

        private void BuildPause()
        {
            _pause = Panel("Pause", new Color(0, 0, 0, 0.7f));
            Label(_pause, "PAUSED", new Vector2(0, 180), 52, FontStyle.Bold, Color.white);
            Button(_pause, new Vector2(0, 60), "Resume", 320, 56, () => { OnResumeGameRequested?.Invoke(); Show(UiState.InGame); });
            Button(_pause, new Vector2(0, -10), "Save", 320, 56, () =>
            {
                if (GameServices.Current != null && GameServices.Current.TryGet<ISaveSystem>(out var s))
                    s.Save(1, NeonCipher.Saving.GameStateCollector.Collect());
            });
            Button(_pause, new Vector2(0, -80), "Settings", 320, 56, () => Show(UiState.Settings));
            Button(_pause, new Vector2(0, -150), "Main Menu", 320, 56, () => Show(UiState.MainMenu));
            Button(_pause, new Vector2(0, -220), "Exit", 320, 56, () => OnExitGameRequested?.Invoke());
        }

        private void BuildPhone()
        {
            _phone = Panel("PhonePanel", new Color(0, 0, 0, 0.7f));
            var frame = new GameObject("Frame", typeof(RectTransform), typeof(Image));
            frame.transform.SetParent(_phone.transform, false);
            var fr = (RectTransform)frame.transform;
            fr.sizeDelta = new Vector2(420, 720);
            frame.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.12f, 1);
            Label(_phone, "LINKDECK", new Vector2(0, 280), 32, FontStyle.Bold, new Color(0.35f, 0.95f, 1f));
            string[] apps = { "Map", "Messages", "Contacts", "Camera", "HackDeck", "Missions", "Inventory", "Settings", "Close" };
            for (int i = 0; i < apps.Length; i++)
            {
                int col = i % 3, row = i / 3; int idx = i;
                Button(_phone, new Vector2((col - 1) * 120, 180 - row * 120), apps[i], 100, 100, () =>
                {
                    if (apps[idx] == "Close") { _phone.SetActive(false); GameServices.Current?.Get<PhoneController>().Close(); return; }
                    GameServices.Current?.Get<PhoneController>().OpenScreen(apps[idx] switch
                    {
                        "Map" => PhoneScreen.Map, "Messages" => PhoneScreen.Messages, "Contacts" => PhoneScreen.Contacts,
                        "Camera" => PhoneScreen.Camera, "HackDeck" => PhoneScreen.HackDeck, "Missions" => PhoneScreen.Missions,
                        "Inventory" => PhoneScreen.Inventory, "Settings" => PhoneScreen.Settings, _ => PhoneScreen.Home
                    });
                });
            }
            _phone.SetActive(false);
        }

        private GameObject Panel(string name, Color bg, bool fullscreen = true)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(_canvas.transform, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            if (fullscreen) { var img = go.AddComponent<Image>(); img.color = bg; }
            return go;
        }

        private Text Label(GameObject parent, string text, Vector2 pos, int size, FontStyle style, Color color, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            var go = new GameObject($"L_{text}", typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(1400, size + 20); rt.anchoredPosition = pos;
            var t = go.AddComponent<Text>();
            t.text = text; t.fontSize = size; t.fontStyle = style; t.color = color; t.alignment = anchor; t.font = _font;
            return t;
        }

        private Button Button(GameObject parent, Vector2 pos, string text, int w, int h, System.Action onClick)
        {
            var go = new GameObject($"B_{text}", typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = pos;
            var img = go.AddComponent<Image>(); img.color = new Color(0.08f, 0.14f, 0.20f, 1);
            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.20f, 0.45f, 0.60f, 1f);
            colors.pressedColor = new Color(0.35f, 0.95f, 1f, 1f);
            btn.colors = colors; btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick?.Invoke());
            Label(go, text, Vector2.zero, Mathf.Max(18, h / 3), FontStyle.Bold, Color.white);
            return btn;
        }

        private InputField InputField(GameObject parent, Vector2 pos, string placeholder, string value, bool password = false)
        {
            var go = new GameObject($"I_{placeholder}", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent.transform, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(360, 50); rt.anchoredPosition = pos;
            go.GetComponent<Image>().color = new Color(0.10f, 0.14f, 0.20f, 1);
            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var tr = (RectTransform)textGo.transform;
            tr.anchorMin = new Vector2(0, 0); tr.anchorMax = new Vector2(1, 1);
            tr.offsetMin = new Vector2(12, 4); tr.offsetMax = new Vector2(-12, -4);
            var t = textGo.AddComponent<Text>();
            t.fontSize = 20; t.color = Color.white; t.alignment = TextAnchor.MiddleLeft; t.font = _font;
            var ph = new GameObject("Placeholder", typeof(RectTransform));
            ph.transform.SetParent(go.transform, false);
            var pr = (RectTransform)ph.transform;
            pr.anchorMin = new Vector2(0, 0); pr.anchorMax = new Vector2(1, 1);
            pr.offsetMin = new Vector2(12, 4); pr.offsetMax = new Vector2(-12, -4);
            var pt = ph.AddComponent<Text>();
            pt.text = placeholder; pt.fontSize = 20; pt.color = new Color(1, 1, 1, 0.4f); pt.alignment = TextAnchor.MiddleLeft; pt.font = _font;
            var input = go.AddComponent<InputField>();
            input.textComponent = t; input.placeholder = pt; input.text = value;
            if (password) { input.contentType = InputField.ContentType.Password; input.inputType = InputField.InputType.Password; }
            return input;
        }

        private Toggle Toggle(GameObject parent, Vector2 pos, string label)
        {
            var go = new GameObject($"T_{label}", typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            var rt = (RectTransform)go.transform; rt.sizeDelta = new Vector2(360, 40); rt.anchoredPosition = pos;
            var box = new GameObject("Box", typeof(RectTransform), typeof(Image));
            box.transform.SetParent(go.transform, false);
            var br = (RectTransform)box.transform; br.sizeDelta = new Vector2(28, 28); br.anchoredPosition = new Vector2(-150, 0);
            box.GetComponent<Image>().color = new Color(0.10f, 0.14f, 0.20f, 1);
            var check = new GameObject("Check", typeof(RectTransform), typeof(Image));
            check.transform.SetParent(box.transform, false);
            var cr = (RectTransform)check.transform; cr.anchorMin = Vector2.zero; cr.anchorMax = Vector2.one; cr.offsetMin = new Vector2(4, 4); cr.offsetMax = new Vector2(-4, -4);
            check.GetComponent<Image>().color = new Color(0.35f, 0.95f, 1f, 1f);
            Label(go, label, new Vector2(30, 0), 20, FontStyle.Normal, Color.white, TextAnchor.MiddleLeft);
            var t = go.AddComponent<Toggle>();
            t.graphic = check.GetComponent<Image>(); t.targetGraphic = box.GetComponent<Image>();
            return t;
        }

        private Dropdown Dropdown(GameObject parent, Vector2 pos, string label, string[] options)
        {
            Label(parent, label, pos + new Vector2(-200, 0), 20, FontStyle.Bold, Color.white, TextAnchor.MiddleLeft);
            var go = new GameObject($"D_{label}", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent.transform, false);
            var rt = (RectTransform)go.transform; rt.sizeDelta = new Vector2(240, 46); rt.anchoredPosition = pos + new Vector2(80, 0);
            go.GetComponent<Image>().color = new Color(0.10f, 0.14f, 0.20f, 1);
            var label2 = new GameObject("Label", typeof(RectTransform));
            label2.transform.SetParent(go.transform, false);
            var lr = (RectTransform)label2.transform; lr.anchorMin = new Vector2(0, 0); lr.anchorMax = new Vector2(1, 1); lr.offsetMin = new Vector2(10, 0); lr.offsetMax = new Vector2(-30, 0);
            var lt = label2.AddComponent<Text>(); lt.fontSize = 20; lt.color = Color.white; lt.alignment = TextAnchor.MiddleLeft; lt.font = _font;
            var dd = go.AddComponent<Dropdown>();
            dd.captionText = lt; dd.options.Clear();
            foreach (var o in options) dd.options.Add(new Dropdown.OptionData(o));
            dd.value = 0; dd.RefreshShownValue();
            return dd;
        }

        private Slider SliderRow(GameObject parent, Vector2 pos, string label, float min, float max, float value)
        {
            Label(parent, label, pos + new Vector2(-200, 0), 20, FontStyle.Bold, Color.white, TextAnchor.MiddleLeft);
            var s = HudSlider(parent, pos + new Vector2(80, 0), new Vector2(0.5f, 0.5f), 240, 8, new Color(0.35f, 0.95f, 1f));
            s.minValue = min; s.maxValue = max; s.value = value;
            return s;
        }

        private Slider HudSlider(GameObject parent, Vector2 pos, Vector2 anchor, int w, int h, Color color)
        {
            var go = new GameObject("Slider", typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = anchor; rt.anchorMax = anchor; rt.sizeDelta = new Vector2(w, h); rt.anchoredPosition = pos;
            var bg = new GameObject("BG", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(go.transform, false);
            var br = (RectTransform)bg.transform; br.anchorMin = Vector2.zero; br.anchorMax = Vector2.one; br.offsetMin = Vector2.zero; br.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = new Color(0.05f, 0.06f, 0.09f, 0.75f);
            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(go.transform, false);
            var fr = (RectTransform)fill.transform; fr.anchorMin = new Vector2(0, 0); fr.anchorMax = new Vector2(1, 1); fr.offsetMin = Vector2.zero; fr.offsetMax = Vector2.zero;
            fill.GetComponent<Image>().color = color;
            var s = go.AddComponent<Slider>();
            s.targetGraphic = bg.GetComponent<Image>(); s.fillRect = fr; s.minValue = 0; s.maxValue = 1; s.value = 1;
            return s;
        }

        private void CycleLanguage()
        {
            var loc = GameServices.Current?.Get<ILocalization>();
            if (loc == null) return;
            string[] order = { "en", "fr", "ar", "ja" };
            int idx = System.Array.IndexOf(order, loc.CurrentLanguage);
            idx = (idx + 1) % order.Length;
            loc.SetLanguage(order[idx]);
        }
    }
}
