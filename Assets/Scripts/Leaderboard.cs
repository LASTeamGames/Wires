using UnityEngine;

public class Leaderboard : MonoBehaviour
{
	public RectTransform entriesRoot;
	public int entriesCount;

	public LBRow playerEntry;
	public bool forcePlayerDisplay;
	public bool displayPlayer = true;

	public void Open()
	{
		gameObject.SetActive(true);

		Populate();
	}

	public void Close()
	{
		gameObject.SetActive(false);
	}

	public void Populate()
	{
		// Start by making all entries enabled & putting player entry last again.
		playerEntry.transform.SetAsLastSibling();
		for (int i = 0; i < entriesCount; ++i)
		{
			entriesRoot.GetChild(i).gameObject.SetActive(true);
		}

		// Find all index in local page space.
		int localStart = 0;
		int place = -1;
		int localPlace = -1;

		if (displayPlayer)
		{
			place = GameData.instance.GetScorePlace(int.Parse(playerEntry.score.text));
			localPlace = place - localStart;
		}

		if (localPlace >= 0 && localPlace < entriesCount && displayPlayer)
		{
			playerEntry.gameObject.SetActive(true);
			playerEntry.transform.SetSiblingIndex(localPlace);
		}

		if (!forcePlayerDisplay || GameData.instance.highscores.Count < entriesCount)
			entriesRoot.GetChild(entriesRoot.transform.childCount - 1).gameObject.SetActive(false);

		int currentHighScore = localStart;

		for (int i = 0; i < entriesCount; ++i)
		{
			LBRow row = entriesRoot.GetChild(i).GetComponent<LBRow>();

			if (row == playerEntry || row == null)
			{
				// We skip the player entry.
				continue;
			}

			if (GameData.instance.highscores.Count > currentHighScore)
			{
				row.gameObject.SetActive(true);
				row.playerName.text = GameData.instance.highscores[currentHighScore].name;
				row.number.text = (localStart + i + 1).ToString();
				row.score.text = GameData.instance.highscores[currentHighScore].score.ToString();

				currentHighScore++;
			}
			else
				row.gameObject.SetActive(false);
		}

		// If we force the player to be displayed, we enable it even if it was disabled from elsewhere
		if (forcePlayerDisplay)
			playerEntry.gameObject.SetActive(true);

		playerEntry.number.text = (place + 1).ToString();
	}
}
