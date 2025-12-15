using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager Instance;

    #region Ad Unit IDs

    [Header("Banner Ad Unit ID")]
    public string bannerAdUnitId;

    [Header("Interstitial Ad Unit ID")]
    public string interstitialAdUnitId;

    [Header("Rewarded Ad Unit ID")]
    public string rewardedAdUnitId;

    [Header("Ad Display Interval")]
    public int adsDisplayInterval = 3;
    #endregion

    #region Ad Objects

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK Initialized");

            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
        });
    }

    #endregion

    #region Banner Ads

    public void LoadBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        int deviceWidth = MobileAds.Utils.GetDeviceSafeWidth();
        AdSize adaptiveSize =
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(deviceWidth);

        bannerView = new BannerView(bannerAdUnitId, adaptiveSize, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () =>
            Debug.Log("Banner Loaded");

        bannerView.OnBannerAdLoadFailed += error =>
            Debug.LogError("Banner Load Failed: " + error.GetMessage());

        bannerView.LoadAd(new AdRequest());
    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    public void DestroyBanner()
    {
        bannerView?.Destroy();
        bannerView = null;
    }

    #endregion

    #region Interstitial Ads

    public void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        InterstitialAd.Load(interstitialAdUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial Load Failed: " + error.GetMessage());
                    return;
                }

                interstitialAd = ad;
                Debug.Log("Interstitial Loaded");

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Interstitial Opened");
                    HideBanner();
                };

                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Interstitial Closed");
                    LoadInterstitial();
                    ShowBanner();
                };

                ad.OnAdFullScreenContentFailed += err =>
                    Debug.LogError("Interstitial Failed To Open: " + err.GetMessage());
            });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
            interstitialAd.Show();
        else
            Debug.Log("Interstitial Not Ready");
    }

    #endregion

    #region Rewarded Ads

    public void LoadRewarded()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        RewardedAd.Load(rewardedAdUnitId, new AdRequest(),
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Rewarded Load Failed: " + error.GetMessage());
                    return;
                }

                rewardedAd = ad;
                Debug.Log("Rewarded Loaded");

                ad.OnAdFullScreenContentOpened += () =>
                {
                    Debug.Log("Rewarded Opened");
                    HideBanner();
                };

                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded Closed");
                    LoadRewarded();
                    ShowBanner();
                };

                ad.OnAdFullScreenContentFailed += err =>
                    Debug.LogError("Rewarded Failed To Open: " + err.GetMessage());
            });
    }

    public void ShowRewarded(Action onReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(reward =>
            {
                Debug.Log("Reward Earned");
                onReward?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded Not Ready");
        }
    }

    public void ShowRewardedAd()
    {
        ShowRewarded(() =>
        {
            Debug.Log("Rewarded Ad Callback – give user reward here");
        });
    }

    #endregion
}