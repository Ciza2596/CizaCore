using UnityEngine;

namespace CizaCore
{
    [CreateAssetMenu(fileName = "VersionConfig", menuName = "Ciza/VersionConfig")]
    public class VersionConfig : ScriptableObject, IVersionConfig
    {
        [SerializeField]
        private VersionKinds _versionKind;

        [Space]
        [SerializeField]
        private int _major;

        [SerializeField]
        private int _minor;

        [SerializeField]
        private int _patch;

        public VersionKinds VersionKind => _versionKind;
        
        public int Major => _major;
        public int Minor => _minor;
        public int Patch => _patch;
    }
}