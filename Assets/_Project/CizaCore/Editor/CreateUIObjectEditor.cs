using UnityEditor;
using UnityEngine;

namespace CizaCore.Editor
{
    public class CreateUIObjectEditor
    {
        public const string CizaCorePath = "CizaCore/";
        public const string UIPath = "UI/";

        public const string VerticalScrollView = "VerticalScrollView";

        [MenuItem("GameObject/Ciza/UI/VerticalScrollView", false, 10)]
        public static void CreateVerticalScrollView()
        {
            CreateUIObject(VerticalScrollView);
        }

        public const string Dropdown = "Dropdown";

        [MenuItem("GameObject/Ciza/UI/Dropdown", false, 10)]
        public static void CreateDropdown()
        {
            CreateUIObject(Dropdown);
        }

        private static void CreateUIObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(CizaCorePath + UIPath + dataId);
            var uiObject = Object.Instantiate(prefab, Selection.activeTransform);
            uiObject.name = dataId;
        }
    }
}