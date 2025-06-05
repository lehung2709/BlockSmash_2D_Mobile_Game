using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreCounter : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI highestScoreTMP;
    [SerializeField] private PopUp popUp;

    [Header("Value")]

    private int currentScore = 0;
    private int highestScore = 0;
    private int displayScore = 0;
    private int combo = 0;

    private bool isUpdatingText=false;

   

    public int GetScore()
    {
        return currentScore;
    }    
    public void SetScore(int HeighestScore,int Score=0)
    {
        this.highestScore = HeighestScore;

        if(Score > highestScore ) highestScore = Score;

        this.currentScore = Score;
        scoreTMP.text = currentScore.ToString();
        highestScoreTMP.text = highestScore.ToString();

    }

    private void AddScore(int amount)
    {
        displayScore=currentScore;
        currentScore += amount;
        if (currentScore > highestScore)
        {
            highestScore = currentScore;
            UpdateScoreText();
        }
        else
        {
            UpdateScoreText();
        }
    }
    private IEnumerator UpdateScoreTextCoroutine()
    {
        AudioEmitter emitter= AudioManager.Instance.SpawnSoundEmitter(null, "Pop", transform.position);
        isUpdatingText = true;
        while (displayScore < currentScore)
        {
            displayScore++;
            scoreTMP.text = displayScore.ToString();
            if(displayScore > highestScore) highestScoreTMP.text = displayScore.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        emitter.StopAndReturnToPool();
        isUpdatingText=false;
    }    

    

    private void UpdateScoreText()
    {
        if (isUpdatingText) return;

        StartCoroutine(UpdateScoreTextCoroutine());
         
        
    }
    
    public void CalculateScore(int clearedRows,int clearedCols )
    {
        int totalScore = 10;
        int totalCleared = clearedRows + clearedCols;
        if( totalCleared > 0)
        {
            combo++;
            totalScore += totalCleared * 10 + (totalCleared - 1) * 5+5*(combo-1);
            popUp.Show(totalScore, clearedRows, clearedCols, combo);

        }
        else
        {
            combo = 0;
            popUp.Show(totalScore);

        }
        AddScore(totalScore);






    }
}
