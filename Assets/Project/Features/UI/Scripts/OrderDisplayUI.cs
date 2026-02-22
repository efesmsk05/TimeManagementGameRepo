using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween; 

public class OrderDisplayUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject gridParent;

    [Header("Fill Image for Timer")]
    [SerializeField] private Image fillImage;
    
    private List<Image> cachedCells = new List<Image>();

    private bool[] activeCells;
    private Tween currentTween;

    // 1. Orijinal boyutu hafızada tutmak için değişken
    private Vector3 initialScale;

    Tween fillTween;

    Tween foodServedTween;

    void Awake()
    {
        // 2. Editörde ayarladığın mükemmel boyutu (örn: 0.006) oyun başlarken kaydet
        initialScale = transform.localScale;

        activeCells = new bool[gridParent.transform.childCount];

        // Güvenlik: Eğer prefab yanlışlıkla Scale 0 olarak kaydedildiyse varsayılan bir değer ver
        if (initialScale.x <= Mathf.Epsilon) initialScale = new Vector3(0.006f, 0.006f, 0.006f);

        cachedCells.Clear();

        foreach(Transform child in gridParent.transform)
        {
            var cell = child.GetComponent<Image>();
            if (cell != null)
            {
                cachedCells.Add(cell);
                child.gameObject.SetActive(false); 
            }
        }
    }

    public void DisplayOrder(List<OrderItemSO> orderItems , float displayDuration)
    {
        for (int i = 0; i < cachedCells.Count; i++)
        {
            if (i < orderItems.Count)
            {
                cachedCells[i].sprite = orderItems[i].foodSprite;
                cachedCells[i].color = Color.white; 
                cachedCells[i].gameObject.SetActive(true);

                activeCells[i] = true;
            }
            else
            {
                cachedCells[i].sprite = null;
                cachedCells[i].gameObject.SetActive(false);
                activeCells[i] = false;
            }
        }

        if (currentTween.isAlive) currentTween.Stop();

        if (fillTween.isAlive) fillTween.Stop();
        

        gameObject.SetActive(true);
        
        // 3. Yoruma aldığın satırı geri açtım. Balon sıfırdan "POP" diye patlayarak çıksın
        transform.localScale = Vector3.zero; 

        // 4. Sabit 0.01f yerine, hafızaya aldığımız initialScale (0.006) değerine kadar büyüsün!
        currentTween = Tween.Scale(transform, endValue: initialScale, duration: 0.5f, ease: Ease.OutBack);



        if (displayDuration > 0)
        {
            if (fillImage != null) fillImage.fillAmount = 1f;

            fillTween = Tween.UIFillAmount(fillImage, endValue: 0f, duration: displayDuration, ease: Ease.Linear);
        }
    }

    public void HideOrder()
    {   
        if (currentTween.isAlive) currentTween.Stop();

        if (fillTween.isAlive) fillTween.Stop();

        currentTween = Tween.Scale(transform, endValue: Vector3.zero, duration: 0.3f, ease: Ease.InBack)
            .OnComplete(() => 
            {
                foreach (var cell in cachedCells)
                {
                    cell.sprite = null;
                    cell.gameObject.SetActive(false);
                }
                gameObject.SetActive(false);
            });

  
     
            
    }

    // yemek verildiğinde doğru yemeği yeşil yap verildiğini belirticez
    public void MarkFoodAsServed(Sprite fooodSprite)
    {
        
        for (int i = 0; i < cachedCells.Count; i++)
        {
            if (cachedCells[i].sprite == fooodSprite && activeCells[i])
            {
                cachedCells[i].color = Color.green; // Doğru yemeği yeşil yap

                Transform targetCell = cachedCells[i].transform;

                Tween.PunchScale(targetCell, strength: new Vector3(0.3f, -0.3f, 0), duration: 0.3f, frequency: 10);

                activeCells[i] = false; 

                break; // İlk eşleşmeyi bulduktan sonra döngüyü kır

            }
        }
    }
    

}