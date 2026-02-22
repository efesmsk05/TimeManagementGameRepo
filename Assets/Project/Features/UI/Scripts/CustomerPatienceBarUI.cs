using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class CustomerPatienceUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas canvas; 
    [SerializeField] private Image fillImage;

    [Header("Gradients")]
    [SerializeField] private Gradient standardGradient;
    [SerializeField] private Gradient waiterGradient;
    [SerializeField] private Gradient orderGradient;

    private Gradient currentGradient;
    private Vector3 initialScale;
    
    private Tween scaleTween;
    private Tween fillTween;

    void Awake()
    {
        // 1. ÖNCE Inspector'da ayarladığın boyutu kaydet.
        // (Editörde scale'i 0.001 yaptıysan onu alır, 0.5 yaptıysan onu alır)
        initialScale = transform.localScale;

        // 2. Güvenlik Kontrolünü Düzeltelim:
        // Sadece eğer scale GERÇEKTEN 0 ise (unutulduysa) müdahale etsin.
        // Mathf.Epsilon, 0'a en yakın sayı demektir.
        if (initialScale.x <= Mathf.Epsilon || initialScale.y <= Mathf.Epsilon) 
        {
            initialScale = Vector3.one; // Sadece tamamen 0 ise 1 yap
        }
    }

    void Start()
    {
        if(canvas != null) canvas.enabled = false;
        transform.localScale = Vector3.zero; 
    }

    public void SetupBar(float duration, PatienceBarType type)
    {
        switch (type)
        {
            case PatienceBarType.waiterWait: currentGradient = waiterGradient; break;
            case PatienceBarType.orderWait: currentGradient = orderGradient; break;
            default: currentGradient = standardGradient; break;
        }

        if(canvas != null) canvas.enabled = true;

        if (scaleTween.isAlive) scaleTween.Stop();
        if (fillTween.isAlive) fillTween.Stop();

        // --- DÜZELTME BURADA ---
        // Animasyona başlamadan önce scale'i sıfırlıyoruz ki "Zaten 1" hatası vermesin.
        transform.localScale = Vector3.zero;
        
        scaleTween = Tween.Scale(transform, endValue: initialScale, duration: 0.4f, ease: Ease.OutBack);

        fillImage.fillAmount = 1f; 
        if(currentGradient != null) fillImage.color = currentGradient.Evaluate(1f);

        if (duration <= 0)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        fillTween = Tween.Custom(startValue: 1f, endValue: 0f, duration: duration, onValueChange: val =>
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = val;
                if (currentGradient != null) fillImage.color = currentGradient.Evaluate(val);
            }
        });
    }

    public void HideBar()
    {
        // --- DÜZELTME BURADA ---
        // Eğer obje zaten çok küçükse (0 ise), animasyon yapma, direkt kapat.
        // Bu sayede "0'dan 0'a scale etmeye çalışıyorsun" hatası gelmez.
        if (transform.localScale.sqrMagnitude < 0.01f)
        {
            if (canvas != null) canvas.enabled = false;
            return;
        }

        if (scaleTween.isAlive) scaleTween.Stop();
        if (fillTween.isAlive) fillTween.Stop();

        scaleTween = Tween.Scale(transform, endValue: Vector3.zero, duration: 0.2f, ease: Ease.InBack)
            .OnComplete(() => 
            {
                if (canvas != null) canvas.enabled = false;
            });
    }

    private void OnDestroy()
    {
        if (scaleTween.isAlive) scaleTween.Stop();
        if (fillTween.isAlive) fillTween.Stop();
    }
}