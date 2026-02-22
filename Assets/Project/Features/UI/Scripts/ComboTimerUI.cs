using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ComboTimerUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject hudContainer; // Tüm paneli açıp kapatmak için
    [SerializeField] private Image radialFillImage;   // Dönen renkli çember
    [SerializeField] private TextMeshProUGUI xCountText; // Ortadaki x2, x5 yazısı

    [Header("Visual Settings")]
    [SerializeField] private Gradient timerGradient; // Yeşil -> Kırmızı geçişi
    [SerializeField] private float punchScale = 0.2f; // Zıplama miktarı

    private int lastComboCount = 0;

    void Start()
    {
        // Başlangıçta gizle
        if(hudContainer != null) hudContainer.SetActive(false);
    }

    void Update()
    {
        // 1. Manager Yoksa Çalışma
        if (ComboManager.Instance == null) return;

        // 2. Verileri Manager'dan Çek
        int currentCount = ComboManager.Instance.CurrentComboCount;
        float ratio = ComboManager.Instance.CurrentComboTimeRatio; // 0.0 ile 1.0 arası gelir

        // --- GÖRÜNÜRLÜK KONTROLÜ ---
        // Combo 2'den küçükse gizle ve çık
        if (currentCount < 2)
        {
            if (hudContainer.activeSelf)
            {
                hudContainer.SetActive(false);
                lastComboCount = 0; // Reset
            }
            return;
        }

        // --- EĞER BURADAYSAK COMBO VAR DEMEKTİR ---
        
        // Kapalıysa aç
        if (!hudContainer.activeSelf) hudContainer.SetActive(true);

        // 3. Barı Doldur (AKICI KISIM)
        if (radialFillImage != null)
        {
            radialFillImage.fillAmount = ratio; // Manager'dan gelen gerçek oran

            // Renk değişimi (Gradient varsa)
            if (timerGradient != null)
                radialFillImage.color = timerGradient.Evaluate(1f - ratio); 
        }

        // 4. Metni Güncelle
        if (xCountText != null)
        {
            xCountText.text = $"x{currentCount}";
        }

        // 5. Efekt (Sadece sayı değiştiğinde bir kere zıplat)
       if (currentCount != lastComboCount)
        {
            // DİKKAT: Scriptin takılı olduğu objeyi değil, GÖRSELİ (hudContainer) zıplatıyoruz.
            Transform targetTransform = hudContainer.transform;

            targetTransform.DOKill(); // Önceki animasyonu öldür
            targetTransform.localScale = Vector3.one; // Boyutu normale döndür (Hata birikmesin)
            
            // Ayarlar: Scale gücü, Süre (0.3s), Titreşim (5), Esneklik (1)
            targetTransform.DOPunchScale(Vector3.one * punchScale, 0.3f, 5, 1f);
            
            lastComboCount = currentCount;
        }
    }
}