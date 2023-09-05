using UnityEngine;

namespace CizaCore
{
	public static class RectTransformExtension
	{
		public static Vector2 GetScreenPoint(this Transform transform, Camera camera = null)
		{
			var uiCamera    = camera is null ? Camera.main : camera;
			var screenPoint = uiCamera.WorldToScreenPoint(transform.position);
			return screenPoint;
		}

		public static Vector2 GetScreenPoint(this RectTransform rectTransform, Camera camera = null)
		{
			var uiCamera    = camera is null ? Camera.main : camera;
			var screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.transform.position);
			return screenPoint;
		}

		public static float GetLeftWidth(this RectTransform rectTransform)
		{
			var width      = rectTransform.rect.width;
			var leftPivotX = rectTransform.pivot.x;

			var leftWidth = width * leftPivotX;
			return leftWidth;
		}

		public static float GetRightWidth(this RectTransform rectTransform)
		{
			var width       = rectTransform.rect.width;
			var rightPivotX = (1 - rectTransform.pivot.x);

			var rightWidth = width * rightPivotX;
			return rightWidth;
		}

		public static float GetUpperHeight(this RectTransform rectTransform)
		{
			var height      = rectTransform.rect.height;
			var upperPivotY = (1 - rectTransform.pivot.y);

			var upperHeight = height * upperPivotY;
			return upperHeight;
		}

		public static float GetLowerHeight(this RectTransform rectTransform)
		{
			var height      = rectTransform.rect.height;
			var lowerPivotY = rectTransform.pivot.y;

			var lowerHeight = height * lowerPivotY;
			return lowerHeight;
		}
	}
}
