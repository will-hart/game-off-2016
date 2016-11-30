#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : ZenSingleton<AudioManager>, IOnAwake
{
	public override int ExecutionPriority => 1000;
	public override Type ObjectType => typeof(AudioManager);

	public Dictionary<SfxTypes, AudioClip> SfxMapping;

	[SerializeField] private AudioSource audioSource2D;

	public void OnAwake()
	{
		SfxMapping = new Dictionary<SfxTypes, AudioClip>()
		{
			{SfxTypes.GunFire, Resources.Load("Audio/Sfx/Gunfire") as AudioClip},
		};

		audioSource2D = gameObject.AddComponent<AudioSource>();
		audioSource2D.loop = false;
		audioSource2D.playOnAwake = false;
		audioSource2D.spatialBlend = 0.0f;
	}

	/// <summary>
	/// Plays a one shot 2D sound effect.  This has no spatial processing at all
	/// </summary>
	/// <param name="sfx"></param>
	public void Trigger2DSfx(SfxTypes sfx)
	{
		audioSource2D.PlayOneShot(SfxMapping[sfx]);
	}
}

public enum SfxTypes
{
	GunFire,
	Explosion,
	UnitProduced
}