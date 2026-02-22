using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private TextMeshProUGUI dayInfoText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI earningsText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button nextDayButton;

    [Header("Settings")]
    [SerializeField] private Vector2 offScreenPos = new Vector2(0, 1200);

    void Start()
    {
        panelRect.anchoredPosition = offScreenPos;
        panelRect.gameObject.SetActive(false);
    }

    void OnEnable() => EventBus.Subscribe<UIEvent.LevelEndEvent>(OnLevelEnd);
    void OnDisable() => EventBus.Unsubscribe<UIEvent.LevelEndEvent>(OnLevelEnd);

    private void OnLevelEnd(UIEvent.LevelEndEvent evt)
    {
        // Bilgileri Doldur
        dayInfoText.text = $"Day {GameDataManager.Instance.currentDayIndex} - {evt.levelManager.currentLevelData.levelName}";
        goalText.text = $"Goal: ${evt.levelManager.targetDailyIncome:F2}";
        earningsText.text = $"Daily: ${EconomyManager.Instance.dailyIncome:F2}"; // Burası günlük kazanç olmalı, total değilse düzeltirsin

        // Sonuç Durumu
        if (evt.isLevelSuccessful)
        {
            resultText.text = "SUCCESS!";
            resultText.color = Color.green;
            nextDayButton.gameObject.SetActive(true);
        }
        else
        {
            resultText.text = "FAILED!";
            resultText.color = Color.red;
            nextDayButton.gameObject.SetActive(false);
        }

        // Paneli Aç
        panelRect.gameObject.SetActive(true);
        panelRect.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
    }

    // --- BUTON FONKSİYONLARI ---
    
    public void DayRepeat()
    {
        panelRect.DOAnchorPos(offScreenPos, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            GameDataManager.Instance.SaveGame();
            SceneLoader.Instance.LoadNewLevel(SceneManager.GetActiveScene().name);
        });
    }

    public void OnMainMenuClicked()
    {
        GameDataManager.Instance.SaveGame();
        panelRect.DOAnchorPos(offScreenPos, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
        {
            SceneLoader.Instance.LoadNewLevel("MainMenu");
        });
    }
}