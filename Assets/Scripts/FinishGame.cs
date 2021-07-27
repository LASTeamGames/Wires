using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishGame : MonoBehaviour
{
	public Text score;
	int scoreInt;
	public Leaderboard leaderboard;
	public InputField playerName;

	public void Open(int score)
	{
		if (GameData.instance.highscores.Count == 10)
		{
			if (score < GameData.instance.highscores[9].score)
			{
				playerName.gameObject.SetActive(false);
			}
			else
			{
				playerName.gameObject.SetActive(true);
			}
		}
		this.score.text = score.ToString();
		gameObject.SetActive(true);
		this.scoreInt = score;
		
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void Ok()
    {
		if (playerName.gameObject.activeSelf)
		{
			
			leaderboard.forcePlayerDisplay = false;
			leaderboard.displayPlayer = true;
			leaderboard.playerEntry.playerName.text = playerName.text;
			leaderboard.playerEntry.score.text = score.text.ToString();
			leaderboard.displayPlayer = false;
			GameData.instance.InsertScore(scoreInt, playerName.text);
			GameData.instance.Save();
		}
		
		
		leaderboard.Open();
		Debug.Log(GameData.instance.highscores.Count);
		Close();
		
    }
}
