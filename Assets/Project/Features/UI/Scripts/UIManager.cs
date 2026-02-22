using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("DayOpenning Panel Elements")]
    [SerializeField] private CanvasGroup dayOpenningPanelCanvasGroup;
    [SerializeField] private RectTransform tabletPanel;
    //money
    [SerializeField] private TextMeshProUGUI moneyText;
    // star
    [SerializeField] private TextMeshProUGUI honorText ;

    [SerializeField] private TextMeshProUGUI currentDayText ;

    // tablet exp bar
    [SerializeField] private UnityEngine.UI.Image TabletexpFillBar;

    [SerializeField] private float expBarFillAmount;

    [SerializeField] private TextMeshProUGUI levelNumberText;

    [SerializeField] private TextMeshProUGUI reqMoneyText;

    [SerializeField] private TextMeshProUGUI reqXpText;



    [Header("Upper Panel Texts")]
    [SerializeField] private GameObject upperPanel;
    [SerializeField] private TextMeshProUGUI dailyIncomeText;
    [SerializeField] private TextMeshProUGUI targetDailyIncomeText;

    [Header("Exp Level Bar Elements")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private UnityEngine.UI.Image expFillBar;

    [Header("Intro Panel Elements")]
    [SerializeField] private RectTransform introPanel;
    [SerializeField] private TextMeshProUGUI dayNameText;
    [SerializeField] private TextMeshProUGUI targetIncomeAmountText;


    [Header("Day End Panel Elements")]
    [SerializeField] private RectTransform dayEndPanel; // --- EKLENDİ: Panel Referansı ---
    [SerializeField] private TextMeshProUGUI dayEndNameText;
    [SerializeField] private TextMeshProUGUI dayEndtargetIncomeAmountText;
    [SerializeField] private TextMeshProUGUI dayEndDailyIncomeAmountText;

    [SerializeField] private TextMeshProUGUI dayEndResultText;

    [SerializeField] private UnityEngine.UI.Button nextDayButton;



    public Vector2 offScreenPos = new Vector2(0, 1200); 
    public Vector2 onScreenPos = Vector2.zero; 

    [Header("Intro Panel Settings")]
    public float introSpeed = 0.5f; 

    [Header("Clock Panel Elements")]
    public TextMeshProUGUI clockText;

    void Start()
    {
        dailyIncomeText.text = "$:$0.00";
        
        // --- Intro Panel Başlangıç Ayarı ---
        introPanel.gameObject.SetActive(false);
        introPanel.anchoredPosition = offScreenPos;

        // --- Day End Panel Başlangıç Ayarı ---
        if (dayEndPanel != null)
        {
            dayEndPanel.gameObject.SetActive(false);
            dayEndPanel.anchoredPosition = offScreenPos;
        }

        if(dayOpenningPanelCanvasGroup != null)
        {
            dayOpenningPanelCanvasGroup.alpha = 0f; // Başlangıçta görünmez yap
        }

        if(upperPanel != null)
        {
            upperPanel.SetActive(false);
        }

        if(expFillBar != null)
        {
            expFillBar.fillAmount = 0f;
        }

        moneyText.text = GameDataManager.Instance.totalMoney.ToString("F2");


    }

    void OnEnable()
    {
        EventBus.Subscribe<UIEvent.UpdateDailyIncomeTextEvent>(HandleUpdateDailyIncomeText);
        EventBus.Subscribe<UIEvent.LevelIntroEvent>(OnLevelIntro);
        EventBus.Subscribe<UIEvent.UpdateClockEvent>(HandleUpdateClock);
        EventBus.Subscribe<UIEvent.LevelEndEvent>(HandleLevelEnd);

        EventBus.Subscribe<UIEvent.ShopOpeningEvent>( OnShopOpening);
        EventBus.Subscribe<UIEvent.DayStartingEvent>( OnDayStarting);

        EventBus.Subscribe<UIEvent.UpdateMoneyTextEvent>(OnMoneyUpdate);

        EventBus.Subscribe<UIEvent.UpdateLevelBarEvent>(OnUpdateLevelBar);

        EventBus.Subscribe<UIEvent.LevelUpEvent>(LevelUpHandler);

        EventBus.Subscribe<UIEvent.ComboUpdateEvent>(OnComboUpdate);
 
 
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<UIEvent.UpdateDailyIncomeTextEvent>(HandleUpdateDailyIncomeText);
        EventBus.Unsubscribe<UIEvent.LevelIntroEvent>(OnLevelIntro);
        EventBus.Unsubscribe<UIEvent.UpdateClockEvent>(HandleUpdateClock);
        EventBus.Unsubscribe<UIEvent.LevelEndEvent>(HandleLevelEnd);

        EventBus.Unsubscribe<UIEvent.ShopOpeningEvent>( OnShopOpening);
        EventBus.Unsubscribe<UIEvent.DayStartingEvent>( OnDayStarting);

        EventBus.Unsubscribe<UIEvent.UpdateMoneyTextEvent>(OnMoneyUpdate);

        EventBus.Unsubscribe<UIEvent.UpdateLevelBarEvent>(OnUpdateLevelBar);

        EventBus.Unsubscribe<UIEvent.LevelUpEvent>(LevelUpHandler);

        EventBus.Unsubscribe<UIEvent.ComboUpdateEvent>(OnComboUpdate);

    }

    private void OnShopOpening(UIEvent.ShopOpeningEvent evt) 
    {
        // dükkan açılı tableti gümceller

        expBarFillAmount = GameDataManager.Instance.currentXp / evt.levelData.requiredReputation;
        TabletexpFillBar.fillAmount = expBarFillAmount;
        levelNumberText.text = evt.levelData.levelIndex.ToString();
        reqMoneyText.text = "$" + evt.levelData.costToUpgrade.ToString("F2");
        reqXpText.text = evt.levelData.requiredReputation.ToString("F2");
        currentDayText.text = "Day " + GameDataManager.Instance.currentDayIndex.ToString();
        moneyText.text = "$" + GameDataManager.Instance.totalMoney.ToString("F2");
        honorText.text = "XP " + GameDataManager.Instance.currentXp.ToString("F2");

        ///

        targetIncomeAmountText.text = "Goal:$" + evt.levelManager.targetDailyIncome.ToString("F2");
        targetDailyIncomeText.text = "Goal:$" + evt.levelManager.targetDailyIncome.ToString("F2");
        dailyIncomeText.text = "$:$0.00";

        dayOpenningPanelCanvasGroup.DOFade(1f, 0.25f).SetEase(Ease.InOutSine);

    }

    private void OnDayStarting(UIEvent.DayStartingEvent evt)
    {

        dayOpenningPanelCanvasGroup.DOFade(0f, .25f).SetEase(Ease.InOutSine).OnComplete(() => 
        {

            if(dayOpenningPanelCanvasGroup != null)
            {
                dayOpenningPanelCanvasGroup.gameObject.SetActive(false);
            }

            if(upperPanel != null)
            {
                upperPanel.SetActive(true);
            }
        });
        // Gün başlangıcı için gerekli UI güncellemeleri burada yapılabilir.
        // Örneğin, gün başlangıcı panelini göstermek gibi.
    }

    private void LevelUpHandler(UIEvent.LevelUpEvent evt)
    {
        // Level atlama durumunda UI güncellemeleri burada yapılabilir.
        // Örneğin, level numarasını güncellemek gibi.
        reqMoneyText.text = "$" + evt.currentLevelData.costToUpgrade.ToString("F2");
        reqXpText.text = evt.currentLevelData.requiredReputation.ToString("F2");
        expBarFillAmount = 0f;
        TabletexpFillBar.fillAmount = expBarFillAmount;

        levelNumberText.text = evt.currentLevelData.levelIndex.ToString();

    }

    private void OnUpdateLevelBar(UIEvent.UpdateLevelBarEvent evt)
    {

        expFillBar.fillAmount = evt.fillAmount;

        //int currentLevel = ShopLevelManager.Instance.GetCurrentLevel();
        // level sayısı güncellenecek 
        //levelText.text = "Level " + currentLevel.ToString();

    }

    private void OnMoneyUpdate(UIEvent.UpdateMoneyTextEvent evt)
    {
        moneyText.text = "$" + evt.currentMoney.ToString("F2");
    }

    private void HandleUpdateDailyIncomeText(UIEvent.UpdateDailyIncomeTextEvent evt)
    {
        dailyIncomeText.text = "$:" + evt.dailyIncome.ToString("F2");
    }

    private void OnLevelIntro(UIEvent.LevelIntroEvent evt)
    {

        dayNameText.text = "Day: " + GameDataManager.Instance.currentDayIndex + " - " + evt.levelData.levelName;
        Sequence seq = DOTween.Sequence();

        // 1. Paneli hazırla
        introPanel.anchoredPosition = offScreenPos;
        introPanel.gameObject.SetActive(true);

        // 2. Animasyon A: Ekrana İniş
        seq.Append(introPanel.DOAnchorPos(onScreenPos, introSpeed).SetEase(Ease.OutBack));
        
        float waitTime = evt.introDuration - 1.0f; 
        if(waitTime < 0) waitTime = 0.1f; 
        
        seq.AppendInterval(waitTime);

        // 4. Animasyon C: Ekrandan Çıkış
        seq.Append(introPanel.DOAnchorPos(offScreenPos, introSpeed).SetEase(Ease.InBack));

        // 5. Bitiş: Paneli kapat
        seq.OnComplete(() => introPanel.gameObject.SetActive(false));
    }

    private void HandleUpdateClock(UIEvent.UpdateClockEvent evt)
    {
        clockText.text = string.Format("{0:00}:{1:00}", evt.hour, evt.minute);
    }

    private void HandleLevelEnd(UIEvent.LevelEndEvent evt)
    {
        dayEndNameText.text =  "Day :"+GameDataManager.Instance.currentDayIndex + " - " + evt.levelManager.GetCurrentLevel().levelName;
        dayEndtargetIncomeAmountText.text = "Goal: $" + evt.levelManager.targetDailyIncome.ToString("F2");
        dayEndDailyIncomeAmountText.text = "Daily $: $" + GameDataManager.Instance.totalMoney.ToString("F2");

        if (evt.isLevelSuccessful)
        {
            dayEndResultText.text = "Success!";
            dayEndResultText.color = Color.green;
            // next day butonu aktif
            nextDayButton.gameObject.SetActive(true);
        }
        else
        {
            dayEndResultText.text = "Failed!";
            dayEndResultText.color = Color.red;
            // next day butonu pasif
            nextDayButton.gameObject.SetActive(false);
        }
        
        dayEndPanel.anchoredPosition = offScreenPos; // Yukarı taşı
        dayEndPanel.gameObject.SetActive(true);      // Aç
        
        // Aşağı in ve orada kal
        dayEndPanel.DOAnchorPos(onScreenPos, introSpeed).SetEase(Ease.OutBack);
    }

    // ui Button
    public void DayRepeatButton()
    {
        // Butona basınca paneli yukarı gönderip sahneyi yenileyebiliriz (Opsiyonel görsel şov)
        dayEndPanel.DOAnchorPos(offScreenPos, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            GameDataManager.Instance.SaveGame();
            SceneLoader.Instance.LoadNewLevel(SceneManager.GetActiveScene().name);
        });
    }

    public void ExitToMainMenuButton()
    {
        Debug.Log("Main Menu'ye Dönülüyor...");
        GameDataManager.Instance.SaveGame();

        dayEndPanel.DOAnchorPos(offScreenPos, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            SceneLoader.Instance.LoadNewLevel("MainMenu");
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

    // player 
    // combo
    private void OnComboUpdate(UIEvent.ComboUpdateEvent evt)
    {
        Debug.Log($"Combo Güncellendi: {evt.currentCombo}");
    }
}