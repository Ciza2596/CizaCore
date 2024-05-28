namespace CizaCore
{
    public static class VersionKindsExtension
    {
        public static bool CheckIsDev(this VersionKinds versionKind) =>
            versionKind == VersionKinds.Dev;

        public static bool CheckIsDemo(this VersionKinds versionKind) =>
            versionKind == VersionKinds.Demo;

        public static bool CheckIsMain(this VersionKinds versionKind) =>
            versionKind == VersionKinds.Main;
    }
}