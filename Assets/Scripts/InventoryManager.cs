using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] slots;

    public bool AddItem(Sprite itemSprite)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(itemSprite);
                return true;
            }
        }

        Debug.Log("Inventory Full");
        return false;
    }
}