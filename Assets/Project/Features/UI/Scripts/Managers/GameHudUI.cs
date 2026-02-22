using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // DOTween'i eklemeyi unutma
using System.Collections.Generic; // Liste kullanmak için

public class GameHudUI : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private GameObject hudContainer;
    
    [SerializeField] private TextMeshProUGUI dailyIncomeText; 
    [SerializeField] private TextMeshProUGUI totalMoneyText;  
    
    [SerializeField] private TextMeshProUGUI targetIncomeText;
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private Image xpBarImage;

    [Header("Rush Effects Settings")]
    [SerializeField] private Color rushColor = new Color(1f, 0.3f, 0.3f); // Tatlı bir Kırmızı
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float pulseScale = 1.15f; // Ne kadar büyüsün? (%15)
    [SerializeField] private float pulseDuration = 0.5f; // Ne kadar sürede büyüsün?

    // Efekt uygulanacak elemanları topluca yönetmek için listeler
    private List<Graphic> rushGraphics = new List<Graphic>();
    private List<Transform> rushTransforms = new List<Transform>();

    void Awake()
    {
        // Listeleri doldur (Kod temizliği için)
        // Graphic listesi Renk değişimi için:
        rushGraphics.Add(dailyIncomeText);
        rushGraphics.Add(targetIncomeText);
        rushGraphics.Add(clockText);
        rushGraphics.Add(xpBarImage);

        // Transform listesi Büyüme/Küçülme (Scale) için:
        rushTransforms.Add(dailyIncomeText.transform);
        rushTransforms.Add(targetIncomeText.transform);
        rushTransforms.Add(clockText.transform);
    }

    void Start()
    {
        hudContainer.SetActive(false);
        if(totalMoneyText != null) 
            totalMoneyText.text = "$" + GameDataManager.Instance.totalMoney.ToString("F2");
    }

    void OnEnable()
    {
        EventBus.Subscribe<UIEvent.UpdateMoneyTextEvent>(OnMoneyUpdate);
        EventBus.Subscribe<UIEvent.UpdateDailyIncomeTextEvent>(OnDailyIncomeUpdate);
        EventBus.Subscribe<UIEvent.UpdateClockEvent>(OnClockUpdate);
        EventBus.Subscribe<UIEvent.UpdateLevelBarEvent>(OnLevelBarUpdate);
        EventBus.Subscribe<UIEvent.DayStartingEvent>(OnDayStart);
        EventBus.Subscribe<GameEvents.RushTimeEvent>(OnRushTimeStart);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIEvent.UpdateMoneyTextEvent>(OnMoneyUpdate);
        EventBus.Unsubscribe<UIEvent.UpdateDailyIncomeTextEvent>(OnDailyIncomeUpdate);
        EventBus.Unsubscribe<UIEvent.UpdateClockEvent>(OnClockUpdate);
        EventBus.Unsubscribe<UIEvent.UpdateLevelBarEvent>(OnLevelBarUpdate);
        EventBus.Unsubscribe<UIEvent.DayStartingEvent>(OnDayStart);
        EventBus.Unsubscribe<GameEvents.RushTimeEvent>(OnRushTimeStart);
        
        // Obje kapanırken tweenleri öldür ki hata vermesin
        KillAllTweens();
    }

    private void OnRushTimeStart(GameEvents.RushTimeEvent evt)
    {
        ToggleRushVisuals(evt.isRushHour);
    }

    // --- DOTWEEN MAGIC ---
    private void ToggleRushVisuals(bool isRush)
    {
        // 1. Önceki tüm animasyonları durdur (Çatışma olmasın)
        KillAllTweens();

        if (isRush)
        {
            Debug.Log("🔥 HUD: Rush Modu Görselleri Aktif!");

            // RENK DEĞİŞİMİ
            foreach (var graphic in rushGraphics)
            {
                if(graphic != null) 
                    graphic.DOColor(rushColor, 0.3f);
            }

            // SCALE (BÜYÜME/KÜÇÜLME) LOOP'U
            foreach (var trans in rushTransforms)
            {
                if (trans != null)
                {
                    // LoopType.Yoyo: Büyür -> Küçülür -> Büyür... (Nefes alma efekti)
                    trans.DOScale(Vector3.one * pulseScale, pulseDuration)
                         .SetEase(Ease.InOutSine)
                         .SetLoops(-1, LoopType.Yoyo); 
                }
            }
        }
        else
        {
            Debug.Log("✅ HUD: Normal Moda Dönüş");

            // RENGİ BEYAZ YAP
            foreach (var graphic in rushGraphics)
            {
                if(graphic != null) 
                    graphic.DOColor(normalColor, 0.3f);
            }

            // BOYUTU NORMALE DÖNDÜR
            foreach (var trans in rushTransforms)
            {
                if (trans != null)
                {
                    trans.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                }
            }
        }
    }

    private void KillAllTweens()
    {
        foreach (var trans in rushTransforms)
        {
            if (trans != null) trans.DOKill();
        }
        foreach (var graphic in rushGraphics)
        {
            if (graphic != null) graphic.DOKill();
        }
    }

    // --- DIĞER EVENTLER (Aynı Kaldı) ---

    private void OnDayStart(UIEvent.DayStartingEvent evt)
    {
        hudContainer.SetActive(true);
        targetIncomeText.text = "Goal: $" + evt.levelManager.targetDailyIncome.ToString("F2");
        dailyIncomeText.text = "$0.00"; 
        
        // Gün başında renkleri ve boyutu sıfırla (Garanti olsun)
        ToggleRushVisuals(false);
    }

    private void OnMoneyUpdate(UIEvent.UpdateMoneyTextEvent evt)
    {
        if (totalMoneyText != null) totalMoneyText.text = "$" + evt.currentMoney.ToString("F2");
    }

    private void OnDailyIncomeUpdate(UIEvent.UpdateDailyIncomeTextEvent evt)
    {
        dailyIncomeText.text = "$:" + evt.dailyIncome.ToString("F2");
        
        // Para geldiğinde ufak bir zıplama efekti (Ekstra Polish ✨)
        if(!DOTween.IsTweening(dailyIncomeText.transform)) // Eğer zaten Rush animasyonu yoksa zıplasın
             dailyIncomeText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.5f);
    }

    private void OnClockUpdate(UIEvent.UpdateClockEvent evt) => clockText.text = $"{evt.hour:00}:{evt.minute:00}";
    private void OnLevelBarUpdate(UIEvent.UpdateLevelBarEvent evt) => xpBarImage.fillAmount = evt.fillAmount;
}