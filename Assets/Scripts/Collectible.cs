using UnityEngine;

public class Collectible : MonoBehaviour
{
    public Sprite itemSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager inventory =
                FindObjectOfType<InventoryManager>();

            if (inventory != null)
            {
                bool added = inventory.AddItem(itemSprite);

                if (added)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}