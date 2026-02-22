using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LeavingShopState : CustomerState
{

    float offsetX = 0f;

    public LeavingShopState(CustomerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        // 1. Collider'ı geri aç
        customerController.OnCollider();

        if(customerController.customers.Count <2 )
        {
            customerController.customerMovement.StartMoving();
            customerController.mainAnimator.SetBool("isWalking", true);
            return;
        }
        foreach (var customer in customerController.customers)
        {
            Vector3 newPos = new Vector3( (-.5f) + offsetX, 0f ,0f ); // Ana objenin pozisyonu

            customer.transform.localPosition = newPos;  // ana merkeze ışınlıyoruz

            customer.transform.localRotation = Quaternion.identity;

            offsetX += .8f; // Sonraki müşteri için biraz sağa kaydır
        }

        customerController.customerMovement.StartMoving();
        customerController.mainAnimator.SetBool("isWalking", true);
    }

    public override void Update()
    {
        bool hasLeft = customerController.customerMovement.MoveTo(LevelManager.Instance.customerExitPos.position, customerController.customer.customerSpeed);

        if (hasLeft)
        {
            customerController.customerMovement.StopMoving();
            customerController.DeSpawnSelf();
        }
    }

    public override void Exit()
    {
        customerController.mainAnimator.SetBool("isWalking", false);
    }
}