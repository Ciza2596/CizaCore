using UnityEngine;

namespace CizaCore
{
    public static class PlayerUtils
    {
        public static string GetPlayerInfo() =>
            GetLanguageAndPlatformInfo() + $"{GetVersionConfig().Version}\n";

        public static string GetLanguageAndPlatformInfo()
        {
            var language = $"Language: {Application.systemLanguage}.\n";
            var platform = $"Platform: {Application.platform}.\n";

            return language + platform;
        }

        public static bool CheckIsDev() =>
            GetVersionConfig().VersionKind.CheckIsDev();

        public static bool CheckIsDemo() =>
            GetVersionConfig().VersionKind.CheckIsDemo();

        public static bool CheckIsMain() =>
            GetVersionConfig().VersionKind.CheckIsMain();


        public static IVersionConfig GetVersionConfig() =>
            Resources.Load<ScriptableObject>("Ciza/VersionConfig") as IVersionConfig;
    }
}