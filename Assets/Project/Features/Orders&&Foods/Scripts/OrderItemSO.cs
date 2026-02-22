using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "OrderItemSO", menuName = "ScriptableObjects/OrderItem")]
public class OrderItemSO : ScriptableObject
{
    public string foodName;

    public float foodPrice;

    public Sprite foodSprite;

    public float prepTime;

    public float eatTime;
}
