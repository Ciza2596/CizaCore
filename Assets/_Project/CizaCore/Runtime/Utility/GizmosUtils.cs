using UnityEngine;

namespace CizaCore
{
	public static class GizmosUtils
	{
		public static void DrawCircle(Vector2 position, float radius, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawWireSphere(position, radius);
		}

		public static void DrawRectangleWithPivotAtCenter(Vector2 position, Vector2 size, Color color) =>
			DrawRectangle(position + new Vector2(-size.x / 2, size.y / 2), position + new Vector2(-size.x / 2, -size.y / 2), position + new Vector2(size.x / 2, -size.y / 2), position + new Vector2(size.x / 2, size.y / 2), color);

		public static void DrawRectangleWithPivotAtTopLeft(Vector2 position, Vector2 size, Color color) =>
			DrawRectangle(position, position + new Vector2(0, -size.y), position + new Vector2(size.x, -size.y), position + new Vector2(size.x, 0), color);

		public static void DrawRectangle(Vector3 topLeft, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight, Color color)
		{
			Gizmos.color = color;
			Gizmos.DrawLine(topLeft, bottomLeft);
			Gizmos.DrawLine(bottomLeft, bottomRight);
			Gizmos.DrawLine(bottomRight, topRight);
			Gizmos.DrawLine(topRight, topLeft);
		}
	}
}
