using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Unity.VisualScripting;

public class DishesStation : MonoBehaviour
{
    public static DishesStation Instance;

    [Header("--- Visual Stack References ---")]
    [SerializeField] private PlatesStackVisual kitchenCleanStackVisual; 
    [SerializeField] private PlatesStackVisual sinkDirtyStackVisual; 

    private Vector3 orginalScale;
    private Vector3 originalLocalPosition;

    [Header("--- Prefabs ---")]
    [SerializeField] private GameObject cleanPlatePrefab;
    [SerializeField] private GameObject dirtyPlatePrefab;

    [Header("--- UI References (PrimeTween Bar) ---")]
    [SerializeField] private GameObject progressBarCanvas; 
    [SerializeField] private Image progressBarFill;        

    [Header("--- Settings ---")]
    public float plateWashTime = 1.5f; // tabak başı speed 

    public float dishesSpeedMultiplier = 0.40f; 
    public int totalPlateCapacity = 10;

    public Ease barOpenEase = Ease.OutBack;
    public Ease barCloseEase = Ease.InBack;
    public Ease barFillEase = Ease.Linear;

    private Tween dishesTween;

    [Tooltip("Bar açıldığında ulaşacağı son boyut")]
    [SerializeField] private Vector3 barTargetScale = new Vector3(0.1f, 0.1f, 0.1f);

    public Transform washPointOffset; 

    [Header("--- Live Data ---")]
    public int cleanDishesCount;
    private int dirtyDishesCount = 0;

    private CancellationTokenSource cts; 

    public bool dishesIsFull => dirtyDishesCount >= totalPlateCapacity;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {

        orginalScale = this.transform.localScale;
        originalLocalPosition = this.transform.localPosition;
        
        cleanDishesCount = totalPlateCapacity;
        dirtyDishesCount = 0;

        if (progressBarCanvas != null)
        {
            progressBarCanvas.SetActive(false);
            progressBarCanvas.transform.localScale = Vector3.zero;
        }

        if(kitchenCleanStackVisual != null) 
            kitchenCleanStackVisual.Initialize(totalPlateCapacity, cleanPlatePrefab);
            
        if(sinkDirtyStackVisual != null) 
            sinkDirtyStackVisual.Initialize(totalPlateCapacity, dirtyPlatePrefab);

        UpdateAllVisuals();
    }

    public bool TryGetCleanDish(int amount)
    {
        if (cleanDishesCount >= amount)
        {
            cleanDishesCount -= amount;
            UpdateAllVisuals();
            return true;
        }
        return false;
    }

    public bool isHaveCleanDishes(int groupSize)
    {
        return cleanDishesCount >= groupSize;
    }

    public void AddDirtyDishes(int count) 
    {
        dirtyDishesCount += count;
        if (dirtyDishesCount > totalPlateCapacity) dirtyDishesCount = totalPlateCapacity;
        UpdateAllVisuals();
        ShowAddingDishesEffect();

    }

    public void WashDishes()
    {
        if (dirtyDishesCount <= 0) return;
        if (cts != null) return;

        
        if (progressBarCanvas != null)
        {
            Tween.StopAll(progressBarCanvas.transform);
            if (progressBarFill != null) Tween.StopAll(progressBarFill);

            progressBarCanvas.SetActive(true);
            progressBarFill.fillAmount = 0f;

            progressBarCanvas.transform.localScale = Vector3.zero;
            
            // PrimeTween Scale Animasyonu
            Tween.Scale(progressBarCanvas.transform, endValue: barTargetScale, duration: 0.3f, ease: barOpenEase);
        }

        cts = new CancellationTokenSource();
        WashDishesRoutine(cts.Token).Forget();
    }

    public async UniTaskVoid WashDishesRoutine(CancellationToken token)
    {
        float totalDuration = (dirtyDishesCount * plateWashTime) * dishesSpeedMultiplier;

        totalDuration = Mathf.Max(totalDuration, 0.5f);

        // PrimeTween Fill Animasyonu
        Tween fillTween = Tween.UIFillAmount(progressBarFill, endValue: 1f, duration: totalDuration, ease: barFillEase); 

        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(totalDuration), cancellationToken: token).SuppressCancellationThrow();
        if (isCancelled)
        {
            fillTween.Stop(); // Kill() yerine Stop()
            ResetToken();
            return;
        }

        // --- YIKAMA TAMAMLANDI ---
        
        int washed = dirtyDishesCount;
        dirtyDishesCount = 0;
        cleanDishesCount += washed;

        Debug.Log($"✨ Pırıl Pırıl! {washed} tabak yıkandı.");

        // --- YIKAMA TAMAMLANDI ---

        if (dishesTween.isAlive) dishesTween.Stop();
        this.transform.localPosition = originalLocalPosition;
        
        await UniTask.Delay(100); 

        CloseBarWithAnimation();
        UpdateAllVisuals();
        
        if (cts != null)
        {
            cts.Dispose();
            cts = null;
        }
    }

    public void CancelWashing()
    {
        if (cts != null)
        {
            cts.Cancel();
            ResetToken();
        }
    }

    private void ResetToken()
    {
        CloseBarWithAnimation();

        if (cts != null)
        {
            cts.Dispose();
            cts = null;
        }
    }

    private void OnDisable()
    {
        // Güvenli durdurmalar
        if(progressBarCanvas != null) Tween.StopAll(progressBarCanvas.transform);
        if(progressBarFill != null) Tween.StopAll(progressBarFill);
        CancelWashing();
    }

    // --- VISUAL UPDATE METHOD ---
    private void UpdateAllVisuals()
    {
        if (kitchenCleanStackVisual != null) 
            kitchenCleanStackVisual.UpdateStackCount(cleanDishesCount);
            
        if (sinkDirtyStackVisual != null)
        {
            sinkDirtyStackVisual.UpdateStackCount(dirtyDishesCount);
            
        }

    }

    private void CloseBarWithAnimation()
    {
        if (progressBarCanvas != null && progressBarCanvas.activeSelf)
        {
            Tween.StopAll(progressBarCanvas.transform);
            
            // PrimeTween Kapanış Animasyonu
            Tween.Scale(progressBarCanvas.transform, endValue: Vector3.zero, duration: 0.2f, ease: barCloseEase)
                .OnComplete(() => 
                {
                    progressBarCanvas.SetActive(false);
                });
        }
    }

    private void ShowAddingDishesEffect()
    {
        Tween.StopAll(this.transform); // Varsa eski animasyonları durdur
        
        // 1. Güvenlik sıfırlaması
        this.transform.localScale = orginalScale; 
        this.transform.localPosition = originalLocalPosition; 

        float fillRatio = (float)dirtyDishesCount / totalPlateCapacity;

        // 2. DÜZ BÜYÜME (Sünme yok, her eksenden orantılı büyür)
        // Doluluk oranına göre %10 ile %25 arası orantılı büyür
        float scaleMultiplier = Mathf.Lerp(1.1f, 1.25f, fillRatio); 
        
        Tween.Scale(this.transform, endValue: orginalScale * scaleMultiplier, duration: 0.15f, ease: Ease.OutQuad)
            .OnComplete(() => 
            {
                // Büyüme bitince yumuşakça orijinal boyuta geri döner
                Tween.Scale(this.transform, endValue: orginalScale, duration: 0.15f, ease: Ease.InQuad);
            });

        // 3. ERKEN BAŞLAYAN SÜREKLİ TİTREME (3 tabakta başlar!)
        if (dirtyDishesCount >= 3)
        {
            // 3 tabaktayken çok hafif (0.03f), tamamen doluyken daha şiddetli (0.15f)
            float constantShakeStrength = Mathf.Lerp(0.03f, 0.09f, fillRatio);

            Vector3 shakeTarget = originalLocalPosition + new Vector3(constantShakeStrength, 0f, 0f);

            dishesTween = Tween.LocalPosition(
                this.transform, 
                endValue: shakeTarget, 
                duration: 0.05f, // Milisaniyelik motor titremesi
                cycles: -1,      // Sonsuza kadar
                cycleMode: CycleMode.Yoyo, // Git-Gel
                ease: Ease.InOutSine
            );
        }
    }
}