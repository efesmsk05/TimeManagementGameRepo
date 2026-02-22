using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using PrimeTween;

public enum FoodFreshness
{
    Hot,
    Warm,
    Cold
}


public class Food : MonoBehaviour
{

    private string foodName;

    private SpriteRenderer spriteRenderer;
    public Sprite foodSprite;
    public CustomerController ownerCustomer;

    public float foodPrice; // yemeğin fiyatı

    public bool isTrash  = false;

    private float foodLifetime ; // Çöp olduktan sonra yok olma süresi

    public FoodFreshness freshness = FoodFreshness.Hot;

    private CancellationTokenSource cts;


    public void DeSpawn()
    {
        CancelLifeTimeProcess();

        ownerCustomer = null;
        FoodPool.Instance.ReturnFoodItem(this.gameObject);

    }
    public void Initialize(OrderItemSO orderItemSO)
    {
        foodName = orderItemSO.name;
        foodSprite = orderItemSO.foodSprite;
        foodPrice = orderItemSO.foodPrice;

        foodLifetime = orderItemSO.prepTime * 5f;// sonradan dengeleme aşamasında dünzelenecek 

        //foodLifetime = customer.;

        spriteRenderer.sprite = foodSprite;

        isTrash = false;
        freshness = FoodFreshness.Hot;

       
        CancelLifeTimeProcess();// daha önce başlatılmış bir unitask varsa iptal et
        // Unitask başlatma 
        cts = new CancellationTokenSource();

    
        PlayFoodReadyEffect();

        LifeTimeAsync(cts.Token).Forget();// forget ile çağırıyoruz çünkü dönüş değeri yok bu arkada çalışacak


    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private async UniTaskVoid LifeTimeAsync(CancellationToken token)
    {
        bool isCanceled =  await UniTask.Delay(TimeSpan.FromSeconds(foodLifetime * .5f), cancellationToken: token).SuppressCancellationThrow();
        // burada sürenin yarısına kadar beklerken iptal edilip edilmediğini kontrol ediyoruz
        // eğer bu süreye kadar müşteriye verilirse bu task döndürme iptal et diyoruz 


        if(isCanceled) return;

        SetWarm();

        isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(foodLifetime * .5f), cancellationToken: token).SuppressCancellationThrow();

        if(isCanceled) return;

        SetCold();
        
    }

    private void CancelLifeTimeProcess()
    {
        if(cts != null && !cts.IsCancellationRequested)
        {
            cts.Cancel();
            cts.Dispose();// bellek sızıntısını önlemek için dispose ediyoruz (elden çıkararak kaynakları temizliyoruz)
            cts = null;
        }
    }


    private void TrashFood()
    {
        isTrash = true;
        // Görsel değişikliği veya diğer işlemler burada yapılabilir
    }

    private void SetWarm()
    {
        // Görsel değişikliği veya diğer işlemler burada yapılabilir
        TrashFood();
        freshness = FoodFreshness.Warm;
        spriteRenderer.color = Color.yellow;

    }

    private void SetCold()
    {
        // Görsel değişikliği veya diğer işlemler burada yapılabilir
        freshness = FoodFreshness.Cold;
        spriteRenderer.color = Color.blue;

    }

    public void PlayFoodReadyEffect()
    {
        Transform foodT = this.transform;

        // 1. GÜVENLİK: Varsa eski animasyonları durdur ve boyutu sıfırla
        Tween.StopAll(foodT);
        foodT.localScale = Vector3.zero;

        // 2. ÇIKIŞ (POP) EFEKTİ: Sıfırdan normal boyutuna yaylanarak büyür
        Tween.Scale(foodT, endValue: Vector3.one, duration: 0.4f, ease: Ease.OutBack)
            .OnComplete(() => 
            {
                // 3. DİKKAT ÇEKME (NEFES ALMA): Çıkış bitince sürekli %10 büyüyüp küçülür
                // cycles: -1 sonsuza kadar tekrar etmesini sağlar
                Tween.Scale(foodT, endValue: Vector3.one * 1.25f, duration: 0.65f, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.InOutSine);
            });
    }

    // Oyuncu veya garson yemeği tezgahtan aldığında bunu çağır ki animasyon dursun!
    public void StopFoodEffect()
    {
        Tween.StopAll(this.transform);
        this.transform.localScale = Vector3.one; // Boyutunu normale döndür
    }

    public void ResetFoodVisual()
    {
        StopFoodEffect();
        spriteRenderer.color = Color.white; // Rengi normale döndür
    }   

}
