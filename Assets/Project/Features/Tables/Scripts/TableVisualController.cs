using UnityEngine;
using System.Collections.Generic;
using PrimeTween;

public class TableVisualController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private OrderDisplayUI orderDisplay;
    [SerializeField] private TableNotificationUI notificationUI;

    [Header("Visual Elements")]
    [SerializeField] private SpriteRenderer tableRenderer;
    [SerializeField] private SpriteRenderer[] plateRenderers;

    [SerializeField] private Sprite dirtyPlateSprite;

    public Transform coinUITarget;

    private Color defaultColor;

    void Awake()
    {
        if (tableRenderer == null) tableRenderer = GetComponent<SpriteRenderer>();
        defaultColor = tableRenderer.color;


        // Başlangıçta tabakları gizle
        foreach (var plate in plateRenderers)
        {
            if (plate != null) plate.gameObject.SetActive(false);
        }
    }

    // --- UI GÖSTERİM METOTLARI ---

    public void MarkFoodAsServed(Sprite foodSprite)
    {
        orderDisplay.MarkFoodAsServed(foodSprite);
    }
    public void ShowReadyToOrder(float patienceDuration)
    {
        orderDisplay.HideOrder();
        notificationUI.ShowNotification(patienceDuration);
        //patienceUI.SetupBar(patienceDuration, PatienceBarType.waiterWait);
    }

    public void ShowWaitingForFood(List<OrderItemSO> items  , float displayDuration)
    {
        notificationUI.HideNotification();

        orderDisplay.gameObject.SetActive(true);

        orderDisplay.DisplayOrder(items ,displayDuration);
      
        //patienceUI.SetupBar(prepDuration, PatienceBarType.orderWait);
    }


    public void HideAllUI()
    {
        orderDisplay.HideOrder();
        notificationUI.HideNotification();
        //patienceUI.HideBar();
    }

    // tabak yönetimi
    public void SetFoodOnPlate(int index, Sprite foodSprite)
    {
        if (index >= 0 && index < plateRenderers.Length)
        {
            plateRenderers[index].sprite = foodSprite;
            plateRenderers[index].gameObject.SetActive(true);

            
        }
    }

    public void SetAllPlatesDirty()
    {
        for (int i = 0; i < plateRenderers.Length; i++)
        {
            if (plateRenderers[i].gameObject.activeSelf && plateRenderers[i].sprite != dirtyPlateSprite)
            {
                plateRenderers[i].sprite = dirtyPlateSprite;
            }
        }
    }

    public void MakeDirtyToActivePlates() // masadak olan tabkaları kirtli bulaşık yapar
    {
        if (plateRenderers != null)
        {
            foreach (var plate in plateRenderers)
            {
                if(plate.sprite != null)
                {
                    DishesStation.Instance.AddDirtyDishes(1);
                }
            }
        }
    }

    public void HidePlate(int index)
    {
        if (index >= 0 && index < plateRenderers.Length)
        {
            plateRenderers[index].gameObject.SetActive(false);
            plateRenderers[index].sprite = null;
        }
    }

    public bool IsPlateDirty(int index)
    {
        if (index >= 0 && index < plateRenderers.Length)
        {
            return plateRenderers[index].sprite == dirtyPlateSprite;
        }
        return false;
    }

    public void ResetAllVisuals()
    {
        foreach (var plate in plateRenderers)
        {
            plate.gameObject.SetActive(false);
            plate.sprite = null;
        }
        HideAllUI();

        Tween.StopAll(transform);
        transform.localScale = Vector3.one;
        tableRenderer.color = defaultColor;
    }

    // Effects
    public void PlaySelectionEffect(bool isSelected)
    {
        Tween.StopAll(transform); 

        if (isSelected)
        {
            tableRenderer.color = new Color(0.41f, 0.94f, 0.68f, 1f);
            Tween.Scale(transform, endValue: 1.035f, duration: 0.6f, cycles: -1, cycleMode: CycleMode.Yoyo);
        }
        else
        {
            tableRenderer.color = defaultColor;
            Tween.Scale(transform, endValue: 1f, duration: 0.2f);
        }
    }

    public void PlayInteractionPunch()
    {
        Tween.PunchScale(transform, strength: new Vector3(0.1f, -0.1f, 0), duration: 0.3f, frequency: 10);
    }
  public void PlayCoinEffect(List<GameObject> coins)
    {
        for (int i = 0; i < coins.Count; i++)
        {
            GameObject coin = coins[i];
            
            coin.transform.localEulerAngles = Vector3.zero;
            
            float delayAmount = (i * 0.08f) + Random.Range(0f, 0.05f);

            Tween.Position(coin.transform, endValue: coinUITarget.position, duration: 0.65f, startDelay: delayAmount, ease: Ease.InBack)
                .OnComplete(() => 
                {
                    CoinsPool.Instance.ReturnCoinItem(coin);
                    // sesler burayay gelebilri
                });

            Vector3 randomSpin = new Vector3(0, 0, Random.Range(-360f, -720f));
            Tween.LocalEulerAngles(coin.transform, startValue: Vector3.zero, endValue: randomSpin, duration: 0.85f, startDelay: delayAmount, ease: Ease.InOutQuad);

            Tween.Scale(coin.transform, endValue: Vector3.zero, duration: 0.65f, startDelay: delayAmount + 0.3f, ease: Ease.InBack);
        }

        coins.Clear(); 
    }
}