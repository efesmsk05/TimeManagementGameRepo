using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DayPrepUI : MonoBehaviour
{
    [Header("Tablet Elements")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI currentDayText;
    [SerializeField] private TextMeshProUGUI moneyText;     // Tablet üzerindeki para
    [SerializeField] private TextMeshProUGUI honorText;     // Tablet üzerindeki XP/Honor
    
    [Header("Level Info")]
    [SerializeField] private TextMeshProUGUI levelNumberText;
    [SerializeField] private TextMeshProUGUI reqMoneyText;
    [SerializeField] private TextMeshProUGUI reqXpText;
    [SerializeField] private Image xpFillBar;

    public Vector2 offScreenPos = new Vector2(0, 1200); 
    public Vector2 onScreenPos = Vector2.zero; 

    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        EventBus.Subscribe<UIEvent.UpdateMoneyTextEvent>(OnUpdateMoneyTextEvent);

        EventBus.Subscribe<UIEvent.ShopOpeningEvent>(OnShopOpening);
        EventBus.Subscribe<UIEvent.DayStartingEvent>(OnDayStarting);
        EventBus.Subscribe<UIEvent.LevelUpEvent>(OnLevelUp);
    }

    void OnDisable()
    {

        EventBus.Unsubscribe<UIEvent.UpdateMoneyTextEvent>(OnUpdateMoneyTextEvent);


        EventBus.Unsubscribe<UIEvent.ShopOpeningEvent>(OnShopOpening);
        EventBus.Unsubscribe<UIEvent.DayStartingEvent>(OnDayStarting);
        EventBus.Unsubscribe<UIEvent.LevelUpEvent>(OnLevelUp);
    }

    private void OnShopOpening(UIEvent.ShopOpeningEvent evt)
    {
        canvasGroup.gameObject.SetActive(true);
        
        // Verileri Doldur
        currentDayText.text = "Day " + GameDataManager.Instance.currentDayIndex;
        moneyText.text = "$" + GameDataManager.Instance.totalMoney.ToString("F2");
        honorText.text = "XP " + GameDataManager.Instance.currentXp.ToString("F2");

        UpdateLevelInfo(evt.levelData);

        // Animasyon
        canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.InOutSine);
    }

    private void UpdateLevelInfo(LevelDataSO data)
    {
        levelNumberText.text = data.levelIndex.ToString();
        reqMoneyText.text = "$" + data.costToUpgrade.ToString("F2");
        reqXpText.text = data.requiredReputation.ToString("F2");
        
        float ratio = GameDataManager.Instance.currentXp / data.requiredReputation;
        xpFillBar.fillAmount = Mathf.Clamp01(ratio);
    }

    private void OnUpdateMoneyTextEvent(UIEvent.UpdateMoneyTextEvent evt)
    {
        moneyText.text = "$" + evt.currentMoney.ToString("F2");
    }

    private void OnLevelUp(UIEvent.LevelUpEvent evt)
    {
        UpdateLevelInfo(evt.currentLevelData);
    }

    private void OnDayStarting(UIEvent.DayStartingEvent evt)
    {
        // Gün başlayınca tableti kapat
        canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InOutSine).OnComplete(() => 
        {
            canvasGroup.gameObject.SetActive(false);
        });
    }

    public void OpenPanel(RectTransform rectTransform)
    {
        rectTransform.DOAnchorPos(onScreenPos, 0.5f).SetEase(Ease.OutBack);


    }

    public void ExitPanel(RectTransform rectTransform)
    {
        rectTransform.DOAnchorPos(offScreenPos, 0.5f).SetEase(Ease.InBack);

    }
}