using UnityEngine;
using UnityEngine.UI;

public class HomeRestartBtn : MonoBehaviour
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button restartBtn; 
    void Start()
    {
        homeBtn.onClick.AddListener(OnHomeClicked);
        restartBtn.onClick.AddListener(OnRestartClicked);
    }

    private void OnHomeClicked()
    {
        GamePlayAdministrator.Instance.SaveSession();
        AudioManager.Instance.PlayBtnSound();
        AsyncLoader.Instance.LoadScene("HomeScene");
    }

    private void OnRestartClicked()
    {
        AudioManager.Instance.PlayBtnSound();
        GamePlayAdministrator.Instance.Restart();
    }

}
