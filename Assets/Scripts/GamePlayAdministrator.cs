using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;


public class GamePlayAdministrator : MonoBehaviour
{
    public static GamePlayAdministrator Instance { get; private set; }

    [Header("Reference")]

    [SerializeField] private GridManager gridManager;
    [SerializeField] private ShapesSpawner shapesSpawner;
    [SerializeField] private ScoreCounter scoreCounter;
    public ColorsSetSO ColorsSetSO;

    [SerializeField] LoseUI loseUI;
    [SerializeField] private SumaryUI sumaryUI;

    private SaveLoader saveLoader;

    [Header("Value")]

    private bool isPlay = true;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            saveLoader=new SaveLoader();
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    private void Start()
    {
        LoadGame();
        
    }
   
    
    public void CheckMatching(int row,int col, ShapeData shapeData,int colorIndex)
    {
        gridManager.CheckMatching(row,col,shapeData,colorIndex);   
    }   
    
    public void ClearPreView()
    {
        gridManager.ClearPreView();
    }  
    
    public bool PlaceBlock()
    {
        PlaceResult result = gridManager.PlaceBlock();
       if( result.placedSuccessfully)
        {
            scoreCounter.CalculateScore(result.clearedRows, result.clearedCols);
            return true;
        }
        return false;
        
            

    }
    public void DecreaseShapeCount()
    {
        shapesSpawner.DecreaseShapeCount(gridManager);
    }    

    public void Lose()
    {
        AudioManager.Instance.SpawnSoundEmitter(null, "Losing", transform.position);

        StartCoroutine(LoseCoroutine());
        saveLoader.EndSession();
        isPlay= false;
    }    
    private IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(1f);
        loseUI.ShowLoseUI();
    }   
    public void SaveSession()
    {
        saveLoader.SaveSession(new GameSessionData(scoreCounter.GetScore(), gridManager.GetGridSaveData(), shapesSpawner.GetShapeSaveData()));
    }
    public void LoadGame()
    {
        gridManager.SpawnGrid();
        shapesSpawner.SpawnShape();
        GameSessionData gameSessionData = saveLoader.LoadSession();
        int score = 0;
        if (gameSessionData!=null)
        {
            gridManager.LoadSession(gameSessionData.gridData);
            shapesSpawner.LoadSession(gameSessionData.shapeDatas);
            score = gameSessionData.score;

        }
        scoreCounter.SetScore(saveLoader.LoadHighestScore(),score);



    }
    public void Revive()
    {
        isPlay = true;
        gridManager.ClearTheWholeGrid();
        shapesSpawner.SpawnShape();
    }    

    public void Restart()
    {
        if (isPlay)
        {
            saveLoader.EndSession();
            isPlay = true;
        }
        gridManager.ClearTheWholeGrid();
        shapesSpawner.SpawnShape();
        scoreCounter.SetScore(saveLoader.LoadHighestScore());
    }

    public void Sumary()
    {

        int highestScore= saveLoader.LoadHighestScore();
        int score = scoreCounter.GetScore();
        saveLoader.SaveHighestScore(score);
        sumaryUI.Sumary(score,highestScore);
    }    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if(isPlay)
               SaveSession();
            else
               saveLoader.SaveHighestScore(scoreCounter.GetScore());

        }
    }

    private void OnApplicationQuit()
    {
        if (isPlay)
            SaveSession();
        else
            saveLoader.SaveHighestScore(scoreCounter.GetScore());
    }

}
