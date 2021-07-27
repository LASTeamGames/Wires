using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    static public GameManager instance { get { return s_Instance; } }
    static protected GameManager s_Instance;

    public Leaderboard leaderboard;
    public FinishGame finishGame;
    public Game game;
    public GameObject menu;

    protected void OnEnable()
    {
        GameData.Create();

        s_Instance = this;
    }

        // Start is called before the first frame update
        void Start()
    {
        Openleaderboard();
        
        CloseMenu();
    }

    public void ClearHighScores()
    {
        GameData.NewSave();
    }
    // Update is called once per frame
    void CloseMenu()
    {
        leaderboard.Close();
        finishGame.Close();
    }

    public void Openleaderboard()
    {
        leaderboard.displayPlayer = false;
        leaderboard.forcePlayerDisplay = false;
        leaderboard.Open();

    }

    public void FinishGame(int score)
    {
        leaderboard.Close();
        finishGame.Open(score);
        game.gameObject.SetActive(false);
        menu.SetActive(true);
    }

    public void StartGame()
    {
        leaderboard.Close();
        menu.SetActive(false);
        game.gameObject.SetActive(true);
        game.Start();
    }
}
