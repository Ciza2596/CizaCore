using UnityEditor;
using UnityEngine;

namespace CizaCore.Editor
{
	public class OpenBuildMenu : MonoBehaviour
	{
		public const string BuildPath = "Build";

		[MenuItem("Tools/Ciza/OpenAddressablesAaFolder", false, 10000)]
		private static void OpenBuildFolder()
		{
			var fullPath = Application.dataPath.Replace("Assets", "") + BuildPath;

			if (System.IO.Directory.Exists(fullPath))
				EditorUtility.RevealInFinder(fullPath);

			else
				Debug.LogWarning($"The path: {fullPath} is not exist.");
		}
	}
}
