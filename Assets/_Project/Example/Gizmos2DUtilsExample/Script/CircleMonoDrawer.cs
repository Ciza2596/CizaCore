using CizaCore;
using UnityEngine;

namespace Gizmos2DUtilsExample
{
	public class CircleMonoDrawer : MonoBehaviour
	{
		[SerializeField]
		private float _radius = 0.5f;

		[SerializeField]
		private Color _color = Color.white;

		private void OnDrawGizmos() =>
			Gizmos2DUtils.DrawCircle(transform.position, _radius, _color);
	}
}
