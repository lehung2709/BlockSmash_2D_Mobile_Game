using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SumaryUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private TextMeshProUGUI heighestScoreTMP;
    [SerializeField] private Button ReplayBtn;

    private void Start()
    {
        ReplayBtn.onClick.AddListener(OnReplayBtn);
    }
    private void OnReplayBtn()
    {
        AudioManager.Instance.PlayBtnSound();
        container.SetActive(false);
        GamePlayAdministrator.Instance.Restart();
    }    
    public void Sumary(int score,int heighestScore)
    {
        container.SetActive(true);
        scoreTMP.text=score.ToString();
        if (score > heighestScore)
        {
            heighestScore = score;
        } 
        heighestScoreTMP.text=heighestScore.ToString();
            
    }    
}
