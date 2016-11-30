#pragma warning disable 0414, 0219, 649, 169, 1570
using System.Runtime.InteropServices;
using UnityEngine;

public class MathApprox
{
	public static float FastSqrt(float z)
	{
		if (z == 0) return 0;
		FloatIntUnion u;
		u.tmp = 0;
		u.f = z;
		u.tmp -= 1 << 23; /* Subtract 2^m. */
		u.tmp >>= 1; /* Divide by 2. */
		u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
		return u.f;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int tmp;
	}

	public static float FindFastApproxDistance(float x1, float y1, float x2, float y2)
	{
		return FastSqrt( (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) );
	}

	public static float FindFastApproxDistance(Vector2 first, Vector2 second)
	{
		return FastSqrt((second.x - first.x) * (second.x - first.x) + (second.y - first.y) * (second.y - first.y));
	}

	public static float FindFastApproxDistance(GameObject first, GameObject second)
	{
		return FindFastApproxDistance(first.transform.position, second.transform.position);
	}

	public static float FindFasterApproxDistanceSquared(Vector2 first, Vector2 second)
	{
		return (second.x - first.x) * (second.x - first.x) + (second.y - first.y) * (second.y - first.y);
	}

	public static float FindFasterApproxDistanceSquared(float x1, float y1, float x2, float y2)
	{
		return (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
	}

	
}