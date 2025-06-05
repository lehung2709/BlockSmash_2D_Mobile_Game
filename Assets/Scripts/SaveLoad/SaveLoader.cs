using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;



[System.Serializable]
public class GridCellSaveData
{
    
    public bool isOccupied;
    public int  colorIndex;

    public GridCellSaveData(bool isOccupied, int colorIndex)
    {
        this.isOccupied = isOccupied;
        this.colorIndex = colorIndex;
    }
}
[System.Serializable]
public class RowSaveData
{
    public GridCellSaveData[] cells;

    public RowSaveData(GridCellSaveData[] cells)
    {
        this.cells = cells;
    }
}

[System.Serializable]
public class ShapeSaveData
{
    public bool isActive=false;
    public bool isDragable;
    public int shapeIndex;
    public int colorIndex;

    public ShapeSaveData(int shapeIndex, int colorIndex, bool isDragable)
    {
        isActive = true;
        this.shapeIndex = shapeIndex;
        this.colorIndex = colorIndex;
        this.isDragable = isDragable;   
    }
    public ShapeSaveData()
    {
        isActive = false;
       
    }
}
[System.Serializable]
public class GameSessionData
{
    public int score;
    public RowSaveData[] gridData; 
    public ShapeSaveData[] shapeDatas;

    public GameSessionData(int score, RowSaveData[] gridData, ShapeSaveData[] shapeDatas)
    {
        this.score = score;
        this.gridData = gridData;
        this.shapeDatas = shapeDatas;
    }
}

public class SaveLoader 
{
    private const string HighestScoreKey = "HighestScore";
    private const string SessionFileName = "game_session.json";

    public void SaveHighestScore(int score)
    {
        if (score > LoadHighestScore())
        {
            PlayerPrefs.SetInt(HighestScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("New highest score saved: " + score);
        }
    }
    public int LoadHighestScore()
    {
        int heighestScore = PlayerPrefs.GetInt(HighestScoreKey, 0);
        return heighestScore;
    }
    public void SaveSession(GameSessionData sessionData)
    {
        string json = JsonUtility.ToJson(sessionData, prettyPrint: true);

        string path = Path.Combine(Application.persistentDataPath, SessionFileName);
        File.WriteAllText(path, json);

        Debug.Log("Session saved to " + path);
    }

    public GameSessionData LoadSession()
    {
        //EndSession();
        string path = Path.Combine(Application.persistentDataPath, SessionFileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSessionData sessionData = JsonUtility.FromJson<GameSessionData>(json);
            Debug.Log("Session loaded from " + path);
            return sessionData;
        }
        else
        {
            Debug.LogWarning("Save file not found at " + path);
            return null;
        }
    }
    public void EndSession()
    {
        string path = Path.Combine(Application.persistentDataPath, "game_session.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Session file deleted.");
        }
        else
        {
            Debug.LogWarning("No session file found to delete.");
        }
    }
    

}
