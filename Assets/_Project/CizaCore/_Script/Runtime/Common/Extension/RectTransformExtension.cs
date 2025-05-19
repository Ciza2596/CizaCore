using UnityEngine;

namespace CizaCore
{
	public static class RectTransformExtension
	{
		public const float PIVOT_CENTER = 0.5f;

		public static Vector2 GetScreenPoint(this Transform transform, Camera camera = null)
		{
			var uiCamera = camera ?? Camera.main;
			var screenPoint = uiCamera.WorldToScreenPoint(transform.position);
			return screenPoint;
		}

		public static Rect GetRect(this RectTransform rectTransform)
		{
			var canvas = rectTransform.GetComponentInParent<Canvas>();
			var rectTransformPosition = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera.WorldToScreenPoint(rectTransform.position) : rectTransform.position;
			return new Rect(rectTransformPosition.x, rectTransformPosition.y, rectTransform.rect.width, rectTransform.rect.height);
		}

		public static Vector2 GetCenterPosition(this RectTransform rectTransform)
		{
			var canvas = rectTransform.GetComponentInParent<Canvas>();
			var rectTransformPosition = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera.WorldToScreenPoint(rectTransform.position) : rectTransform.position;
			var rectTransformRect = rectTransform.rect;

			var pivot = rectTransform.pivot;
			var diffPivotX = PIVOT_CENTER - pivot.x;
			var diffPivotY = PIVOT_CENTER - pivot.y;

			return new Vector2(rectTransformPosition.x + diffPivotX * rectTransformRect.width, rectTransformPosition.y + diffPivotY * rectTransformRect.height);
		}

		public static Vector2 GetScreenPoint(this RectTransform rectTransform, Camera camera = null)
		{
			var uiCamera = camera is null ? Camera.main : camera;
			var screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.transform.position);
			return screenPoint;
		}

		public static float GetLeftWidth(this RectTransform rectTransform)
		{
			var width = rectTransform.rect.width;
			var leftPivotX = rectTransform.pivot.x;

			var leftWidth = width * leftPivotX;
			return leftWidth;
		}

		public static float GetRightWidth(this RectTransform rectTransform)
		{
			var width = rectTransform.rect.width;
			var rightPivotX = (1 - rectTransform.pivot.x);

			var rightWidth = width * rightPivotX;
			return rightWidth;
		}

		public static float GetUpperHeight(this RectTransform rectTransform)
		{
			var height = rectTransform.rect.height;
			var upperPivotY = (1 - rectTransform.pivot.y);

			var upperHeight = height * upperPivotY;
			return upperHeight;
		}

		public static float GetLowerHeight(this RectTransform rectTransform)
		{
			var height = rectTransform.rect.height;
			var lowerPivotY = rectTransform.pivot.y;

			var lowerHeight = height * lowerPivotY;
			return lowerHeight;
		}
	}
}