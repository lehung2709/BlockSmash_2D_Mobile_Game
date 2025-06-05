using GoogleMobileAds.Api;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseUI : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] private GameObject Container;
    [SerializeField] private TextMeshProUGUI waitTimeTMP;
    [SerializeField] private Button skipBtn;
    [SerializeField] private Button adsBtn;


    private Coroutine coroutine;

    [Header("Value")]

    [SerializeField] private int waitDuration = 5;

    private bool isSumary=true;

    private void Start()
    {
        skipBtn.onClick.AddListener(Sumary);
        adsBtn.onClick.AddListener(ShowAds2Revive);
    }

    public void ShowLoseUI()
    {
        Container.SetActive(true);
        coroutine= StartCoroutine(CountdownCoroutine(waitDuration));

    }
    private IEnumerator CountdownCoroutine(int duration)
    {
        int timer = duration;

        isSumary = true;
        while (timer > 0)
        {
            waitTimeTMP.text = timer.ToString();
            yield return new WaitForSeconds(1f);
            timer -= 1;
        }
        waitTimeTMP.text = timer.ToString();
        yield return new WaitForSeconds(0.1f);
        if(isSumary)
        Sumary();
    }
    private void Sumary()
    {
        isSumary = false;
        Container.SetActive(false);
        GamePlayAdministrator.Instance.Sumary();
    }   
    
    private void ShowAds2Revive()
    {
        StopCoroutine(coroutine);
        Container.SetActive(false);
        RewardedInterstitialAd ad= AdmobManager.Instance.GetRewardedInterstitialAd();
        ad.OnAdFullScreenContentClosed += GamePlayAdministrator.Instance.Revive;
        AdmobManager.Instance.ShowRewardedInterstitialAd();
    }    
}
