using UnityEngine;

namespace CizaCore
{
	public class BoundUtils
	{
		public static (Vector2, Vector2) Encapsulate(Vector2 boundPosition, Vector2 boundSize, Vector2 addBoundPosition, Vector2 addBoundSize)
		{
			var bounds    = new Bounds(boundPosition, boundSize);
			var addBounds = new Bounds(addBoundPosition, addBoundSize);

			var mergedBounds = new Bounds(Vector3.zero, Vector3.zero);
			mergedBounds.Encapsulate(bounds);
			mergedBounds.Encapsulate(addBounds);

			var center  = bounds.center;
			var extents = bounds.extents;
			return (new Vector2(center.x, center.y), new Vector2(extents.x, extents.y));
		}

		public static Vector2 GetTopLeftPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x - halfWidth, position.x + halfHeight);
		}

		public static Vector2 GetTopRightPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x + halfWidth, position.x + halfHeight);
		}

		public static Vector2 GetBottomLeftPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x - halfWidth, position.x - halfHeight);
		}

		public static Vector2 GetBottomRightPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x + halfWidth, position.x - halfHeight);
		}
	}
}
