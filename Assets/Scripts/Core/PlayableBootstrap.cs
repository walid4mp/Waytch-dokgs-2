// Neon Cipher — Playable Bootstrap (entry point in Main.unity)
using System.Collections;
using NeonCipher.UI;
using NeonCipher.World;
using UnityEngine;

namespace NeonCipher.Core
{
    [DefaultExecutionOrder(-500)]
    public sealed class PlayableBootstrap : MonoBehaviour
    {
        private RuntimeUiBuilder _ui;
        private GameSceneComposer _scene;
        private bool _gameStarted;

        private void Awake()
        {
            if (GameServices.Current == null) gameObject.AddComponent<GameBootstrap>();
            var uiGo = new GameObject("[Runtime UI]");
            uiGo.transform.SetParent(transform, false);
            _ui = uiGo.AddComponent<RuntimeUiBuilder>();
            _ui.OnStartGameRequested += StartGame;
            _ui.OnResumeGameRequested += ResumeGame;
            _ui.OnExitGameRequested += ExitGame;
            StartCoroutine(BootFlow());
        }

        private IEnumerator BootFlow()
        {
            _ui.Show(UiState.Splash);
            yield return new WaitForSeconds(1.2f);
            _ui.Show(UiState.Loading);
            for (float t = 0; t < 1f; t += Time.deltaTime * 0.75f)
            {
                _ui.SetLoadingProgress(t, t < 0.4f ? "loading city…" : t < 0.75f ? "wiring input…" : "ready");
                yield return null;
            }
            _ui.SetLoadingProgress(1f, "done");
            yield return new WaitForSeconds(0.3f);
            _ui.Show(UiState.Login);
        }

        private void StartGame()
        {
            if (_gameStarted) { _ui.Show(UiState.InGame); return; }
            _gameStarted = true;
            var sceneGo = new GameObject("[Scene Composer]");
            _scene = sceneGo.AddComponent<GameSceneComposer>();
            _scene.BuildAll(_ui.Hud);
            _ui.Show(UiState.InGame);
        }

        private void ResumeGame() => _ui.Show(UiState.InGame);
        private void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void Update()
        {
            if (!_gameStarted) return;
#if ENABLE_INPUT_SYSTEM
            var kb = UnityEngine.InputSystem.Keyboard.current;
            if (kb == null) return;
            if (kb.escapeKey.wasPressedThisFrame) _ui.Show(UiState.Pause);
            if (kb.tabKey.wasPressedThisFrame) _ui.TogglePhone();
#endif
        }
    }
}
