using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingToTableState : CustomerState
{
    private List<Transform> targetSeats;
    private TableController targetTable;

    public WalkingToTableState(CustomerController controller, List<Transform> targetSeats , TableController targetTable) : base(controller)
    {
        this.targetSeats = targetSeats;
        this.targetTable = targetTable;
    }

    public override void Enter()
    {
        customerController.currentTable = targetTable;
        customerController.mainAnimator.SetBool("isWalking", true);

        customerController.customerMovement.StartMoving();

        customerController.OffCollider();

    }

    public override void Update()
    {
        bool reached = customerController.customerMovement.MoveTo(targetTable.interactionPoint.position , customerController.customer.customerSpeed);

        if(reached)
        {
            // bu kıssımda gurubun altındaki her bir customeri seat atayıp ona oturtucaz do move ile 


            //customerController.transform.position = targetSeat.position;

            foreach (var customer in customerController.customers)
            {
                Transform seat = targetSeats[customer.seatIndex];
                customer.transform.position = seat.position;
            }

            customerController.customerMovement.StopMoving();

            customerController.ChangeState(new MakingDecisionState(customerController));
        }
    }

    public override void Exit()
    {
        customerController.mainAnimator.SetBool("isWalking", false);
    }
}
