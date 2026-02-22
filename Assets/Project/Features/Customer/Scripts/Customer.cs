using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Customer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CustomerData[] customerData;

    private SpriteRenderer spriteRenderer;


    public float customerSpeed;
    public float patienceMultiplier;

    public string customerName;

    public int seatIndex = -1; 


    void Start()
    {
        Initialize();
    }


    public void Initialize()
    {
        int randomIndex = Random.Range(0, customerData.Length);

        customerSpeed = customerData[randomIndex].customerSpeed;
        patienceMultiplier = customerData[randomIndex].patienceMultiplier;
        customerName = customerData[randomIndex].customerName;
        // Debug.Log("Müşteri Seçildi: " + customerData[randomIndex].customerName);
        // Debug.Log("Hız: " + customerData[randomIndex].customerSpeed);
        // Debug.Log("Sabır Süresi: " + customerData[randomIndex].patienceTime);
        //spriteRenderer = GetComponent<SpriteRenderer>();
        // bu scritpin amacaı base bir sınıf oluşturup oradan türetmek olabilir.
    }
 
}
