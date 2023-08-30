using CizaCore;
using UnityEngine;

namespace Gizmos2DUtilsExample
{
	public class ArcMonoDrawer : MonoBehaviour
	{
		[SerializeField]
		private float _radius = 0.5f;

		[SerializeField]
		private float _eulerAngle = 30;

		[SerializeField]
		private Color _color = Color.white;

		private void OnDrawGizmos() =>
			Gizmos2DUtils.DrawArc(transform.position, transform.rotation.eulerAngles.z, _radius, _eulerAngle, _color);
	}
}
