// Neon Cipher — Editor helper: auto-add Main scene to Build Settings and
// provide menu items to (re)create a boot scene if the .unity gets corrupted.
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NeonCipher.EditorTools
{
    [InitializeOnLoad]
    public static class NeonCipherProjectSetup
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";

        static NeonCipherProjectSetup()
        {
            EditorApplication.delayCall += EnsureBuildSettings;
        }

        [MenuItem("Neon Cipher/Generate Main Scene")]
        public static void GenerateMainScene()
        {
            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var boot = new GameObject("[Playable Bootstrap]");
            boot.AddComponent<NeonCipher.Core.PlayableBootstrap>();
            EditorSceneManager.MoveGameObjectToScene(boot, scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureBuildSettings();
            Debug.Log("[NeonCipher] Main.unity regenerated.");
        }

        [MenuItem("Neon Cipher/Open Main Scene")]
        public static void OpenMain()
        {
            if (File.Exists(ScenePath))
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            else
                GenerateMainScene();
        }

        private static void EnsureBuildSettings()
        {
            if (!File.Exists(ScenePath)) return;
            var scenes = EditorBuildSettings.scenes;
            foreach (var s in scenes) if (s.path == ScenePath) return;
            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes)
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            EditorBuildSettings.scenes = list.ToArray();
        }
    }
}
#endif
