using CizaCore;
using NUnit.Framework;
using UnityEngine;

public class Bound2DUtilsTest
{
	private static readonly Vector2 _position = Vector2.zero;
	private static readonly Vector2 _size     = new Vector2(4, 4);

	private static readonly Vector2 _otherPosition = new Vector2(2, 2);
	private static readonly Vector2 _otherSize     = new Vector2(2, 2);

	private static readonly Vector2 _encapsulatedPosition = new Vector2(0.5f, 0.5f);
	private static readonly Vector2 _encapsulatedSize     = new Vector2(5, 5);

	private static readonly Vector2 _limitPosition = Vector2.zero;
	private static readonly Vector2 _limitSize     = new Vector2(3, 3);

	[Test]
	public void _01_GetTopLeftPosition()
	{
		var expectedTopLeftPosition = new Vector2(_position.x - _size.x / 2, _position.y + _size.y / 2);

		var topLeftPosition = Bound2DUtils.GetTopLeftPosition(_position, _size);
		Assert.AreEqual(expectedTopLeftPosition, topLeftPosition, $"TopLeftPosition: {topLeftPosition} should be {expectedTopLeftPosition}.");
	}

	[Test]
	public void _02_GetTopRightPosition()
	{
		var expectedTopRightPosition = new Vector2(_position.x + _size.x / 2, _position.y + _size.y / 2);

		var topRightPosition = Bound2DUtils.GetTopRightPosition(_position, _size);
		Assert.AreEqual(expectedTopRightPosition, topRightPosition, $"TopRightPosition: {topRightPosition} should be {expectedTopRightPosition}.");
	}

	[Test]
	public void _03_GetBottomLeftPosition()
	{
		var expectedBottomLeftPosition = new Vector2(_position.x - _size.x / 2, _position.y - _size.y / 2);

		var bottomLeftPosition = Bound2DUtils.GetBottomLeftPosition(_position, _size);
		Assert.AreEqual(expectedBottomLeftPosition, bottomLeftPosition, $"BottomLeftPosition: {bottomLeftPosition} should be {expectedBottomLeftPosition}.");
	}

	[Test]
	public void _04_GetBottomRightPosition()
	{
		var expectedBottomRightPosition = new Vector2(_position.x + _size.x / 2, _position.y - _size.y / 2);

		var bottomRightPosition = Bound2DUtils.GetBottomRightPosition(_position, _size);
		Assert.AreEqual(expectedBottomRightPosition, bottomRightPosition, $"BottomRightPosition: {bottomRightPosition} should be {expectedBottomRightPosition}.");
	}

	[Test]
	public void _05_Limit()
	{
		(var position, var size) = Bound2DUtils.Limit(_position, _size, _limitPosition, _limitSize);

		Assert.AreEqual(_limitPosition, position, $"Position: {position} should be {_limitPosition}.");
		Assert.AreEqual(_limitSize, size, $"Size: {size} should be {_limitSize}.");
	}

	[Test]
	public void _06_Encapsulate()
	{
		(var position, var size) = Bound2DUtils.Encapsulate(_position, _size, _otherPosition, _otherSize);

		Assert.AreEqual(_encapsulatedPosition, position, $"Position: {position} should be {_encapsulatedPosition}.");
		Assert.AreEqual(_encapsulatedSize, size, $"Size: {size} should be {_encapsulatedSize}.");
	}
}
