using UnityEngine;
using UnityEngine.UI;
using PrimeTween;


public class ErrorFeedbackUI : MonoBehaviour
{
    [SerializeField] private Image redFlashImage;

    [SerializeField] private Camera redFlashCamera;

    void OnEnable()
    {
        EventBus.Subscribe<GameEvents.OnErrorEvent>(HandleOnError);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameEvents.OnErrorEvent>(HandleOnError);
    }

    void Start()
    {
        Color c = redFlashImage.color;
        c.a = 0f;
        redFlashImage.color = c;
    }

    public void PlayErrorFlash()
    {
        // Önce aniden kızarır (0.15 saniye), sonra yavaşça solar (0.3 saniye)
        Tween.Alpha(redFlashImage, startValue: 0f, endValue: 0.20f, duration: 0.15f)
             .Chain(Tween.Alpha(redFlashImage, endValue: 0f, duration: 0.20f, ease: Ease.InSine));
        
        Tween.ShakeLocalPosition(redFlashCamera.transform, strength: new Vector3(0.03f, 0.03f, 0f), duration: 0.2f, frequency: 10);

        
    }

    private void HandleOnError(GameEvents.OnErrorEvent e)
    {
        PlayErrorFlash();
    }


    public void PlayFoodReadyEffect(GameObject foodObj)
    {
        Transform foodT = foodObj.transform;

        // 1. GÜVENLİK: Varsa eski animasyonları durdur ve boyutu sıfırla
        Tween.StopAll(foodT);
        foodT.localScale = Vector3.zero;

        // 2. ÇIKIŞ (POP) EFEKTİ: Sıfırdan normal boyutuna yaylanarak büyür
        Tween.Scale(foodT, endValue: Vector3.one, duration: 0.4f, ease: Ease.OutBack)
            .OnComplete(() => 
            {
                // 3. DİKKAT ÇEKME (NEFES ALMA): Çıkış bitince sürekli %10 büyüyüp küçülür
                // cycles: -1 sonsuza kadar tekrar etmesini sağlar
                Tween.Scale(foodT, endValue: Vector3.one * 1.1f, duration: 0.8f, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.InOutSine);
            });
    }

    // Oyuncu veya garson yemeği tezgahtan aldığında bunu çağır ki animasyon dursun!
    public void StopFoodEffect(GameObject foodObj)
    {
        Tween.StopAll(foodObj.transform);
        foodObj.transform.localScale = Vector3.one; // Boyutunu normale döndür
    }
}