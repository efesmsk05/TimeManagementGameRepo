using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class TableNotificationUI : MonoBehaviour
{
    [Header("Order Notification Elements")]
    [SerializeField] private Image fillImage; 
    [SerializeField] private Canvas canvas;

    private Vector3 initialScale;

    private Tween punchTween;
    private Tween fillTween;

    void Awake()
    {
        initialScale = transform.localScale;
        if (initialScale.sqrMagnitude < 0.0001f) initialScale = Vector3.one;
    }

    void Start()
    {
        if(canvas != null) canvas.enabled = false;
        transform.localScale = Vector3.zero;
    }

    public void ShowNotification(float duration)
    {
        // 1. GÜVENLİK: Obje hiyerarşide aktif değilse animasyon oynatma (Sarı uyarıyı önler)
        if (!gameObject.activeInHierarchy) return;

        if (canvas != null) canvas.enabled = true;

        if (punchTween.isAlive) punchTween.Stop();
        if (fillTween.isAlive) fillTween.Stop();

        transform.localScale = Vector3.zero;
        fillImage.fillAmount = 1f;

        Tween.Scale(transform, endValue: initialScale, duration: 0.4f, ease: Ease.OutBack).OnComplete(() => 
        {
            punchTween = Tween.Scale(transform, endValue: initialScale * 1.15f, duration: 0.9f, cycles: -1, cycleMode: CycleMode.Yoyo, ease: Ease.InOutSine);
        });

        if (duration > 0)
        {
            fillTween = Tween.UIFillAmount(fillImage, endValue: 0f, duration: duration, ease: Ease.Linear);
        }
    }

    public void HideNotification()
    {
        if (!gameObject.activeInHierarchy || transform.localScale.sqrMagnitude < 0.001f)// güvenlik kontrolü
        {
            if(canvas != null) canvas.enabled = false;
            return;
        }

        if (punchTween.isAlive) punchTween.Stop(); 
        if (fillTween.isAlive) fillTween.Stop(); 
        
        Tween.Scale(transform, endValue: Vector3.zero, duration: 0.2f, ease: Ease.InBack)
            .OnComplete(() => 
            {
                if(canvas != null) canvas.enabled = false;
            });
    }

    private void OnDestroy()
    {
        if (punchTween.isAlive) punchTween.Stop();
        if (fillTween.isAlive) fillTween.Stop();
    }
}