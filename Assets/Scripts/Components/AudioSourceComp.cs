using GameGHJ.Common.ZenECS;
using UnityEngine;

public class AudioSourceComp : ComponentECS
{
	public override ComponentTypes ComponentType =>  ComponentTypes.AudioSourceComp;

	private AudioSource _audioSource;

	public AudioSource audioSource
	{
		get { return _audioSource; }
		set
		{
			_audioSource = value;
			_audioSource.playOnAwake = false;
			_audioSource.loop = false;
			_audioSource.spatialBlend = 1.0f;
			_audioSource.volume = 1.0f;
			_audioSource.panStereo = 0.5f;
			_audioSource.dopplerLevel = 0.0f;
		}
	}

	public AudioSourceComp() : base()
	{
	}

	

	/// <summary>
	/// Triggers a positional sound effect centered at this object's location
	/// </summary>
	/// <param name="sfx"></param>
	public void TriggerSfx(SfxTypes sfx)
	{
		_audioSource.PlayOneShot(AudioManager.Instance.SfxMapping[sfx]);
	}
	
}
