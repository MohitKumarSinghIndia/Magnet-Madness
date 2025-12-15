using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager Instance;

    [Header("BANNER AD UNIT ID")]
    public string bannerAdUnitId;

    [Header("INTERSTITIAL AD UNIT ID")]
    public string interstitialAdUnitId;

    [Header("REWARDED AD UNIT ID")]
    public string rewardedAdUnitId;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    IEnumerator Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Google Mobile Ads SDK Initialized");

            LoadBanner();
            LoadInterstitial();
            LoadRewarded();
        });
        yield return new WaitForSeconds(2);
        ShowBanner();
    }

    #region ------------------------ BANNER -------------------------------

    public void LoadBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        int deviceWidth = MobileAds.Utils.GetDeviceSafeWidth();

        // Define the anchored adaptive ad size.
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(deviceWidth);

        bannerView = new BannerView(bannerAdUnitId, adaptiveSize, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () => Debug.Log("Banner Loaded");
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) => Debug.Log("Banner Loaded Failed");

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    public void ShowBanner() => bannerView?.Show();
    public void HideBanner() => bannerView?.Hide();
    public void DestroyBanner() { bannerView?.Destroy(); bannerView = null; }

    #endregion

    #region ------------------------ INTERSTITIAL -------------------------------

    public void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Interstitial Load Failed: " + error.GetMessage());
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial Loaded");

            ad.OnAdFullScreenContentOpened += () =>
                Debug.Log("Interstitial Opened");

            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial Closed");
                LoadInterstitial();  // auto reload
            };

            ad.OnAdFullScreenContentFailed += (AdError err) =>
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

    #region ------------------------ REWARDED -----------------------------------

    public void LoadRewarded()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Rewarded Load Failed: " + error.GetMessage());
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded Loaded");

            ad.OnAdFullScreenContentOpened += () =>
                Debug.Log("Rewarded Opened");

            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded Closed");
                LoadRewarded(); // auto reload
            };

            ad.OnAdFullScreenContentFailed += (AdError err) =>
                Debug.LogError("Rewarded Failed To Open: " + err.GetMessage());
        });
    }

    public void ShowRewarded(System.Action onReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
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

    // ---- WRAPPER FOR BUTTON (no parameters) ----
    public void ShowRewardedAd()
    {
        ShowRewarded(() =>
        {
            Debug.Log("Rewarded Ad Callback Give user reward!");

            // your reward logic here
            // Example:
            // playerCoins += 100;
        });
    }

    #endregion
}
