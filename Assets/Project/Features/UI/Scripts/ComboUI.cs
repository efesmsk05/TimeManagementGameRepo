using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using DG.Tweening.Core.Enums;

public class ComboUI : MonoBehaviour
{
    public static ComboUI Instance;
    [Header("Settings")]
    [SerializeField] private GameObject textPrefab; // Senin hazırladığın prefab
    [SerializeField] private Transform container;   // Canvas'ın içinde bir "Panel" olsun, dağınıklık olmasın
    
    [Header("Animation")]
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private float floatDistance = 100f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    // Basit bir Object Pooling (Performans için)
    private Queue<TextMeshProUGUI> textPool = new Queue<TextMeshProUGUI>();

    void OnEnable()
    {
        EventBus.Subscribe<UIEvent.ComboUpdateEvent>( ShowComboText);

   
    }

    void OnDisable()
    {
        
        EventBus.Unsubscribe<UIEvent.ComboUpdateEvent>( ShowComboText);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ShowComboText(UIEvent.ComboUpdateEvent evt)
    {
        // 1. Havuzdan veya yeni obje al
        TextMeshProUGUI tmp = GetTextFromPool();

        // 2. Pozisyonu Ayarla (3D Dünyadan -> 2D Ekrana)
        // Kameranın gördüğü noktaya çeviriyoruz
        Vector2 screenPos = Camera.main.WorldToScreenPoint(evt.worldPosition);
        tmp.transform.position = screenPos;

        // 3. Metni ve Rengi Ayarla
        tmp.text = $"x{evt.currentCombo}";
        
        // Renk değişimi (Combo arttıkça renk kızarır)
        if (evt.currentCombo < 3) tmp.color = Color.white;
        else if (evt.currentCombo < 6) tmp.color = Color.yellow;
        else tmp.color = new Color(1f, 0.5f, 0f); // Turuncu/Altın

        // 4. Resetle
        tmp.gameObject.SetActive(true);
        tmp.transform.localScale = Vector3.zero; // Büyüyerek çıkacak
        tmp.alpha = 1f;

        // 5. DOTween Şov Başlasın! 🚀
        Sequence seq = DOTween.Sequence();

        // A. Büyüyerek çık (Punch)
        seq.Append(tmp.transform.DOScale(Vector3.one * (1f + (evt.currentCombo * 0.25f)), 0.3f).SetEase(scaleEase));
        
        // B. Yukarı süzül ve kaybol (Eş zamanlı)
        seq.Join(tmp.transform.DOMoveY(screenPos.y + floatDistance, floatDuration).SetEase(moveEase));
        seq.Join(tmp.DOFade(0f, floatDuration * 0.5f).SetDelay(floatDuration * 0.5f));

        // C. İş bitince havuza geri at
        seq.OnComplete(() => ReturnToPool(tmp));
    }

    private TextMeshProUGUI GetTextFromPool()
    {
        if (textPool.Count > 0)
        {
            return textPool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(textPrefab, container);
            return obj.GetComponent<TextMeshProUGUI>();
        }
    }

    private void ReturnToPool(TextMeshProUGUI tmp)
    {
        tmp.gameObject.SetActive(false);
        textPool.Enqueue(tmp);
    }
}
