using UnityEngine;

// ReSharper disable once CheckNamespace
public static partial class Extensions
{
	public static Vector3 Back(this Quaternion @this)
	{
		return @this * Vector3.back;
	}
	public static Vector3 Down(this Quaternion @this)
	{
		return @this * Vector3.down;
	}
	public static Vector3 Forward(this Quaternion @this)
	{
		return @this * Vector3.forward;
	}
	public static bool IsNaN(this Quaternion @this)
	{
		return float.IsNaN(@this.x) || float.IsNaN(@this.y) || float.IsNaN(@this.z) || float.IsNaN(@this.w);
	}
	public static Vector3 Left(this Quaternion @this)
	{
		return @this * Vector3.left;
	}
	public static Vector3 Right(this Quaternion @this)
	{
		return @this * Vector3.right;
	}
	public static Vector3 Up(this Quaternion @this)
	{
		return @this * Vector3.up;
	}
}
