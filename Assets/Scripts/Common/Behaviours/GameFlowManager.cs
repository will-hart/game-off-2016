using UnityEngine;
using System.Collections;

public class GameFlowManager : MonoSingleton<GameFlowManager>
{
	public void EndGame()
	{
		Application.Quit();
	}

	public void PlayerWon()
	{
		GUIStatLabelManager.Instance.SetNotificationText("YOU WON!");
		Invoke("EndGame", 3f);
	}

	public void PlayerLost()
	{
		GUIStatLabelManager.Instance.SetNotificationText("YOU LOST!");
		Invoke("EndGame", 3f);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			GUIStatLabelManager.Instance.SetNotificationText("QUITTING");
			Invoke("EndGame", 3f);
		}
	}
}
