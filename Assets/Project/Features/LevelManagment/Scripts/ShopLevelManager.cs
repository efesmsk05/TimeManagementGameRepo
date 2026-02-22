using UnityEngine;

public class ShopLevelManager : MonoBehaviour
{
    public static ShopLevelManager Instance;
    
    // Şu anki levelin hedeflerini bilmemiz için data referansı
    private LevelDataSO currentLevelConfig;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. Oyun başladığında mevcut level datasını LevelManager'dan çek
        // DİKKAT: LevelManager Start'ta bu veriyi set etmiş olmalı.
        // Eğer LevelManager henüz hazır değilse hata alabiliriz. 
        // En garantisi LevelManager'ın StartLevel fonksiyonunda ShopLevelManager'a data göndermesidir.
        // Ama şimdilik basitçe buradan çekmeyi deneyelim:
        
        if (LevelManager.Instance != null && LevelManager.Instance.currentLevelData != null)
        {
            currentLevelConfig = LevelManager.Instance.GetCurrentLevel();
            UpdateUI(); // Başlangıçta barı doğru konuma getir
        }
    }

    // LevelManager her level yüklediğinde bu fonksiyonu çağırıp datayı güncelleyebilir
    public void SetLevelConfig(LevelDataSO levelData)
    {
        currentLevelConfig = levelData;
        UpdateUI();
    }

    void OnEnable()
    {
        EventBus.Subscribe<DataEvents.OnReputationGainedEvent>(OnReputationGained);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<DataEvents.OnReputationGainedEvent>(OnReputationGained);
    }

    private void OnReputationGained(DataEvents.OnReputationGainedEvent evt)
    {

        if (currentLevelConfig == null)
        {
            Debug.LogWarning("⚠️ Mevcut level datası ayarlı değil!");
            return;
        }
          

        // 1. XP Ekle (GameDataManager kalıcı hafızadır)
        Debug.Log($"⭐ XP Kazanıldı: {evt.amount}");
        GameDataManager.Instance.AddXp(evt.amount);
        
        // 2. UI Güncelle
        UpdateUI();

        // 3. Level Atlamaya Hazır mı Kontrol Et
        if (CheckIfReadyToLevelUp())
        {
            Debug.Log("🎉 LEVEL UP HAZIR! Butonu yakabilirsin.");
            // İstersen burada UI'da "Upgrade" butonunu aktif eden bir event fırlatabilirsin.
            // EventBus.Publish(new UIEvent.ShopUpgradeAvailableEvent(true));
        }
    }

    private void UpdateUI()
    {
        if (currentLevelConfig == null) return;

        float currentXp = GameDataManager.Instance.currentXp;
        float requiredXp = currentLevelConfig.requiredReputation;

        // 0 ile 1 arasında sınırla (Bar taşmasın)
        float fillAmount = Mathf.Clamp01(currentXp / requiredXp);
        
        EventBus.Publish(new UIEvent.UpdateLevelBarEvent(fillAmount));
    }

    // --- LEVEL ATLAMA SİSTEMİ (Manuel Tetiklenecek) ---

    public bool CheckIfReadyToLevelUp()
    {
        if (currentLevelConfig == null) return false;

        float currentXp = GameDataManager.Instance.currentXp;
        float currentMoney = GameDataManager.Instance.totalMoney;
        
        return (currentXp >= currentLevelConfig.requiredReputation && 
                currentMoney >= currentLevelConfig.costToUpgrade);
    }

    public void TryLevelUp()
    {
        if (CheckIfReadyToLevelUp())
        {
            // 1. Bedeli Öde
            GameDataManager.Instance.TrySpendMoney(currentLevelConfig.costToUpgrade);

            // 2. XP'yi Sıfırla (Veya artan kısmı sonraki levele devret)
            // Devretmek daha oyuncu dostudur:
            float overflowXp = GameDataManager.Instance.currentXp - currentLevelConfig.requiredReputation;
            GameDataManager.Instance.currentXp = overflowXp; 
            // Eğer sıfırlamak istersen: GameDataManager.Instance.currentXp = 0;


            GameDataManager.Instance.LevelUp();

            EventBus.Publish(new UIEvent.LevelUpEvent(LevelManager.Instance.GetCurrentLevel()));
            LevelManager.Instance.UpdateLevelData();
            // 4. Kaydet
            GameDataManager.Instance.SaveGame();

            // 5. UI Güncelle
            UpdateUI();
        }
        else
        {
            Debug.Log("Yetersiz kaynak!");
        }
    }
}