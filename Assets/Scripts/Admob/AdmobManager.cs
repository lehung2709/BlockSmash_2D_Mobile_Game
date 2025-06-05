using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance { get; private set; }

#if UNITY_ANDROID
    private string _bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IPHONE
    private string _bannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
    private string _rewardedAdUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
    private string _bannerAdUnitId = "unused";
    private string _rewardedAdUnitId = "unused";
#endif

    private BannerView _bannerView;
    private RewardedInterstitialAd _rewardedInterstitialAd;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            CreateBannerView();
            LoadBannerAd();
            LoadRewardedInterstitialAd();
        });
    }

    #region Banner Ad

    public void CreateBannerView()
    {
        if (_bannerView != null)
        {
            DestroyBannerAd();
        }

        _bannerView = new BannerView(_bannerAdUnitId, AdSize.Banner, AdPosition.Top);
        ListenToBannerAdEvents();
    }

    public void LoadBannerAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var request = new AdRequest();
        _bannerView.LoadAd(request);
    }

    private void ListenToBannerAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () => Debug.Log("Banner ad loaded.");
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) => Debug.LogError("Banner ad failed to load: " + error);
        _bannerView.OnAdPaid += (AdValue adValue) => Debug.Log($"Banner ad paid {adValue.Value} {adValue.CurrencyCode}");
        _bannerView.OnAdClicked += () => Debug.Log("Banner ad clicked.");
        _bannerView.OnAdImpressionRecorded += () => Debug.Log("Banner ad impression recorded.");
        _bannerView.OnAdFullScreenContentOpened += () => Debug.Log("Banner ad fullscreen opened.");
        _bannerView.OnAdFullScreenContentClosed += () => Debug.Log("Banner ad fullscreen closed.");
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
            Debug.Log("Banner ad destroyed.");
        }
    }

    #endregion

    #region Rewarded Ad

    public RewardedInterstitialAd GetRewardedInterstitialAd()
    {
        return _rewardedInterstitialAd;
    }    
    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedInterstitialAd != null)
        {
            _rewardedInterstitialAd.Destroy();
            _rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        RewardedInterstitialAd.Load(_rewardedAdUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedInterstitialAd = ad;
                RegisterEventHandlers(_rewardedInterstitialAd);
                RegisterReloadHandler(_rewardedInterstitialAd);
            });
    }


    public void ShowRewardedInterstitialAd()
    {
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
        {
            _rewardedInterstitialAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open " +
                           "full screen content with error : " + error);
        };
    }

    private void RegisterReloadHandler(RewardedInterstitialAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += ()=>
         {
            Debug.Log("Rewarded interstitial ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open " +
                           "full screen content with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedInterstitialAd();
        };
    }

    #endregion
}
