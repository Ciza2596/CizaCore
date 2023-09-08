using UnityEngine;

namespace CizaCore
{
	public class Bound2DUtils
	{
		public static Vector2 GetTopLeftPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x - halfWidth, position.y + halfHeight);
		}

		public static Vector2 GetTopRightPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x + halfWidth, position.y + halfHeight);
		}

		public static Vector2 GetBottomLeftPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x - halfWidth, position.y - halfHeight);
		}

		public static Vector2 GetBottomRightPosition(Vector2 position, Vector2 size)
		{
			var halfWidth  = size.x / 2;
			var halfHeight = size.y / 2;
			return new Vector2(position.x + halfWidth, position.y - halfHeight);
		}

		public static (Vector2, Vector2) Limit(Vector2 boundPosition, Vector2 boundSize, Vector2 limitBoundPosition, Vector2 limitBoundSize)
		{
			var m_topLeftPosition      = GetTopLeftPosition(boundPosition, boundSize);
			var m_limitTopLeftPosition = GetTopLeftPosition(limitBoundPosition, limitBoundSize);

			var m_targetTopLeftPosition = new Vector2(m_topLeftPosition.x >= m_limitTopLeftPosition.x ? m_topLeftPosition.x : m_limitTopLeftPosition.x, m_topLeftPosition.y <= m_limitTopLeftPosition.y ? m_topLeftPosition.y : m_limitTopLeftPosition.y);


			var m_topRightPosition      = GetTopRightPosition(boundPosition, boundSize);
			var m_limitTopRightPosition = GetTopRightPosition(limitBoundPosition, limitBoundSize);

			var m_targetTopRightPosition = new Vector2(m_topRightPosition.x <= m_limitTopRightPosition.x ? m_topRightPosition.x : m_limitTopRightPosition.x, m_topRightPosition.y <= m_limitTopRightPosition.y ? m_topRightPosition.y : m_limitTopRightPosition.y);


			var m_bottomLeftPosition      = GetBottomLeftPosition(boundPosition, boundSize);
			var m_limitBottomLeftPosition = GetBottomLeftPosition(limitBoundPosition, limitBoundSize);

			var m_targetBottomLeftPosition = new Vector2(m_bottomLeftPosition.x >= m_limitBottomLeftPosition.x ? m_bottomLeftPosition.x : m_limitBottomLeftPosition.x, m_bottomLeftPosition.y >= m_limitBottomLeftPosition.y ? m_bottomLeftPosition.y : m_limitBottomLeftPosition.y);


			var m_bottomRightPosition      = GetBottomRightPosition(boundPosition, boundSize);
			var m_limitBottomRightPosition = GetBottomRightPosition(limitBoundPosition, limitBoundSize);

			var m_targetBottomRightPosition = new Vector2(m_bottomRightPosition.x <= m_limitBottomRightPosition.x ? m_bottomRightPosition.x : m_limitBottomRightPosition.x, m_bottomRightPosition.y >= m_limitBottomRightPosition.y ? m_bottomRightPosition.y : m_limitBottomRightPosition.y);

			var maxX = m_targetTopRightPosition.x >= m_targetBottomRightPosition.x ? m_targetTopRightPosition.x : m_targetBottomRightPosition.x;
			var minX = m_targetTopLeftPosition.x  <= m_targetBottomLeftPosition.x ? m_targetTopLeftPosition.x : m_targetBottomLeftPosition.x;

			var maxY = m_targetTopLeftPosition.y    >= m_targetTopRightPosition.y ? m_targetTopLeftPosition.y : m_targetTopRightPosition.y;
			var minY = m_targetBottomLeftPosition.y <= m_targetBottomRightPosition.y ? m_targetBottomLeftPosition.y : m_targetBottomRightPosition.y;

			var size = new Vector2(maxX                      - minX, maxY                               - minY);
			return (new Vector2(m_targetBottomLeftPosition.x + size.x / 2, m_targetBottomLeftPosition.y + size.y / 2), size);
		}

		public static (Vector2, Vector2) Encapsulate(Vector2 boundPosition, Vector2 boundSize, Vector2 addBoundPosition, Vector2 addBoundSize)
		{
			var bounds    = new Bounds(boundPosition, boundSize);
			var addBounds = new Bounds(addBoundPosition, addBoundSize);

			var mergedBounds = new Bounds(Vector3.zero, Vector3.zero);
			mergedBounds.Encapsulate(bounds);
			mergedBounds.Encapsulate(addBounds);

			return (mergedBounds.center, mergedBounds.extents * 2);
		}
	}
}
