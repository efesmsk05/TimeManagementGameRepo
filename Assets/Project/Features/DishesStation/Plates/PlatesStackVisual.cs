using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class PlatesStackVisual : MonoBehaviour
{
    [Header("Settings")]
    public float plateOffsetY = 0.03f; 
    public float jumpHeight = 0.5f;   // Tabak eksilirken zıplayacağı yükseklik

    private List<GameObject> plateVisuals = new List<GameObject>();
    private Vector3 originalScale; 

    public void Initialize(int maxCapacity, GameObject prefabToUse)
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        plateVisuals.Clear();

        originalScale = prefabToUse.transform.localScale;

        for (int i = 0; i < maxCapacity; i++)
        {
            GameObject plate = Instantiate(prefabToUse, transform);
            
            SpriteRenderer sr = plate.GetComponent<SpriteRenderer>();
            if(sr != null) sr.sortingOrder += i; 

            plate.transform.localPosition = new Vector3(0, i * plateOffsetY, 0);
            
            plate.transform.localScale = Vector3.zero;
            plate.SetActive(false);
            
            plateVisuals.Add(plate);
        }
    }

    public void UpdateStackCount(int currentCount)
    {
        for (int i = 0; i < plateVisuals.Count; i++)
        {
            GameObject plate = plateVisuals[i];
            Transform plateT = plate.transform;

            Tween.StopAll(plateT);

            Vector3 originalPos = new Vector3(0, i * plateOffsetY, 0);

            if (i < currentCount) // yığıan tabak eklerken
            {
                if (!plate.activeSelf) 
                {
                    plate.SetActive(true);
                    plateT.localScale = Vector3.zero; 
                    
                    plateT.localPosition = originalPos; 
                }

                Tween.Scale(plateT, endValue: originalScale, duration: 0.2f, ease: Ease.OutBack);
            }
            else// yığından tabak eksiltirken
            {
                if (plate.activeSelf)
                {
                    Vector3 jumpPos = originalPos + new Vector3(0, jumpHeight, 0);

                    Tween.LocalPosition(plateT, endValue: jumpPos, duration: 0.75f, ease: Ease.OutQuad);// jump effect

                    Tween.Scale(plateT, endValue: Vector3.zero, duration: 0.5f, ease: Ease.InBack)
                         .OnComplete(() => 
                         {
                             plate.SetActive(false);
                             plateT.localPosition = originalPos; 
                         });
                }
            }
        }
    }
}