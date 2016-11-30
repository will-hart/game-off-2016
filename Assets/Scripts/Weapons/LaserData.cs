#pragma warning disable 0414, 0219
using UnityEngine;

public class LaserData : MonoBehaviour
{
	public float laserDistance = 10;
	public float laserSpeed = 2;
	public float laserShieldDamage = 10;
	public float laserHullDamage = 10;
	public float laserPrecision = 1;
	public float laserForce = 1;
	public float laserForceMult = 0.1f;
	public float laserFireRate = 1.0f;
	public bool continuousLaser;
}
