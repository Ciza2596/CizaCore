using CizaCore;
using UnityEngine;

namespace Gizmos2DUtilsExample
{
	public class CapsuleMonoDrawer : MonoBehaviour
	{
		[SerializeField]
		private float _radius = 0.5f;

		[SerializeField]
		private float _height = 1;

		[SerializeField]
		private Color _color = Color.white;

		private void OnDrawGizmos() =>
			Gizmos2DUtils.DrawCapsule(transform.position, transform.rotation.eulerAngles.z, _radius, _height, _color);
	}
}
