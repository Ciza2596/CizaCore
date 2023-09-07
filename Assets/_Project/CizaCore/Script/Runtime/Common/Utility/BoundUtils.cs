using UnityEngine;

namespace CizaCore
{
	public class BoundUtils
	{
		public static (Vector2, Vector2) Expand(Vector2 boundPosition, Vector2 boundSize, Vector2 addBoundPosition, Vector2 addBoundSize)
		{
			var bounds = new Bounds(boundPosition, boundSize);

			var topLeft = GetTopLeft(addBoundPosition, addBoundSize);
			bounds.Expand(topLeft);

			var topRight = GetTopRight(addBoundPosition, addBoundSize);
			bounds.Expand(topRight);

			var bottomLeft = GetBottomLeft(addBoundPosition, addBoundSize);
			bounds.Expand(bottomLeft);

			var bottomRight = GetBottomRight(addBoundPosition, addBoundSize);
			bounds.Expand(bottomRight);

			var center  = bounds.center;
			var extents = bounds.extents;
			return (new Vector2(center.x, center.y), new Vector2(extents.x, extents.y));
		}

		public static Vector2 GetTopLeft(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x - halfWidth, position.x + halfHeight);
		}

		public static Vector2 GetTopRight(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x + halfWidth, position.x + halfHeight);
		}

		public static Vector2 GetBottomLeft(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x - halfWidth, position.x - halfHeight);
		}

		public static Vector2 GetBottomRight(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x + halfWidth, position.x - halfHeight);
		}
	}
}
