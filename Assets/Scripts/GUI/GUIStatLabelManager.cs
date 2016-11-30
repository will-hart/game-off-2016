using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using MEC;

public class GUIStatLabelManager : ZenSingleton<GUIStatLabelManager>, IOnStart
{
	// Use this for initialization
	public override int ExecutionPriority => 10000;
	public override Type ObjectType => typeof(GUIStatLabelManager);

	[SerializeField]
	private TextMeshPro DNAText, RNAText, NotifyText;
	//private int currentDNA, currentRNA;
	private SidePropertiesComp playerProps;

	public void OnStart()
	{
		playerProps = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp).First(x => x.IsPlayerControlled == true);
		//currentDNA = playerProps.Dna;
		//currentRNA = playerProps.SpecialDna;
		DNAText.text = playerProps.Dna.ToString();
		RNAText.text = playerProps.SpecialDna.ToString();
		Timing.RunCoroutine(UpdateText(), Segment.SlowUpdate);
	}

	public void SetNotificationText(string newText)
	{
		NotifyText.text = newText;
	}

	IEnumerator<float> UpdateText()
	{
		while (true)
		{
			DNAText.text = playerProps.Dna.ToString();
			RNAText.text = playerProps.SpecialDna.ToString();
			yield return 0;
		}
	}

}
