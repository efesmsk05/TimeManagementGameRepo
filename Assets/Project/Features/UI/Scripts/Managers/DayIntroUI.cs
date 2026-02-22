using UnityEngine;
using TMPro;
using DG.Tweening;

public class DayIntroUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform introPanelRect;
    [SerializeField] private TextMeshProUGUI dayNameText;

    [SerializeField] private TextMeshProUGUI targetIncomeText;


    [Header("Animation Settings")]
    [SerializeField] private Vector2 offScreenPos = new Vector2(0, 1200);
    [SerializeField] private Vector2 onScreenPos = Vector2.zero;
    [SerializeField] private float animSpeed = 0.5f;

    void Start()
    {
        introPanelRect.anchoredPosition = offScreenPos;
        introPanelRect.gameObject.SetActive(false);
    }

    void OnEnable() => EventBus.Subscribe<UIEvent.LevelIntroEvent>(OnPlayIntro);
    void OnDisable() => EventBus.Unsubscribe<UIEvent.LevelIntroEvent>(OnPlayIntro);

    private void OnPlayIntro(UIEvent.LevelIntroEvent evt)
    {
        // Metni Güncelle
        dayNameText.text = $"Day {GameDataManager.Instance.currentDayIndex}\n{evt.levelData.levelName}";

        targetIncomeText.text = $"Goal: ${evt.levelManager.targetDailyIncome:F2}";

        // Animasyon Dizisi
        Sequence seq = DOTween.Sequence();

        introPanelRect.gameObject.SetActive(true);
        
        // 1. İniş
        seq.Append(introPanelRect.DOAnchorPos(onScreenPos, animSpeed).SetEase(Ease.OutBack));
        
        // 2. Bekleme
        float waitTime = Mathf.Max(0.1f, evt.introDuration - 1.0f);
        seq.AppendInterval(waitTime);

        // 3. Çıkış
        seq.Append(introPanelRect.DOAnchorPos(offScreenPos, animSpeed).SetEase(Ease.InBack));

        // 4. Kapanış
        seq.OnComplete(() => introPanelRect.gameObject.SetActive(false));
    }
}