using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.Mathematics;
using System.Collections.Generic;

public enum LevelState
{
    Loading,
    DayOpenning,
    Intro,
    Playing,
    Closing,
    Result,
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;    

    [Header ("Customer Key Locations")]
    [SerializeField] public Transform customerExitPos;

    [Header ("Current Level Data")]
    public LevelDataSO[] levelDataSOs;
    public LevelDataSO currentLevelData; 

    [Header("Settings")]
    public float introTime = 2f; 
    public float realSecondsPerHour = 2f; 

    public float baseGoalPrice = 100f;
    
    // UI Animasyonu (0.5s) + Ufak bir pay (0.1s)
    private const float UI_TRANSITION_BUFFER = 0.6f; 

    public float targetDailyIncome;

    public bool isDayStarted { get; private set; } = false;
    
    public LevelState currentLevelState { get; private set; }

    private CancellationTokenSource dayClockCTS;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

            // 1. VSync'i Kapat (Çok Önemli!)
        // VSync açıkken (1 veya 2), Unity hedef FPS'i görmezden gelir ve monitöre kilitler.
        QualitySettings.vSyncCount = 0;

        // 2. Hedef FPS'i Belirle
        // 60, 120 veya limitsiz (-1) yapabilirsin.
        Application.targetFrameRate = 120;
    }

  

    // --- LOADING ---

    public async UniTask PrepareLevelRoutine(UnityEngine.UI.Slider loadingBar)
    {
        Debug.Log("Sahne geldi, Hazırlık Başlıyor...");
        CustomerPool.Instance.InitializePool();

        while (!CustomerPool.Instance.IsReady)
        {
            if(loadingBar.value < 0.9f) loadingBar.value += Time.deltaTime * 0.5f;
            await UniTask.Yield(); 
        }

        loadingBar.value = 1f;
        StartLevel(levelDataSOs[GameDataManager.Instance.GetCurrentLevelIndex()]); 
    }

    public void StartLevel(LevelDataSO levelData)
    {
        currentLevelData = levelData;

        if(ShopLevelManager.Instance != null)
        {
            ShopLevelManager.Instance.SetLevelConfig(levelData);
        }

        DayDataSO selectedDay = GetRandomDay(levelData.dayData);


        ClockManager.Instance.Initialize(selectedDay.dayStartTime, selectedDay.dayEndTime, realSecondsPerHour);
        SetDayDifficulty(selectedDay);
        // ÖNEMLİ: Yeni gün başlarken bayrağı sıfırla!
        isDayStarted = false;

        Debug.Log("🚀 YÜKLEME BİTTİ -> HAZIRLIK MODU");
        ChangeState(LevelState.DayOpenning); 
    }

    private DayDataSO GetRandomDay(List<DayEntry> dayEntries)
    {
        if (dayEntries == null || dayEntries.Count == 0) return null;

        // ---------------------------------------------------------
        // ADIM 1: TOPLAM AĞIRLIĞI HESAPLA (Pastanın Büyüklüğü)
        // ---------------------------------------------------------
        // Torbada toplam kaç bilet olacağını hesaplıyoruz.
        int totalWeight = 0;
        foreach (var entry in dayEntries)
        {
            totalWeight += entry.weight; // Örn: 70 + 30 = 100
        }

        // ---------------------------------------------------------
        // ADIM 2: ZARI AT (Rastgele Bir Sayı Seç)
        // ---------------------------------------------------------
        // 0 ile Toplam Ağırlık arasında bir sayı tutuyoruz.
        // Örn: 0 ile 100 arasında bir sayı. Diyelim ki "85" geldi.
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        // ---------------------------------------------------------
        // ADIM 3: KAZANANI BUL (Kümülatif Kontrol)
        // ---------------------------------------------------------
        int cumulativeWeight = 0; // Şu ana kadar baktığımız bilet sayısı

        foreach (var entry in dayEntries)
        {
            // Mevcut günün ağırlığını, birikmiş ağırlığa ekle.
            // Bu bize o günün "menzilini" (range) verir.
            cumulativeWeight += entry.weight;

            // EĞER tuttuğumuz sayı (randomValue), bu menzilin içindeyse;
            // KAZANAN BU GÜNDÜR!
            if (randomValue < cumulativeWeight)
            {
                return entry.dayData;
            }
        }

        // ---------------------------------------------------------
        // ADIM 4: GÜVENLİK AĞI (Fallback)
        // ---------------------------------------------------------
        // Matematiksel olarak kodun buraya hiç düşmemesi gerekir.
        // Ama bilgisayarlarda bazen kayar nokta hataları vb. olabilir.
        // Oyun çökmesin diye "Garanti olsun, ilkini döndür" diyoruz.
        return dayEntries[0].dayData; 
    }

    public void SetDayDifficulty(DayDataSO dayData)
    {
        float goalMultiplier = dayData.goalMultiplier;
        float spawnRateMultiplier = dayData.spawnRateMultiplier;

        float baseGoal = baseGoalPrice * Mathf.Pow(1.15f, GameDataManager.Instance.currentDayIndex );
        float baseSpawnRate = 5f;

        float finalGoal = baseGoal * goalMultiplier;
        targetDailyIncome = finalGoal;

        float finalSpawnRate = baseSpawnRate / spawnRateMultiplier;
        int totalCustomer =   UnityEngine.Random.Range(currentLevelData.maxTableCap * 2, currentLevelData.maxCustomerCap+ 1);

        CustomerSpawner.Instance.InitializeLevel(totalCustomer  );

        Debug.Log($"Gün Ayarlandı: Hedef Gelir: {finalGoal} , Spawn Süresi: {finalSpawnRate} sn , Günün ismi : {dayData.dayName}  , Toplam Müşteri: {totalCustomer}");

        // özel ayarları varsa burada ekleyebiliriz (Kahve günü olkması gibi özel günler)
    }

 

    public void ChangeState(LevelState newState)
    {
        currentLevelState = newState;

        switch (newState)
        {
            case LevelState.DayOpenning:
                Debug.Log("Level State: DayOpenning");
                Time.timeScale = 1f;
                
                DayOpenningSequence(this.GetCancellationTokenOnDestroy()).Forget();
                break;

            case LevelState.Intro:
                Debug.Log("Level State: INTRO");
                // IntroSequence token ile başlar
                IntroSequence(this.GetCancellationTokenOnDestroy()).Forget();
                break;

            case LevelState.Playing:
                Debug.Log("Level State: PLAYING");
                //StopClock();
                ClockManager.Instance.StopClock();                
                ClockManager.Instance.StartClock();
                CustomerSpawner.Instance.StartSpawning();
                break;

            case LevelState.Closing:
                Debug.Log("Level State: CLOSING");
                ClockManager.Instance.StopClock();
                CustomerSpawner.Instance.StopSpawning();
                ClosingSequence(this.GetCancellationTokenOnDestroy()).Forget();
                break;

            case LevelState.Result:
                Debug.Log("Level State: RESULT");
                EconomyManager.Instance.EndOfDay(this);

                break;
        }
    }


    private async UniTaskVoid DayOpenningSequence(CancellationToken token)
    {
        Debug.Log("🔒 Dükkan Kapalı. Hazırlık bekleniyor...");

        // 1. Sahne açılınca hemen UI patlamasın diye çok kısa bekle
        bool isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token).SuppressCancellationThrow();
        if (isCancelled) return;

        // 2. Hazırlık Ekranını Aç
        EventBus.Publish(new UIEvent.ShopOpeningEvent(  currentLevelData , this));

        // 3. Oyuncunun "Start" butonuna basmasını bekle (WaitUntil)
        // SuppressCancellationThrow kullanarak try-catch bloğundan kurtulduk, daha temiz oldu.
        isCancelled = await UniTask.WaitUntil(() => isDayStarted == true, cancellationToken: token).SuppressCancellationThrow();
        if (isCancelled) return;

        Debug.Log(" Tabela Çevrildi! Dükkan Açılıyor...");

        // 4. UI Kapanış Animasyonunu Bekle
        // UI Manager'da animasyon 0.5sn sürüyor, biz 0.6sn bekleyerek garantiye alıyoruz.
        EventBus.Publish(new UIEvent.DayStartingEvent(currentLevelData , this));
        
        isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(UI_TRANSITION_BUFFER), cancellationToken: token).SuppressCancellationThrow();
        if (isCancelled) return;
        
        ChangeState(LevelState.Intro);
    }

    private async UniTaskVoid IntroSequence(CancellationToken token)
    {
        bool isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: token).SuppressCancellationThrow();
        if(isCancelled) return;

        EventBus.Publish(new UIEvent.LevelIntroEvent(currentLevelData, introTime , this));

        // Intro süresi kadar bekle
        isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(introTime), cancellationToken: token).SuppressCancellationThrow();
        if(isCancelled) return;

        ChangeState(LevelState.Playing);
    }

    private async UniTaskVoid ClosingSequence(CancellationToken token)
    {
        // Kapanış süresi
        bool isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: token).SuppressCancellationThrow();
        if(isCancelled) return;

        ChangeState(LevelState.Result);
    }


    // --- HELPERS ---

  
    public void StartDay()
    {
        isDayStarted = true;
    }

    public LevelDataSO GetCurrentLevel()
    {
        int levelIndex = GameDataManager.Instance.GetCurrentLevelIndex();
        LevelDataSO _currentLevelData = levelDataSOs[levelIndex];
        // currentLevelData = _currentLevelData;
        return _currentLevelData;
    }

    public void UpdateLevelData()
    {
        int levelIndex = GameDataManager.Instance.GetCurrentLevelIndex();
        currentLevelData = levelDataSOs[levelIndex];
    }
}