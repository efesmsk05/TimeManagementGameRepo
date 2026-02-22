using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Game Data")]
    public float totalMoney = 5000f;
    public int currentDayIndex = 1; 

    [Header("Shop Level Data")]
    public float currentXp;    

    public int currentLevelIndex = 0;

    [Header("Config")]
    public List<UpgradeDataSO> allUpgrades; // ScriptableObject Referansları

    // Dosya yolu yerine artık bir "Anahtar Kelime" kullanıyoruz. web ve mobile için uygun.
    private const string SAVE_KEY = "Save_v1";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }


    }

    // --- SAVE & LOAD SİSTEMİ (PLAYER PREFS) ---

    public void SaveGame()
    {
        SaveData data = new SaveData();
        
        // 2. Oyundaki mevcut verileri koliye doldur
        data.playerData.totalMoney = totalMoney;
        data.playerData.currentDayIndex = currentDayIndex;
        data.shopStats.currentXp = currentXp;
        data.shopStats.currentLevelIndex = currentLevelIndex;

        // 3. Upgrade verilerini ScriptableObject'lerden çekip listeye ekle
        foreach (var upg in allUpgrades)
        {
            UpgradeSaveData uData = new UpgradeSaveData();
            uData.type = upg.upgradeType;
            uData.level = upg.currentLevel; // O anki seviyeyi al
            data.upgrades.Add(uData);
        }

        // 4. Koliyi kapat ve JSON formatına (Metin) çevir
        string json = JsonUtility.ToJson(data, true);

        // 5. Tarayıcının/Telefonun hafızasına (PlayerPrefs) yaz
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save(); // Diske yazmayı garantile

        Debug.Log("💾 Oyun Kaydedildi (Web/Mobil Uyumlu): " + json);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            // 2. Varsa o metni (JSON) getir
            string json = PlayerPrefs.GetString(SAVE_KEY);

            // 3. Metni tekrar SaveData nesnesine çevir (Koliyi aç)
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 4. Verileri oyuna geri yükle
            totalMoney = data.playerData.totalMoney;
            currentDayIndex = data.playerData.currentDayIndex;
            currentXp = data.shopStats.currentXp;
            currentLevelIndex = data.shopStats.currentLevelIndex;

            // 5. Upgrade seviyelerini ScriptableObject'lere işle
            foreach (var savedUpg in data.upgrades)
            {
                // Listeden doğru ScriptableObject'i bul
                UpgradeDataSO match = allUpgrades.Find(x => x.upgradeType == savedUpg.type);
                
                if (match != null)
                {
                    // Runtime değerini güncelle
                    match.currentLevel = savedUpg.level; 
                }
            }
            Debug.Log("📂 Veriler Başarıyla Yüklendi!");
        }
        else
        {
            Debug.Log("⚠️ Kayıt bulunamadı, Yeni Oyun başlatılıyor.");
            ResetAllUpgrades(); // İlk kez açılıyorsa her şeyi sıfırla
        }
    }

    // RESET

    public void DeleteSaveAndRestart()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            Debug.Log("🗑️ Kayıt Hafızadan Silindi!");
        }

        totalMoney = 5000f;
        currentDayIndex = 1;
        currentXp = 0f;
        currentLevelIndex = 0;
        ResetAllUpgrades(); 

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadNewLevel(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Yedek olarak standart Unity yükleyicisi
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        Debug.Log("🔄 Oyun Sıfırlandı ve Yeniden Başlatıldı.");
    }

    // --- YARDIMCI FONKSİYONLAR ---

    private void ResetAllUpgrades()
    {
        foreach(var item in allUpgrades)
        {
            item.currentLevel = 1;
        }
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }

    // --- UPGRADE MANTIĞI ---

    public bool TryBuyUpgrade(UpgradeDataSO upgrade)
    {
        if (!upgrade.CanLevelUp()) return false;

        float cost = upgrade.GetCurrentCost();
        if (totalMoney >= cost)
        {
            totalMoney -= cost;
            upgrade.LevelUp();
            
            // Satın alma işlemi kritiktir, hemen kaydet!
            SaveGame(); 

            EventBus.Publish(new UIEvent.UpdateMoneyTextEvent(totalMoney));
            EventBus.Publish(new DataEvents.OnUpgradeSuccessEvent(upgrade.upgradeType));
            return true;
        }
        return false;
    }

    public float GetUpgradeValues(UpgradeType upgradeType)
    {
        foreach (UpgradeDataSO data in allUpgrades)
        {
            if (data.upgradeType == upgradeType)
            {
                return data.GetCurrentValue();
            }
        }
        return 0f;
    }


    // Economy data management

    public void AddMoney(float amount)
    {
        totalMoney += amount;
        EventBus.Publish(new UIEvent.UpdateMoneyTextEvent(totalMoney));
    }

    public bool TrySpendMoney(float amount)
    {
        if (totalMoney >= amount)
        {
            totalMoney -= amount;
            EventBus.Publish(new UIEvent.UpdateMoneyTextEvent(totalMoney));
            return true;
        }
        else
        {
            return false;
        }

    }

    // Level progression

    public void NextDay()
    {
        currentDayIndex++;
        SceneLoader.Instance.LoadNewLevel(SceneManager.GetActiveScene().name);
        SaveGame();
    }

    public void AddXp(float amount)
    {
        currentXp += amount;
        SaveGame();
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    public void LevelUp()
    {
        currentLevelIndex++;
        currentXp = 0f; // XP sıfırlanır
        SaveGame();
    }

}