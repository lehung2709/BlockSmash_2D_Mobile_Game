using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayBtn : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    private void Start()
    {
        playBtn.onClick.AddListener(Play);
    }
    private void Play()
    {
        AudioManager.Instance.PlayBtnSound();
        AsyncLoader.Instance.LoadScene("GameScene");
    }    

}
