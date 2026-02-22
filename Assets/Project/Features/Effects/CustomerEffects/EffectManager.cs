using UnityEngine;
using PrimeTween;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        EventBus.Subscribe<CustomerEvents.CustomerSelectedEvent>(SelectedEffect);
        EventBus.Subscribe<CustomerEvents.CustomerDeselectedEvent>(DeselectedEffect);
        EventBus.Subscribe<CustomerEvents.OrderTakenEvent>(OrderTakenEffect);
        EventBus.Subscribe<CustomerEvents.CustomerStartEatingEvent>(CustomerEatingEffect);
        EventBus.Subscribe<CustomerEvents.CustomerLeftEvent>(CustomerLeftEffect);
        EventBus.Subscribe<CustomerEvents.CustomerMakeDecisionEvent>(CustomerThinkingEffect);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<CustomerEvents.CustomerSelectedEvent>(SelectedEffect);
        EventBus.Unsubscribe<CustomerEvents.CustomerDeselectedEvent>(DeselectedEffect);
        EventBus.Unsubscribe<CustomerEvents.OrderTakenEvent>(OrderTakenEffect);
        EventBus.Unsubscribe<CustomerEvents.CustomerStartEatingEvent>(CustomerEatingEffect);
        EventBus.Unsubscribe<CustomerEvents.CustomerLeftEvent>(CustomerLeftEffect);
        EventBus.Unsubscribe<CustomerEvents.CustomerMakeDecisionEvent>(CustomerThinkingEffect);
    }

    public void SelectedEffect(CustomerEvents.CustomerSelectedEvent target)
    {
        for (int i = 0; i < target.Customer.customers.Count; i++)
        {
            SpriteRenderer spriteRenderer = target.Customer.customers[i].GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.cyan;
            }
        }

        Transform visualTransform = target.Customer.transform;

        
        Tween.Scale(visualTransform, endValue: Vector3.one * 1.1f, duration: 0.5f, ease: Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo);
    }
    

    public void DeselectedEffect(CustomerEvents.CustomerDeselectedEvent target)
    {
       
        for (int i = 0; i < target.Customer.customers.Count; i++)
        {
            SpriteRenderer spriteRenderer = target.Customer.customers[i].GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        Transform visualTransform = target.Customer.transform;

        Tween.StopAll(visualTransform); // tween effektlerini durduruyoruz
        visualTransform.localScale = Vector3.one; 

    }

    public void CustomerThinkingEffect(CustomerEvents.CustomerMakeDecisionEvent customer)
    {
       // Efekt kodu
    }

    public void OrderTakenEffect(CustomerEvents.OrderTakenEvent customer)
    {
        SpriteRenderer spriteRenderer = customer.Customer.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
        }
    }

    public void CustomerEatingEffect(CustomerEvents.CustomerStartEatingEvent customer)
    {
        SpriteRenderer spriteRenderer = customer.Customer.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }
    }

    public void CustomerLeftEffect(CustomerEvents.CustomerLeftEvent customer)
    {
        SpriteRenderer spriteRenderer = customer.Customer.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)// müşteri ayrııldığında rengini resetliyoruz
        {
            spriteRenderer.color = Color.white; 
        }
    }

    public void CustomerAngryEffect(CustomerController customerController)
    {
        SpriteRenderer spriteRenderer = customerController.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }
}