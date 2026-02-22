using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/Customer/CustomerData", order = 1)]
public class CustomerData : ScriptableObject
{

    // customer properties
    public float customerSpeed;
    public float patienceMultiplier;

    public string customerName;

    public Sprite sprite ;

}
