using CizaCore;
using UnityEngine;

namespace Gizmos2DUtilsExample
{
	public class RectangleMonoDrawer : MonoBehaviour
	{
		[SerializeField]
		private Vector2 _size = Vector2.one;

		[SerializeField]
		private Color _color = Color.white;

		private void OnDrawGizmos() =>
			Gizmos2DUtils.DrawRectangle(transform.position, transform.rotation.eulerAngles.z, _size, _color);
	}
}
