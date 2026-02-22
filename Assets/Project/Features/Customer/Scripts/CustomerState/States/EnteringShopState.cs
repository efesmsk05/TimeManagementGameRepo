using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteringShopState : CustomerState
{
    public EnteringShopState(CustomerController customerController) : base(customerController)
    {
    }

    public override void Enter()
    {

        customerController.mainAnimator.SetBool("isIdle", false);
        customerController.mainAnimator.SetBool("isWalking", true);
        customerController.customerMovement.StartMoving();

    }

    public override void Update()
    {
        bool hasArrived = customerController.customerMovement.MoveTo(customerController.currentTargetPos, customerController.customer.customerSpeed);

        if (hasArrived)
        {

            customerController.customerMovement.StopMoving();
            customerController.ChangeState(new WaitingForSeatState(customerController));
        }
    }

    public override void Exit()
    {
        customerController.mainAnimator.SetBool("isWalking", false);

    }
}
