using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{

   
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button SFXBtn;

    [SerializeField] private Image musicBtnIcon;
    [SerializeField] private Image SFXBtnIcon;


    [SerializeField] private Sprite[] musicBtnSprites;
    [SerializeField] private Sprite[] SFXBtnSprites;

    

    void Start()
    {
        
        musicBtn.onClick.AddListener(ToggleMusic);
        SFXBtn.onClick.AddListener(ToggleSFX);
        musicBtnIcon.sprite = AudioManager.Instance.IsMusicOn ? musicBtnSprites[0] : musicBtnSprites[1];
        SFXBtnIcon.sprite = AudioManager.Instance.IsSFXOn ? SFXBtnSprites[0] : SFXBtnSprites[1];


    }

    private void ToggleMusic()
    {
        AudioManager.Instance.PlayBtnSound();
        bool isMusicOn = AudioManager.Instance.ToggleMusic();
        musicBtnIcon.sprite = isMusicOn ? musicBtnSprites[0] : musicBtnSprites[1];
    }

    private void ToggleSFX()
    {
        AudioManager.Instance.PlayBtnSound();
        bool isSFXOn = AudioManager.Instance.ToggleSFX();
        SFXBtnIcon.sprite = isSFXOn ? SFXBtnSprites[0] : SFXBtnSprites[1];
    }
}




