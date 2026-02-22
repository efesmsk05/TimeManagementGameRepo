using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{

    public void Interact(PlayerInventory playerInventory)
    {
        Debug.Log("Çöp kutusuyla etkileşim gerçekleşti.");
        playerInventory.ThrowTrash();
        

    }
}
