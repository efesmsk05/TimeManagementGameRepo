using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders; // SceneInstance için gerekli
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI")]
    public GameObject loadingScreenPanel;
    
    public Slider loadingBar;

    private AsyncOperationHandle<SceneInstance> currentSceneHandle;

    private CancellationTokenSource cts = new CancellationTokenSource();

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
        // HATA 1 DÜZELTİLDİ: Fonksiyon ismi doğrusuyla değiştirildi.
        LoadNewLevel("MainMenu");
    }

    public void LoadNewLevel(string levelAddress)
    {
        LoadLevelSequence(levelAddress, cts.Token).Forget();
    }



    private async UniTaskVoid LoadLevelSequence(string levelAddress, CancellationToken token)
    {
        // 1. Loading Ekranını Aç
        loadingScreenPanel.SetActive(true);
        loadingBar.value = 0;

        // HATA 2 DÜZELTİLDİ: LoadSceneMode.Single kullanırken manuel Unload yapmıyoruz.
        // Ancak eski handle hafızada yer kaplamasın diye serbest bırakabiliriz (Release).
        // Not: Eğer Bootstrap sahnesindeysek handle zaten boştur (IsValid kontrolü o yüzden var).
        if (currentSceneHandle.IsValid())
        {
            // Sahneyi kapatmıyoruz (Single modu kapatacak), sadece Addressable referansını düşürüyoruz.
            // Bu satır opsiyoneldir ama hafıza yönetimi için iyidir.
            // Addressables.Release(currentSceneHandle); 
            // *Basitlik adına şimdilik burayı yorum satırı yapıyorum, Single mode işi çözer.*
        }

        // 2. Yeni Sahneyi Yükle
        // LoadSceneMode.Single: Önceki sahneyi otomatik yok eder.
        var loadOp = Addressables.LoadSceneAsync(levelAddress, LoadSceneMode.Single);

        // Yükleme sırasında barı güncelle (%0 - %50 arası)
        while (!loadOp.IsDone)
        {
            loadingBar.value = loadOp.PercentComplete * 0.5f;
            await UniTask.Yield();
        }

        if (loadOp.Status == AsyncOperationStatus.Succeeded)
        {

            // Yeni sahnenin handle'ını kaydet
            currentSceneHandle = loadOp;

            Debug.Log($"Sahne Yüklendi: {levelAddress}. Şimdi Varlıklar Yükleniyor...");

            // 3. Sahne yüklendi, şimdi LevelManager'ı bulup Müşterileri yükletelim
            // Yeni sahne aktif olana kadar 1 frame beklemek garanti olur
            await UniTask.Yield();

            LevelManager levelMgr = FindObjectOfType<LevelManager>();

            if (levelMgr != null)
            {
                // Müşterileri yükle ve Barın kalanını (%50 - %100) doldur
                await levelMgr.PrepareLevelRoutine(loadingBar);


            }
            else
            {
                // Eğer MainMenu sahnesiysek LevelManager olmayabilir, bu normaldir.
                // O yüzden sadece barı fulleyip geçebiliriz.
                loadingBar.value = 1f;
            }
        }
        else
        {
            Debug.LogError("Sahne yüklenemedi: " + levelAddress);
        }

        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f)); // Ufak bir bekleme ekleyelim ki barın dolduğunu görelim
        // doffade ekle




        loadingScreenPanel.SetActive(false);

        // 4. Her şey bitti, Loading'i kapat

    }
}