using CizaCore;
using UnityEngine;

public class StringToArry : MonoBehaviour
{
	[SerializeField]
	private string _str;

	private void OnEnable()
	{
		var str = string.Empty;

		foreach (var item in _str.ToArray())
			str += item;

		Debug.Log(str);
	}
}
