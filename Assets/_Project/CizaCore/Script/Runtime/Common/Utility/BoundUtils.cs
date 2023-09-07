using UnityEngine;

namespace CizaCore
{
	public class BoundUtils
	{
		public static (Vector2, Vector2) Expand(Vector2 boundPosition, Vector2 boundSize, Vector2 addBoundPosition, Vector2 addBoundSize)
		{
			var bounds = new Bounds(boundPosition, boundSize);

			var halfWidth  = addBoundSize.x / 2;
			var halfHeight = addBoundSize.y / 2;

			var topLeft = new Vector2(addBoundPosition.x - halfWidth, addBoundPosition.x + halfHeight);
			bounds.Expand(topLeft);

			var topRight = new Vector2(addBoundPosition.x + halfWidth, addBoundPosition.x + halfHeight);
			bounds.Expand(topRight);

			var bottomLeft = new Vector2(addBoundPosition.x - halfWidth, addBoundPosition.x - halfHeight);
			bounds.Expand(bottomLeft);

			var bottomRight = new Vector2(addBoundPosition.x + halfWidth, addBoundPosition.x - halfHeight);
			bounds.Expand(bottomRight);

			var center  = bounds.center;
			var extents = bounds.extents;
			return (new Vector2(center.x, center.y), new Vector2(extents.x, extents.y));
		}
	}
}
