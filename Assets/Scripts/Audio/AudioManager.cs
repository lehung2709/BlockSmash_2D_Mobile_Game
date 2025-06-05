using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Reference")]

    [SerializeField] private SoundLibSO soundLibSO;
    [SerializeField] private Queue<AudioEmitter> audioEmitterPool;

    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup SFXGroup;

    [Header("Value")]

    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxEmitters = 20;
    [SerializeField] private string musicName;

    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";
    public bool IsMusicOn { get; private set; } = true;
    public bool IsSFXOn { get; private set; } = true;

    



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            InitializePool();
            LoadAudioSettings();
            PlayMusic(musicName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicGroup.audioMixer.SetFloat(MUSIC_VOLUME, IsMusicOn ? 0f : -80f);
        SFXGroup.audioMixer.SetFloat(SFX_VOLUME, IsSFXOn ? 0f : -80f);
    }

    private void LoadAudioSettings()
    {
        IsMusicOn = PlayerPrefs.GetInt(MUSIC_VOLUME, 1) == 1;
        IsSFXOn = PlayerPrefs.GetInt(SFX_VOLUME, 1) == 1;
    }

    public bool ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;
        musicGroup.audioMixer.SetFloat(MUSIC_VOLUME, IsMusicOn ? 0f : -80f);
        PlayerPrefs.SetInt(MUSIC_VOLUME, IsMusicOn ? 1 : 0);
        PlayerPrefs.Save();
        return IsMusicOn;
    }

    public bool ToggleSFX()
    {
        IsSFXOn = !IsSFXOn;
        SFXGroup.audioMixer.SetFloat(SFX_VOLUME, IsSFXOn ? 0f : -80f);
        PlayerPrefs.SetInt(SFX_VOLUME, IsSFXOn ? 1 : 0);
        PlayerPrefs.Save();
        return IsSFXOn;
    }
    private void InitializePool()
    {
        audioEmitterPool = new Queue<AudioEmitter>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateSoundEmitter();
        }
    }

    private AudioEmitter CreateSoundEmitter()
    {
        GameObject soundEmitterObject = new GameObject("SoundEmitter");
        AudioEmitter emitter = soundEmitterObject.AddComponent<AudioEmitter>();
        soundEmitterObject.SetActive(false);
        audioEmitterPool.Enqueue(emitter);
        return emitter;
    }

    public AudioEmitter SpawnSoundEmitter(Transform parent, string soundName, Vector3 pos)
    {
        AudioEmitter audioEmitter;

        if (audioEmitterPool.Count > 0)
        {
            audioEmitter = audioEmitterPool.Dequeue();
        }
        else if (audioEmitterPool.Count < maxEmitters)
        {
            audioEmitter = CreateSoundEmitter();
        }
        else
        {
            audioEmitter = audioEmitterPool.Dequeue();
        }

        audioEmitter.transform.SetParent(parent);
        audioEmitter.transform.position = pos;
        audioEmitter.gameObject.SetActive(true);

        audioEmitter.PlaySound(soundLibSO.GetSound(soundName));
        return audioEmitter;
    }

    public void ReturnToPool(AudioEmitter emitter)
    {
        audioEmitterPool.Enqueue(emitter);
    }


    public void PlayBtnSound()
    {
        SpawnSoundEmitter(transform, "Btn", Vector3.zero);
    }

    public void PlayMusic(string musicName)
    {
        this.musicName = musicName;
        SpawnSoundEmitter(null, musicName, Vector3.zero);

    }
}
