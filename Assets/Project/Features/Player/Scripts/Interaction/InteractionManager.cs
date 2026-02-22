using UnityEngine;
    using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Layers")]
    public LayerMask customerLayer; 
    public LayerMask tableLayer;
    public LayerMask foodLayer;

    public LayerMask trashLayer;

    public LayerMask dishesStationLayer;

    private PlayerController playerController;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerController = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        PlayerInputHandler.OnClickStartedEvent += HandleClick;
    }

    void OnDisable()
    {
        PlayerInputHandler.OnClickStartedEvent -= HandleClick;
    }

    private void HandleClick(Vector2 pos)
    {
        // Tıklamayı o anki State'e yönlendir
        if (playerController.currentState != null)
        {
            playerController.currentState.OnClick(pos);
        }
    }

    // --- YARDIMCI RAYCAST METODLARI ---
    
    public CustomerController GetCustomerAtPosition(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, customerLayer);

        if (hit.collider != null)
        {
            CustomerController customer = hit.collider.GetComponentInParent<CustomerController>();
            
            if (customer == null) 
            {
                Debug.LogError($"HATA: {hit.collider.name} objesinde veya ebeveyninde 'CustomerController' bulunamadı!");
            }

            return customer;
        }
        
        return null;
    }
    public Food GetFoodAtPosition(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, foodLayer);
        return hit.collider != null ? hit.collider.GetComponent<Food>() : null;
    }

    public TableController GetTableAtPosition(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, tableLayer);
        return hit.collider != null ? hit.collider.GetComponent<TableController>() : null;
    }

    public Trash GetTrashAtPosition(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, trashLayer);
        return hit.collider != null ? hit.collider.GetComponent<Trash>() : null;
    }

    public DishesStation GetDishesStation(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, dishesStationLayer);
        return hit.collider != null ? hit.collider.GetComponent<DishesStation>() : null;
    }



}