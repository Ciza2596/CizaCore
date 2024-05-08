namespace CizaCore
{
    public interface IVersionConfig
    {
        VersionKinds VersionKind { get; }

        int Major { get; }

        int Minor { get; }

        int Patch { get; }

        string Version => $"Version: {VersionKind} - {Major}.{Minor}.{Patch}";
    }
}