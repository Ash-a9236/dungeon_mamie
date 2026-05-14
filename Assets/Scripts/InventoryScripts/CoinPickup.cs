using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public Sprite coinSprite;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching the coin is tagged "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player picked up the coin!");
            
            if (Inventory.instance != null)
            {
                Inventory.instance.AddItem(coinSprite);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Inventory instance not found! Is the script on an active object?");
            }
        }
    }
}