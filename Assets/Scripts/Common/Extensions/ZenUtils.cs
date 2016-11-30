using System;
using UnityEngine;
using System.Collections;

public static class ZenUtils
{
	public static System.Random RndGen = new System.Random();

	public static Vector3 GetTerrainPositionFromCursor(int terrainLayerMask)
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, terrainLayerMask);
		
		return hitInfo.point;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	public static float MaxVectorElement(Vector3 v)
	{
		return Mathf.Max(v.x, v.y, v.z); 
	}

	//Measures distance in clockwise and counterclockwise directions between angles in radians and returns smallest
	public static float ShortestAngleBetween(float anglestart, float anglefinish)
	{
		float counterClockwiseDistance, clockwiseDistance;//Counterclockwise
		if (anglefinish < anglestart)
		{
			clockwiseDistance = anglestart - anglefinish;
			counterClockwiseDistance = Mathf.PI * 2 - anglestart + anglefinish;
		}
		else
		{
			clockwiseDistance = Mathf.PI * 2 - anglefinish + anglestart;
			counterClockwiseDistance = anglefinish - anglestart;
		}
		if (counterClockwiseDistance < clockwiseDistance) return counterClockwiseDistance;
		else return -clockwiseDistance;
	}
	//Checks angle in radians between vectors. Positive is counterClockwise, negative is clockwise.
	public static float AngleBetweenVectors(Vector2 first, Vector2 second)
	{
		float ang1 = Mathf.Atan2(first.y, first.x), ang2 = Mathf.Atan2(second.y, second.x);
		return ShortestAngleBetween(ang1, ang2);
	}

	public static Quaternion RotationToTarget2D(Vector3 objectToRotate, Vector3 targetPos)
	{
		Vector3 diff = targetPos - objectToRotate;
		diff.Normalize();

		float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
		return Quaternion.Euler(0f, 0f, rot_z - 90);
	}

	public static long RandomLong()
	{
		byte[] buffer = new byte[8];
		RndGen.NextBytes(buffer);
		return BitConverter.ToInt64(buffer, 0);

	}
}
