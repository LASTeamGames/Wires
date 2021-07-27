using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public struct HighscoreEntry : System.IComparable<HighscoreEntry>
{
    public string name;
    public int score;

    
    public int CompareTo(HighscoreEntry other)
    {
        //sort from highest to lowest, so inverse the comparison.
        return other.score.CompareTo(score);
    }
}
public class GameData
{
    
    static protected GameData m_Instance;
    static public GameData instance { get { return m_Instance; } }

    protected string saveFile = "";
    public List<HighscoreEntry> highscores = new List<HighscoreEntry>();

    static int s_Version = 1;

    public int GetScorePlace(int score)
    {
        HighscoreEntry entry = new HighscoreEntry();
        entry.score = score;
        entry.name = "";

        int index = highscores.BinarySearch(entry);

        return index < 0 ? (~index) : index;
    }

    public void InsertScore(int score, string name)
    {
        HighscoreEntry entry = new HighscoreEntry();
        entry.score = score;
        entry.name = name;

        highscores.Insert(GetScorePlace(score), entry);

        // Keep only the 10 best scores.
        while (highscores.Count > 10)
            highscores.RemoveAt(highscores.Count - 1);
    }

    // File management

    static public void Create()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameData();
        }

        m_Instance.saveFile = Application.persistentDataPath + "/save.bin";

        if (File.Exists(m_Instance.saveFile))
        {
            // If we have a save, we read it.
            m_Instance.Read();
            Debug.Log("read scores");
        }
        else
        {
            // If not we create one with default data.
            NewSave();
        }

    }

    static public void NewSave()
    {
        m_Instance.highscores.Clear();
        m_Instance.Save();
    }

    public void Read()
    {
        BinaryReader r = new BinaryReader(new FileStream(saveFile, FileMode.Open));

        int ver = r.ReadInt32();

        // Added highscores.
        if (ver ==1)
        {
            highscores.Clear();
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                HighscoreEntry entry = new HighscoreEntry();
                entry.name = r.ReadString();
                entry.score = r.ReadInt32();

                highscores.Add(entry);
            }
        }

        
        r.Close();
    }

    public void Save()
    {
        BinaryWriter w = new BinaryWriter(new FileStream(saveFile, FileMode.OpenOrCreate));

        w.Write(s_Version);
        
        w.Write(highscores.Count);
        for (int i = 0; i < highscores.Count; ++i)
        {
            w.Write(highscores[i].name);
            w.Write(highscores[i].score);
        }

        w.Close();
    }


}
