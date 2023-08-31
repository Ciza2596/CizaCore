using UnityEngine;

namespace CizaCore
{
	public static class Gizmos2DUtils
	{
		public const float Theta = 0.01f;

		public static void DrawCircle(Vector2 position, float radius, Color color, float scale = 1) =>
			DrawArc(position, 0, radius, 360, color, scale, true);

		public static void DrawArc(Vector2 position, float eulerAngle, float radius, float arcEulerAngle, Color color, float scale = 1, bool isRadiusLineHided = false, float theta = Theta)
		{
			Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(new Vector3(0, 0, eulerAngle)), new Vector3(scale, scale, scale));
			Gizmos.color  = color;

			DrawArc(Vector2.zero, radius, arcEulerAngle, isRadiusLineHided, theta);
		}

		public static void DrawRectangle(Vector2 position, float eulerAngle, Vector2 size, Color color, float scale = 1)
		{
			Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(new Vector3(0, 0, eulerAngle)), new Vector3(scale, scale, scale));

			DrawRectangle(new Vector2(-size.x / 2, size.y / 2), new Vector2(-size.x / 2, -size.y / 2), new Vector2(size.x / 2, -size.y / 2), new Vector2(size.x / 2, size.y / 2), color);
		}

		public static void DrawRectangle(Vector3 topLeft, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight, Color color)
		{
			Gizmos.color = color;

			Gizmos.DrawLine(topLeft, bottomLeft);
			Gizmos.DrawLine(bottomLeft, bottomRight);
			Gizmos.DrawLine(bottomRight, topRight);
			Gizmos.DrawLine(topRight, topLeft);
		}

		public static void DrawCapsule(Vector2 position, float eulerAngle, float radius, float height, Color color, float scale = 1)
		{
			Gizmos.color  = color;
			Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(new Vector3(0, 0, eulerAngle + 180)), new Vector3(scale, scale, scale));
			DrawArc(new Vector2(-radius, 0), radius, 180, true, Theta);

			var heightPosition = radius + height;
			Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(new Vector3(0, 0, eulerAngle)), new Vector3(scale, scale, scale));
			Gizmos.DrawLine(new Vector2(radius, radius), new Vector2(heightPosition, radius));
			Gizmos.DrawLine(new Vector2(radius, -radius), new Vector2(heightPosition, -radius));

			DrawArc(new Vector2(heightPosition, 0), radius, 180, true, Theta);
		}

		private static Vector2 GetArcPosition(Vector2 position, float radius, float theta) =>
			position + new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));

		public static void DrawArc(Vector2 position, float radius, float arcEulerAngle, bool isRadiusLineHided, float theta)
		{
			var halfRad = Mathf.Deg2Rad * arcEulerAngle / 2;

			var firstPoint = GetArcPosition(position, radius, -halfRad);
			var beginPoint = firstPoint;

			if (!isRadiusLineHided)
				Gizmos.DrawLine(Vector2.zero, firstPoint);

			for (float t = -halfRad; t <= halfRad; t += theta)
			{
				var endPoint = GetArcPosition(position, radius, t);
				Gizmos.DrawLine(beginPoint, endPoint);
				beginPoint = endPoint;
			}

			if (!isRadiusLineHided)
				Gizmos.DrawLine(beginPoint, position);
		}

		public static void DrawArc(Matrix4x4 matrix4X4, Vector2 position, float eulerAngle, float radius, float arcEulerAngle, Color color, float scale = 1, bool isRadiusLineHided = false, float theta = Theta)
		{
			Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.Euler(new Vector3(0, 0, eulerAngle)), new Vector3(scale, scale, scale)) * matrix4X4;
			Gizmos.color  = color;

			DrawArc(Vector2.zero, radius, arcEulerAngle, isRadiusLineHided, theta);
		}
	}
}
