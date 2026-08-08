// Neon Cipher — Editor build helpers: Windows64 / Android APK / Android AAB
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NeonCipher.EditorTools
{
    public static class NeonCipherBuild
    {
        private const string SceneMain = "Assets/Scenes/Main.unity";

        [MenuItem("Neon Cipher/Build/Windows64")]
        public static void BuildWindows64() => Do(BuildTarget.StandaloneWindows64,
            "Builds/Windows64/NeonCipher.exe", BuildOptions.None);

        [MenuItem("Neon Cipher/Build/Android APK (Debug)")]
        public static void BuildAndroidApk()
        {
            EditorUserBuildSettings.buildAppBundle = false;
            Do(BuildTarget.Android, "Builds/Android/NeonCipher-Debug.apk",
               BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        [MenuItem("Neon Cipher/Build/Android AAB (Release)")]
        public static void BuildAndroidAab()
        {
            EditorUserBuildSettings.buildAppBundle = true;
            Do(BuildTarget.Android, "Builds/Android/NeonCipher-Release.aab", BuildOptions.None);
        }

        private static void Do(BuildTarget target, string outPath, BuildOptions opts)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            var opt = new BuildPlayerOptions
            {
                scenes = new[] { SceneMain },
                locationPathName = outPath,
                target = target,
                options = opts
            };
            var report = BuildPipeline.BuildPlayer(opt);
            if (report.summary.result == BuildResult.Succeeded)
                Debug.Log($"[NeonCipher] Build OK: {outPath} ({report.summary.totalSize} bytes)");
            else
                Debug.LogError($"[NeonCipher] Build FAILED: {report.summary.result}");
        }
    }
}
#endif
